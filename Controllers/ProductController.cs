using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NLog;
using System.ComponentModel.DataAnnotations;
using WebApiCrawler.Data;
using WebApiCrawler.Models;
using Mapster; 

namespace WebApiCrawler.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class ProductController : ControllerBase
    {

        private readonly CrawlerDbContext _context;
        private readonly NLog.ILogger _logger;

        public ProductController(CrawlerDbContext dbContext, NLog.ILogger logger)
        {
            _context = dbContext;
            _logger= logger;
        }

        /// <summary>
        /// Adds a new product to the database
        /// </summary>
        /// <param name="productDto">The product data to add</param>
        /// <returns>The result of the operation</returns>
        [HttpPost("AddProduct")]
        public async Task<IActionResult> AddProduct(ProductDto productDto)
        {
            try
            {
                if (!await StoreExists(productDto.StoreId))
                {
                    _logger.Error($"Store with ID {productDto.StoreId} was not found.");
                    return BadRequest($"Store with ID {productDto.StoreId} was not found.");
                }

                if (await CodeExists(productDto.Code))
                {
                    _logger.Error($"Product with code {productDto.Code} already exist.");
                    return BadRequest($"Product with code {productDto.Code} already exist.");
                }

                var product = productDto.Adapt<Product>();
                _context.Products.Add(product);
                await _context.SaveChangesAsync();

                _logger.Info($"Product added: {product.Id}");
                return CreatedAtAction(nameof(AddProduct), product);
            }
            catch (Exception ex)
            {
                _logger.Error($"Error occurred while adding product: {ex.Message}");
                return StatusCode(500, $"Error occurred while adding product: {ex.Message}");
            }
        }

        /// <summary>
        /// Retrieves all products from the database
        /// </summary>
        /// <returns>
        /// IActionResult containing a list of products if found,
        /// otherwise returns a NotFound response indicating that no products were found
        /// </returns>
        [HttpGet("GetProducts")]
        public async Task<IActionResult> GetProducts()
        {
            var products = await _context.Products.ToListAsync();
            if (products.Count == 0)
            {
                _logger.Warn("No products were found in the database.");
                return NotFound("No products were found in the database.");
            }
            return Ok(products);
        }

        /// <summary>
        /// Retrieves a product with the specified ID from the database
        /// </summary>
        /// <param name="id">The ID of the product to retrieve</param>
        /// <returns>
        /// IActionResult containing the product if found,
        /// otherwise returns a BadRequest response indicating that the store was not found
        /// </returns>
        [HttpGet("GetProduct")]
        public async Task<IActionResult> GetProduct([Required] Guid id)
        {
            var product = await _context.Products.FindAsync(id);
            if (product == null)
            {
                _logger.Warn($"Product with ID {id} was not found.");
                return BadRequest($"Product with ID {id} was not found.");
            }
            return Ok(product);
        }

        /// <summary>
        /// Deletes a product from the database
        /// </summary>
        /// <param name="id">The ID of the product to delete</param>
        /// <returns>The result of the operation</returns>
        [HttpDelete("DeleteProduct")]
        public async Task<IActionResult> DeleteProduct([Required] Guid id)
        {
            var product = await _context.Products.FindAsync(id);
            if (product == null)
            {
                _logger.Warn($"Product with ID {id} was not found.");
                return NotFound($"Product with ID {id} was not found.");
            }

            try
            {
                _context.Products.Remove(product);
                await _context.SaveChangesAsync();
                _logger.Info($"Product with ID {id} has been deleted successfully.");
                return Ok($"Product with ID {id} has been deleted successfully.");
            }
            catch (Exception ex)
            {
                _logger.Error($"Error deleting image: {ex.Message}");
                return StatusCode(500, $"Error deleting image: {ex.Message}");
            }
        }


        /// <summary>
        /// Updates an existing product in the database
        /// </summary>
        /// <param name="id">The ID of the product to update</param>
        /// <param name="productDto">The product data to update</param>
        /// <returns>The result of the operation</returns>
        [HttpPut("UpdateProduct")]
        public async Task<IActionResult> UpdateProduct([Required] Guid id, ProductDto productDto)
        {
            try
            {
                var existingProduct = await _context.Products.FindAsync(id);
                if (existingProduct == null)
                {
                    _logger.Warn($"Product with ID {id} was not found.");
                    return NotFound($"Product with ID {id} was not found.");
                }

                if (!await StoreExists(productDto.StoreId))
                {
                    _logger.Warn($"Store with ID {productDto.StoreId} was not found.");
                    return NotFound($"Store with ID {productDto.StoreId} was not found.");
                }

                existingProduct.StoreId = productDto.StoreId;
                existingProduct.Code = productDto.Code;
                existingProduct.Name = productDto.Name;

                _context.Products.Update(existingProduct);
                await _context.SaveChangesAsync();


                _logger.Info($"Product updated: {existingProduct.Id}");
                return Ok(existingProduct);
            }
            catch (Exception ex)
            {
                _logger.Error($"Error occurred while updating product: {ex.Message}");
                return StatusCode(500, $"Error occurred while updating product: {ex.Message}");
            }
        }

        /// <summary>
        /// Checks if a product with the specified code already exists in the database
        /// </summary>
        /// <param name="code">The code of the product to check</param>
        /// <returns> True if the code is unique, otherwise false
        /// </returns>
        private async Task<bool> CodeExists(string code)
        {
            var product = await _context.Products.FirstOrDefaultAsync(p => p.Code == code);
            if (product == null)
            {
                return false;
            }
            return true;
        }

        /// <summary>
        /// Checks if a store exists in the database
        /// </summary>
        /// <param name="storeId">The ID of the store to check</param>
        /// <returns>True if the store exists, otherwise false</returns>
        private async Task<bool> StoreExists(Guid storeId)
        {
            var store = await _context.WebStores.FindAsync(storeId);
            if (store == null)
            {
                return false;
            }
            return true;
        }
    }
}
