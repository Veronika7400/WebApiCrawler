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
    public class StoreController : ControllerBase
    {

        private readonly CrawlerDbContext _context;
        private readonly NLog.ILogger _logger;

        public StoreController(CrawlerDbContext dbContext, NLog.ILogger logger)
        {
            _context = dbContext;
            _logger = logger;
        }


        /// <summary>
        /// Adds a new store to the database
        /// </summary>
        /// <param name="webStoreDto">The store data to add</param>
        /// <returns></returns>
        [HttpPost("AddStore")]
        public async Task<IActionResult> AddStore(WebStoreDto webStoreDto)
        {
            var webStore = webStoreDto.Adapt<WebStore>();

            try
            {
                if (!await UniqueName(webStoreDto.Name))
                {
                    return BadRequest($"Store with name {webStoreDto.Name} already exist.");
                }

                _context.WebStores.Add(webStore);
                await _context.SaveChangesAsync();

                _logger.Info($"Store added: {webStore.Id}");
                return CreatedAtAction(nameof(AddStore), webStore);
            }
            catch (Exception ex)
            {
                _logger.Error($"Error occurred while adding store: {ex.Message}");
                return StatusCode(500, $"Error occurred while adding store: {ex.Message}");
            }
        }

        /// <summary>
        /// Retrieves all stores from the database
        /// </summary>
        /// <returns>
        /// IActionResult containing a list of stores if found,
        /// otherwise returns a NotFound response indicating that no stores were found
        /// </returns>
        [HttpGet("GetStores")]
        public async Task<IActionResult> GetStores()
        {
            var stores = await _context.WebStores.ToListAsync();
            if (stores.Count == 0)
            {
                _logger.Warn("No stores were found in the database.");
                return NotFound("No stores were found in the database.");
            }
            return Ok(stores);
        }

        /// <summary>
        /// Retrieves a store with the specified ID from the database
        /// </summary>
        /// <param name="id">The ID of the store to retrieve</param>
        /// <returns>
        /// IActionResult containing the store if found,
        /// otherwise returns a BadRequest response indicating that the store was not found
        /// </returns>
        [HttpGet("GetStore")]
        public async Task<IActionResult> GetStore([Required] Guid id)
        {
            var store = await _context.WebStores.FindAsync(id);
            if (store == null)
            {
                _logger.Warn($"Store with ID {id} was not found.");
                return BadRequest($"Store with ID {id} was not found.");
            }
            return Ok(store);
        }

        /// <summary>
        /// Deletes a store from the database
        /// </summary>
        /// <param name="id">The ID of the store to delete</param>
        /// <returns>The result of the operation</returns>
        [HttpDelete("DeleteStore")]
        public async Task<IActionResult> DeleteStore([Required] Guid id)
        {
            var store = await _context.WebStores.FindAsync(id);
            if (store == null)
            {
                _logger.Warn($"Store with ID {id} was not found.");
                return NotFound($"Store with ID {id} was not found.");
            }

            try
            {
                _context.WebStores.Remove(store);
                await _context.SaveChangesAsync();
                _logger.Info($"Store with ID {id} has been deleted successfully.");
                return Ok($"Store with ID {id} has been deleted successfully.");
            }
            catch (Exception ex)
            {
                _logger.Error($"Error deleting store: {ex.Message}");
                return StatusCode(500, $"Error deleting store: {ex.Message}");
            }
        }

        /// <summary>
        /// Updates an existing store in the database
        /// </summary>
        /// <param name="id">The ID of the store to update</param>
        /// <param name="webStoreDto">The store data to update</param>
        /// <returns>The result of the operation</returns>
        [HttpPut("UpdateStore")]
        public async Task<IActionResult> UpdateStore([Required] Guid id, WebStoreDto webStoreDto)
        {
            try
            {
                var existingStore = await _context.WebStores.FindAsync(id);
                if (existingStore == null)
                {
                    _logger.Warn($"Store with ID {id} was not found.");
                    return NotFound($"Store with ID {id} was not found.");
                }

                if (!await UniqueName(webStoreDto.Name))
                {
                    _logger.Error($"Store with name {webStoreDto.Name} already exist.");
                    return BadRequest($"Store with name {webStoreDto.Name} already exist.");
                }

                existingStore.Name = webStoreDto.Name;

                _context.WebStores.Update(existingStore);
                await _context.SaveChangesAsync();

                _logger.Info($"Store updated: {existingStore.Id}");
                return Ok(existingStore);
            }
            catch (Exception ex)
            {
                _logger.Error($"Error occurred while updating store: {ex.Message}");
                return StatusCode(500, $"Error occurred while updating store: {ex.Message}");
            }
        }

        /// <summary>
        /// Checks if a store with the specified name already exists in the database
        /// </summary>
        /// <param name="name">The name of the store to check</param>
        /// <returns>True if the store name is unique, otherwise false</returns>
        private async Task<bool> UniqueName(string name)
        {
            var store = await _context.WebStores.FirstOrDefaultAsync(s => s.Name == name);
            if (store == null)
            {
                return true;
            }
            return false;
        }
    }
}
