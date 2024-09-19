using Mapster;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NLog;
using WebApiCrawler.Data;
using WebApiCrawler.Models;

namespace WebApiCrawler.Services
{
    public class ImageService : IImageService
    {
        private readonly CrawlerDbContext _context;
        private static readonly NLog.ILogger logger = LogManager.GetCurrentClassLogger();

        public ImageService(CrawlerDbContext dbContext)
        {
            _context = dbContext;
        }

        /// <summary>
        /// Adds a new image to the database
        /// </summary>
        /// <param name="imageDto">The data transfer object representing the image to be added</param>
        /// <returns>Result of the operation</returns>
        public async Task<IActionResult> AddNewImage(ImageDto imageDto)
        {
            try
            {
                if (!await ProductExists(imageDto.ProductId))
                {
                    logger.Error($"Product with ID {imageDto.ProductId} was not found.");
                   return new BadRequestObjectResult($"Product with ID {imageDto.ProductId} was not found.");
                }
               
                var image = imageDto.Adapt<Image>();

                _context.Images.Add(image);
                await _context.SaveChangesAsync();

                logger.Info($"Image added: {image.Id}");
                return new OkObjectResult(image);

            }
            catch (Exception ex)
            {
                logger.Error($"Error occurred while adding image: {ex.Message}");
                return new StatusCodeResult(500);
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
        /// Checks if an image already exists in the database based on its path
        /// </summary>
        /// <param name="imageDto">The image data transfer object containing the path of the image to check</param>
        /// <returns>Returns true if the image already exists in the database; otherwise, returns false</returns>
        public async Task<bool> ImageExists(ImageDto imageDto)
        {
            var existingImage = await _context.Images.AnyAsync(i => i.Path.Equals(imageDto.Path));
            return existingImage;
        }
    }
}
