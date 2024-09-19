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
    public class ConfigurationController : ControllerBase
    {
        private readonly CrawlerDbContext _context;
        private  readonly NLog.ILogger _logger;

        public ConfigurationController(CrawlerDbContext dbContext, NLog.ILogger logger)
        {
            _context = dbContext;
            _logger = logger;
        }

        /// <summary>
        /// Adds a new configuration to the database
        /// </summary>
        /// <param name="confDto">The configuration data to add</param>
        /// <returns>The result of the operation</returns>
        [HttpPost("AddConfiguration")]
        public async Task<IActionResult> AddConfiguration(WebConfigurationDto confDto)
        {
            try
            {
                if (!await StoreExists(confDto.StoreId))
                {
                    return BadRequest($"Store with ID {confDto.StoreId} was not found.");
                }

                var configuration = confDto.Adapt<WebConfiguration>();

                Microsoft.EntityFrameworkCore.ChangeTracking.EntityEntry<WebConfiguration> entityEntry = _context.WebConfigurations.Add(configuration);
                await _context.SaveChangesAsync();

                _logger.Info($"Configuration added: {configuration.Id}");
                return CreatedAtAction(nameof(AddConfiguration), configuration);
            }
            catch (Exception ex)
            {
                _logger.Error($"Error occurred while adding configuration: {ex.Message}");
                return StatusCode(500, $"Error occurred while adding configuration: {ex.Message}");
            }
        }

        /// <summary>
        /// Retrieves all configurations from the database
        /// </summary>
        /// <returns>
        /// IActionResult containing a list of configurations if found,
        /// otherwise returns a NotFound response indicating that no configurations were found
        /// </returns>
        [HttpGet("GetConfigurations")]
        public async Task<IActionResult> GetConfigurations()
        {
            var configurations = await _context.WebConfigurations.ToListAsync();
            if (configurations.Count == 0)
            {
                _logger.Warn("No configuratons were found in the database.");
                return NotFound("No configurations were found in the database.");
            }
            return Ok(configurations);
        }

        /// <summary>
        /// Retrieves a configuration with the specified ID from the database
        /// </summary>
        /// <param name="id">The ID of the configuration to retrieve</param>
        /// <returns>
        /// IActionResult containing the configuration if found,
        /// otherwise returns a BadRequest response indicating that the store was not found
        /// </returns>
        [HttpGet("GetConfiguration")]
        public async Task<IActionResult> GetConfiguration([Required] Guid id)
        {
            var webConfiguration = await _context.WebConfigurations.FindAsync(id);
            if (webConfiguration == null)
            {
                _logger.Warn($"Configuration with ID {id} was not found.");
                return BadRequest($"Configuration with ID {id} was not found.");
            }
            return Ok(webConfiguration);
        }

        /// <summary>
        /// Retrieves the web configuration associated with the specified store ID
        /// </summary>
        /// <param name="storeId">The unique identifier of the store</param>
        /// <returns>
        /// If successful, returns an OkObjectResult containing the web configuration for the specified store ID.
        /// If no configuration is found for the specified store ID, returns a BadRequestObjectResult
        /// </returns>
        [HttpGet("GetStoreConfiguration")]
        public async Task<IActionResult> GetStoreConfiguration([Required] Guid storeId)
        {
            var webConfiguration = await _context.WebConfigurations.Where(c=> c.StoreId == storeId).FirstOrDefaultAsync();
            if (webConfiguration == null)
            {
                _logger.Warn($"Configuration for store with ID {storeId} was not found.");
                return BadRequest($"Configuration for store with ID {storeId} was not found.");
            }
            return Ok(webConfiguration);
        }

        /// <summary>
        /// Deletes a configuration from the database
        /// </summary>
        /// <param name="id">The ID of the configuration to delete</param>
        /// <returns>The result of the operation</returns>
        [HttpDelete("DeleteConfiguration")]
        public async Task<IActionResult> DeleteConfiguration([Required] Guid id)
        {
            var configuration = await _context.WebConfigurations.FindAsync(id);
            if (configuration == null)
            {
                _logger.Warn($"Configuration with ID {id} was not found.");
                return NotFound($"Configuration with ID {id} was not found.");
            }

            try
            {
                _context.WebConfigurations.Remove(configuration);
                await _context.SaveChangesAsync();
                _logger.Info($"Configuration with ID {id} has been deleted successfully.");
                return Ok($"Configuration with ID {id} has been deleted successfully.");
            }
            catch (Exception ex)
            {
                _logger.Error($"Error deleting configuration: {ex.Message}");
                return StatusCode(500, $"Error deleting configuration: {ex.Message}");
            }
        }

        /// <summary>
        /// Updates an existing configuration in the database
        /// </summary>
        /// <param name="id">The ID of the configuration to update</param>
        /// <param name="confDto">The configuration data to update</param>
        /// <returns>The result of the operation</returns>
        [HttpPut("UpdateConfiguration")]
        public async Task<IActionResult> UpdateConfiguration([Required] Guid id, WebConfigurationDto confDto)
        {
            try
            {
                var existingConfiguration = await _context.WebConfigurations.FindAsync(id);
                if (existingConfiguration == null)
                {
                    _logger.Warn($"Configuration with ID {id} was not found.");
                    return NotFound($"Configuration with ID {id} was not found.");
                }

                if (!await StoreExists(confDto.StoreId))
                {
                    _logger.Warn($"Store with ID {confDto.StoreId} was not found.");
                    return NotFound($"Store with ID {confDto.StoreId} was not found.");
                }

                existingConfiguration.UrlPage = confDto.UrlPage;
                existingConfiguration.ImgTag = confDto.ImgTag;
                existingConfiguration.Img = confDto.Img;
                existingConfiguration.StorageExtension = confDto.StorageExtension;
                existingConfiguration.StoreId = confDto.StoreId;

                _context.WebConfigurations.Update(existingConfiguration);
                await _context.SaveChangesAsync();

                _logger.Info($"Configuration updated: {existingConfiguration.Id}");
                return Ok(existingConfiguration);
            }
            catch (Exception ex)
            {
                _logger.Error($"Error occurred while updating configuration: {ex.Message}");
                return StatusCode(500, $"Error occurred while updating configuration: {ex.Message}");
            }
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
