using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NLog;
using System.ComponentModel.DataAnnotations;
using WebApiCrawler.Data;
using WebApiCrawler.Models;
using Mapster;
using WebApiCrawler.Services;

namespace WebApiCrawler.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class ImageController : ControllerBase
    {

        private readonly CrawlerDbContext _context;
        private readonly NLog.ILogger _logger;

        public ImageController(CrawlerDbContext dbContext, NLog.ILogger logger)
        {
            _context = dbContext;
            _logger=logger;
        }

        /// <summary>
        /// Adds a new image to the database
        /// </summary>
        /// <param name="imageDto">The image data to add</param>
        /// <returns></returns>
        [HttpPost("AddImage")]
        public async Task<IActionResult> AddImage(ImageDto imageDto)
        {
            IImageService imageService = new ImageService(_context);
            bool imageExists = await imageService.ImageExists(imageDto);

            if (imageDto != null && !imageExists)
            {
                return await imageService.AddNewImage(imageDto);
            }
            return BadRequest("Image already in database.");
        }

        /// <summary>
        /// Retrieves all images data from the database
        /// </summary>
        /// <returns>
        /// IActionResult containing a list of images if found,
        /// otherwise returns a NotFound response indicating that no images were found
        /// </returns>
        [HttpGet("GetImagesData")]
        public async Task<IActionResult> GetImagesData()
        {
            var images = await _context.Images.ToListAsync();
            if (images.Count == 0)
            {
                _logger.Warn("No images were found in the database.");
                return NotFound("No images were found in the database.");
            }
            return Ok(images);
        }

        /// <summary>
        /// Retrieves a image data with the specified ID from the database
        /// </summary>
        /// <param name="id">The ID of the image to retrieve</param>
        /// <returns>
        /// IActionResult containing the image if found,
        /// otherwise returns a BadRequest response indicating that the store was not found
        /// </returns>
        [HttpGet("GetImageData")]
        public async Task<IActionResult> GetImageData([Required] Guid id)
        {
            var image = await _context.Images.FindAsync(id);
            if (image == null)
            {
                _logger.Warn($"Image with ID {id} was not found.");
                return BadRequest($"Image with ID {id} was not found.");
            }
            return Ok(image);
        }

        /// <summary>
        /// Retrieves an image from the specified file path
        /// </summary>
        /// <param name="path">The file path of the image to retrieve</param>
        /// <returns>
        /// If successful, returns the image file with the appropriate content type.
        /// If the image does not exist in the database or the file system, returns a Not Found error
        /// If an error occurs while reading the image, returns a 500 Internal Server Error with details
        /// </returns>
        ///  
        [HttpGet("GetImageByPath")]
        public async Task<IActionResult> GetImageByPath([Required] string path)
        {
            var validPath = path.Replace("\\\\", "\\");
            var image = await _context.Images.Where(i => i.Path.Equals(validPath)).FirstOrDefaultAsync();
            if (image == null)
            {
                _logger.Error($"Image with path {path} was not found.");
                return BadRequest($"Image with path {path} was not found.");
            }

            return await GetImage(image);
        }

        /// <summary>
        /// Retrieves an image from the specified id
        /// </summary>
        /// <param name="id">The id of the image to retrieve</param>
        /// <returns>
        /// If successful, returns the image file with the appropriate content type.
        /// If the image does not exist in the database or the file system, returns a Not Found error
        /// If an error occurs while reading the image, returns a 500 Internal Server Error with details
        /// </returns>
        [HttpGet("GetImageById")]
        public async Task<IActionResult> GetImageById([Required] Guid id)
        {
            var image = await _context.Images.FindAsync(id);
            if (image == null)
            {
                _logger.Error($"Image with ID {id} was not found.");
                return BadRequest($"Image with ID {id} was not found.");
            }
            return await GetImage(image);
        }

        /// <summary>
        /// Deletes a image from the database
        /// </summary>
        /// <param name="id">The ID of the image to delete</param>
        /// <returns>The result of the operation</returns>
        [HttpDelete("DeleteImage")]
        public async Task<IActionResult> DeleteImage([Required] Guid id)
        {
            var image = await _context.Images.FindAsync(id);
            if (image == null)
            {
                _logger.Warn($"Image with ID {id} was not found.");
                return NotFound($"Image with ID {id} was not found.");
            }

            try
            {
                _context.Images.Remove(image);
                await _context.SaveChangesAsync();

                if (await FileExists(image.Path))
                {
                    System.IO.File.Delete(image.Path);
                    _logger.Info($"Image with ID {id} has been deleted from file successfully.");
                }

                _logger.Info($"Image with ID {id} has been deleted successfully.");
                return Ok($"Image with ID {id} has been deleted successfully.");
            }
            catch (Exception ex)
            {
                _logger.Error($"Error deleting image: {ex.Message}");
                return StatusCode(500, $"Error deleting image: {ex.Message}");
            }
        }

        /// <summary>
        /// Updates an existing image in the database
        /// </summary>
        /// <param name="id">>The ID of the image to update</param>
        /// <param name="imageDto">The image data to update</param>
        /// <returns>The result of the operation</returns>
        [HttpPut("UpdateImage")]
        public async Task<IActionResult> UpdateImage([Required] Guid id, ImageDto imageDto)
        {
            try
            {
                var existingImage = await _context.Images.FindAsync(id);
                if (existingImage == null)
                {
                    _logger.Warn($"Image with ID {id} was not found.");
                    return NotFound($"Image with ID {id} was not found.");
                }

                if (!await ProductExists(imageDto.ProductId))
                {
                    _logger.Warn($"Product with ID {imageDto.ProductId} was not found.");
                    return NotFound($"Product with ID {imageDto.ProductId} was not found.");
                }

                existingImage.Path = imageDto.Path;
                existingImage.ProductId = imageDto.ProductId;

                _context.Images.Update(existingImage);
                await _context.SaveChangesAsync();

                _logger.Info($"Image updated {existingImage.Id}");
                return Ok(existingImage);
            }
            catch (Exception ex)
            {
                _logger.Error($"Error occurred while updating image: {ex.Message}");
                return StatusCode(500, $"Error occurred while updating image: {ex.Message}");
            }
        }

        /// <summary>
        ///  Checks if a product exists in the database
        /// </summary>
        /// <param name="productId">The ID of the product to check</param>
        /// <returns>True if the product exists, otherwise false</returns>
        private async Task<bool> ProductExists(Guid productId)
        {
            var product = await _context.Products.FindAsync(productId);
            if (product == null)
            {
                return false;
            }
            return true;
        }

        /// <summary>
        /// Checks if the specified file exists at the provided path
        /// </summary>
        /// <param name="path">The path of the file to check</param>
        /// <returns>True if the file exists; otherwise, false</returns>
        private async Task<bool> FileExists(string path)
        {
            try
            {
                return System.IO.File.Exists(path);
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// Retrieves an image from the specified file path.
        /// </summary>
        /// <param name="path">The file path of the image to retrieve.</param>
        /// <returns>
        /// If successful, returns the image file with the appropriate content type.
        /// If the image does not exist in the file system, returns a Not Found error.
        /// If an error occurs while reading the image, returns a 500 Internal Server Error with details.
        /// </returns>
        private async Task<IActionResult> GetImage(Image image)
        {
            try
            {
                byte[] imageBytes = System.IO.File.ReadAllBytes(image.Path);

                var product = await _context.Products.FirstOrDefaultAsync(p => p.Id == image.ProductId);
                if (product == null)
                {
                    _logger.Error($"Error occurred while retrieving product for image: {image.Path}");
                    return StatusCode(500, $"Error occurred while retrieving product for image: {image.Path}");
                }

                var configuration = await _context.WebConfigurations.FirstOrDefaultAsync(c => c.StoreId == product.StoreId);
                if (configuration == null)
                {
                    _logger.Error($"Error occurred while retrieving configuration for image: {image.Path}");
                    return StatusCode(500, $"Error occurred while retrieving configuration for image: {image.Path}");
                }

                return File(imageBytes, $"image/{configuration.StorageExtension}");
            }
            catch (Exception ex)
            {
                _logger.Error($"Error occurred while reading the image: {ex.Message}");
                return StatusCode(500, $"Error occurred while reading the image: {ex.Message}");
            }
        }
    }
}
