using Mapster;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.StaticFiles;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.IO.Compression;
using System.Net;
using System.Text;
using WebApiCrawler.Data;
using System.Net.Http;
using System.IO;
using WebApiCrawler.Models;
using Microsoft.Extensions.Configuration;
using System;
using Microsoft.Extensions.Logging;
using System.Xml;
using HtmlAgilityPack;
using Microsoft.IdentityModel.Tokens;
using NLog;
using WebApiCrawler.Services;
using static System.Net.WebRequestMethods;
using WebApiCrawler.AbrakadabraModels;
using System.Text.Json;

namespace WebApiCrawler.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class CrawlerController : ControllerBase
    {
        private readonly CrawlerDbContext _context;
        private readonly NLog.ILogger _logger;

        public CrawlerController(CrawlerDbContext dbContext, NLog.ILogger logger)
        {
            _context = dbContext;
            _logger = logger; 
        }

        /// <summary>
        /// Downloads images for products associated with a specific web store
        /// </summary>
        /// <param name="webStoreId">The unique identifier of the web store.</param>
        /// <returns>The result of the image download operation</returns>
        [HttpPost("DownloadImages")]
        public async Task<IActionResult> DownloadImages([Required] Guid webStoreId)
        {
            try
            {
                var webStore = await _context.WebStores.FindAsync(webStoreId);
                if (webStore == null)
                {
                    _logger.Error($"Store with ID {webStoreId} was not found.");
                    return BadRequest($"Store with ID {webStoreId} was not found.");
                }

                var configuration = await _context.WebConfigurations.FirstOrDefaultAsync(c => c.StoreId == webStoreId);
                if (configuration == null)
                {
                    _logger.Error($"Configuration for store with ID {webStoreId} was not found.");
                    return BadRequest($"Configuration for store with ID {webStoreId} was not found.");
                }

                if (CheckSettings(configuration))
                {
                    _logger.Error($"Invalid configuration settings for store with ID {configuration.StoreId}.");
                    return BadRequest($"Invalid configuration settings for store with ID {configuration.StoreId}.");
                }

                var products = await _context.Products.Where(p => p.StoreId == webStoreId).ToListAsync();
                if (!products.Any())
                {
                    _logger.Warn($"No products found for store with ID {webStoreId}.");
                    return NotFound($"No products found for store with ID {webStoreId}.");
                }

                string path = Path.GetFullPath(configuration.StorageFolder);

                ICrawler crawler = new Crawler();
                if (!await crawler.CreateDowloadFolder(path))
                {
                    return StatusCode(500, $"Error occurred while creating download directory"); 
                }
                IImageService imageService = new ImageService(_context);
                foreach (var product in products)
                {
                    ImageDto imageDto =await crawler.DownloadProductImage(product,configuration);
                    bool imageExists = await imageService.ImageExists(imageDto);

                    if (imageDto != null && !imageExists)
                    {
                        await imageService.AddNewImage(imageDto);
                    }
                }
                _logger.Info("Download completed!");
                return Ok("Download completed!");
            }
            catch (Exception ex)
            {
                _logger.Error ($"Error occurred while downloading images: {ex.Message}");
                return StatusCode(500, $"Error occurred while downloading images: {ex.Message}");
            }
        }

        /// <summary>
        /// Checks if the provided web configuration settings are valid.
        /// </summary>
        /// <param name="configuration">The web configuration to check.</param>
        /// <returns>True if the configuration settings are valid; otherwise, false.</returns>
        private static bool CheckSettings(WebConfiguration configuration)
        {
            return configuration.ImgTag.IsNullOrEmpty() || configuration.Img.IsNullOrEmpty()
                || configuration.UrlPage.IsNullOrEmpty() || configuration.StorageExtension.IsNullOrEmpty()
                || configuration.StorageFolder.IsNullOrEmpty(); 
        }
    }
}
