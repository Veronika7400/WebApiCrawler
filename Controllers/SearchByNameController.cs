using Mapster;
using MapsterMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.IdentityModel.Tokens;
using System;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics;
using System.Reflection;
using System.Text.Json;
using WebApiCrawler.AbrakadabraModels;
using WebApiCrawler.Data;
using WebApiCrawler.ModelsHGSpot;
using WebApiCrawler.SearchModels;
using WebApiCrawler.Services;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace WebApiCrawler.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SearchByNameController : ControllerBase
    {
        private readonly CrawlerDbContext _context;
        private readonly NLog.ILogger _logger;
        private IPriceService ps; 

        public SearchByNameController(CrawlerDbContext dbContext, NLog.ILogger logger)
        {
            _context = dbContext;
            _logger = logger;
        }

        /// <summary>
        /// Retrieves the lowest price for the given search word from all stores
        /// </summary>
        /// <param name="searchWord">The search word</param>
        /// <returns>An IActionResult representing the lowest price from all stores</returns>
        [HttpGet("GetLowestPriceFromAllStores")]
        public async Task<IActionResult> GetLowestPriceFromAllStores(string searchWord, int page = 1, int pageSize = 10)
        {
            ps = new PriceService(_context, _logger);
            IActionResult productsResult = await ps.GetLowestPriceInEachStore(searchWord, page, pageSize);
            if (productsResult is OkObjectResult)
            {
                var products = ((OkObjectResult)productsResult).Value;
                if (products is IEnumerable<LowestPriceResult> productList)
                {
                    LowestPriceResult minPriceProduct = productList.OrderBy(product => product.PriceValue).FirstOrDefault();
                    return Ok(minPriceProduct);
                }
            }
            return productsResult; 
        }

        /// <summary>
        /// Retrieves the lowest price for the specified search word from each store
        /// </summary>
        /// <param name="searchWord">The search word</param>
        /// <returns>An asynchronous task that returns an IActionResult representing the lowest price from each store</returns>

        [HttpGet("GetLowestPriceInEachStore")]
        public async Task<IActionResult> GetLowestPriceInEachStore(string searchWord, int page = 1, int pageSize = 10)
        {
            ps = new PriceService(_context, _logger);
            return await ps.GetLowestPriceInEachStore(searchWord, page, pageSize); 
        }

        /// <summary>
        ///  Retrieves the lowest price for the specified search word from each store, without applying pagination.
        /// </summary>
        /// <param name="searchWord">The search word</param>
        /// <returns>An asynchronous task that returns an IActionResult representing the lowest price from each store</returns>
        [HttpGet("GetLowestPriceInEachStoreWithoutPagination")]
        public async Task<IActionResult> GetLowestPriceInEachStoreWithoutPagination(string searchWord)
        {
            ps = new PriceService(_context, _logger);
            return await ps.GetLowestPriceInEachStoreWithoutPagination(searchWord);
        }

        /// <summary>
        /// Retrieves the lowest price for the specified search word from a specific store
        /// </summary>
        /// <param name="searchWord">The search word</param>
        /// <param name="guid">The unique identifier of the store</param>
        /// <returns>An asynchronous task that returns an IActionResult representing the lowest price from the specified store</returns>

        [HttpGet("GetLowestPriceFromStore")]
        public async Task<IActionResult> GetLowestPrice( string searchWord, [Required] Guid guid)
        {
            ps = new PriceService(_context, _logger);
            IActionResult pricesResult = await ps.GetPricesFromUrl(searchWord, guid);
            return await ps.GetLowestPrice(pricesResult);
        }

        /// <summary>
        /// Retrieves the prices from a specific store for the specified search word
        /// </summary>
        /// <param name="searchWord">The search word</param>
        /// <param name="guid">The unique identifier of the store</param>
        /// <returns>An asynchronous task that returns an IActionResult representing the prices from the specified store.</returns>

        [HttpGet("GetPricesFromStore")]
        public async Task<IActionResult> GetPricesFromStore(string searchWord, [Required] Guid guid)
        {
            ps = new PriceService(_context, _logger); 
            var response = await ps.GetPricesFromUrl(searchWord, guid);
            return response;  
        }

        /// <summary>
        /// Retrieves collection of LowestPriceResult objects representig products from a specific store based on the search word and store ID
        /// </summary>
        /// <param name="searchWord">The search word used to query the products</param>
        /// <param name="guid">The unique identifier of the store</param>
        /// <returns>
        /// Returns the lowest prices for products from the specific store
        /// </returns>
        [HttpGet("GetPricesFromStoreLowestPriceResult")]
        public async Task<IActionResult> GetPricesFromStoreLowestPriceResult(string searchWord, [Required] Guid guid, int page = 1, int pageSize = 10)
        {
            ps = new PriceService(_context, _logger);
            var response = await ps.GetPricesFromUrlLowestPriceResult(searchWord, guid, page, pageSize);
            return response;
        }

        /// <summary>
        /// Retrieves the lowest price results for a given search word from a store, without applying pagination.
        /// </summary>
        /// <param name="searchWord">The search term to use for finding prices.</param>
        /// <param name="guid">The unique identifier for the store or product for which to retrieve prices.</param>
        /// <returns>An IActionResult containing the results of the price retrieval operation.</returns>
        [HttpGet("GetPricesFromStoreLowestPriceResultWithoutPagination")]
        public async Task<IActionResult> GetPricesFromStoreLowestPriceResultWithoutPagination(string searchWord, [Required] Guid guid)
        {
            ps = new PriceService(_context, _logger);
            var response = await ps.GetPricesFromUrlLowestPriceResultWithoutPagination(searchWord, guid);
            return response;
        }
    }
}
