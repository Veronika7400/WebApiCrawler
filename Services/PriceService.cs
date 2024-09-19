using HtmlAgilityPack;
using Mapster;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.Reflection.Metadata;
using System;
using System.Text.Json;
using WebApiCrawler.Data;
using WebApiCrawler.SearchModels;

namespace WebApiCrawler.Services
{
    public class PriceService : IPriceService
    {
        private readonly CrawlerDbContext _context;
        private readonly NLog.ILogger _logger;
        public PriceService(CrawlerDbContext dbContext, NLog.ILogger logger)
        {
            _context = dbContext;
            _logger = logger;
        }

        /// <summary>
        /// Retrieves the lowest price for the given search word in each store with pagination
        /// </summary>
        /// <param name="searchWord">The search word to be used</param>
        /// <param name="page">The page number to retrieve</param>
        /// <param name="pageSize">The number of products per page</param>
        /// <returns>An IActionResult representing the lowest prices in each store</returns>
        public async Task<IActionResult> GetLowestPriceInEachStore(string searchWord, int page = 1, int pageSize = 10)
        {
            var stores = _context.WebStores.ToList();
            if (!stores.Any())
            {
                _logger.Error("No stores found in database.");
                return new BadRequestObjectResult("No stores found in database.");
            }

            var webConfigurations = _context.WebConfigurations.ToList();

            List<LowestPriceResult> products = new List<LowestPriceResult>();
            foreach (var store in stores)
            {
                if (ValidConfiguration(store.Id))
                {
                    IActionResult pricesResult = await GetPricesFromUrl(searchWord, store.Id);
                    IActionResult lowestPrice = await GetLowestPrice(pricesResult);
                    var conf = webConfigurations.FirstOrDefault(c => c.StoreId == store.Id);
                    if (lowestPrice is OkObjectResult)
                    {
                        var product = ((OkObjectResult)lowestPrice).Value;
                        if (product is IProductDto)
                        {
                            var productDto = ((IProductDto)product);
                            products.Add(new LowestPriceResult
                            {
                                StoreName = store.Name,
                                ProductName = productDto.ProductName,
                                PriceValue = productDto.PriceValue,
                                Url = conf.UrlPage + productDto.Link,
                                ImageUrl = null
                            });
                        }
                    }
                }
            }

            if (products.Count > 0)
            {
                var paginatedProducts = products
                    .Skip((page - 1) * pageSize)
                    .Take(pageSize)
                    .ToList();

                var crawler = new Crawler();
                var fetchImageTasks = paginatedProducts.Select(async product =>
                {
                    var conf = webConfigurations.FirstOrDefault(c => c.StoreId == stores.First(s => s.Name == product.StoreName).Id);
                    if (conf != null)
                    {
                        product.ImageUrl = await crawler.GetFirstImageUrlFromPage(product.Url, conf);
                    }
                });

                await Task.WhenAll(fetchImageTasks);

                _logger.Info("Download prices.");
                return new OkObjectResult(paginatedProducts);
            }

            _logger.Error("No products found in any store.");
            return new BadRequestObjectResult("No products found in any store.");
        }

        
        /// <summary>
        /// Retrieves the lowest price for the given search word in each store
        /// </summary>
        /// <param name="searchWord">The search word to be used</param>
        /// <returns>An IActionResult representing the lowest prices in each store</returns>
        public async Task<IActionResult> GetLowestPriceInEachStoreWithoutPagination(string searchWord)
        {
            var stores = _context.WebStores.ToList();
            if (!stores.Any())
            {
                _logger.Error("No stores found in database.");
                return new BadRequestObjectResult("No stores found in database.");
            }
            List<LowestPriceResult> products = new List<LowestPriceResult>();
            foreach (var store in stores)
            {
                if (ValidConfiguration(store.Id))
                {
                    IActionResult pricesResult = await GetPricesFromUrl(searchWord, store.Id);
                    IActionResult lowestPrice = await GetLowestPrice(pricesResult);
                    var conf = _context.WebConfigurations.Where(c => c.StoreId == store.Id).FirstOrDefault();
                    if (lowestPrice is OkObjectResult)
                    {
                        Crawler crawler = new Crawler();
                        var product = ((OkObjectResult)lowestPrice).Value;
                        if (product is IProductDto)
                        {
                            var productDto = ((IProductDto)product);
                            products.Add(new LowestPriceResult
                            {
                                StoreName = store.Name,
                                ProductName = productDto.ProductName,
                                PriceValue = productDto.PriceValue,
                                Url = conf.UrlPage + productDto.Link,
                                ImageUrl = await crawler.GetFirstImageUrlFromPage(conf.UrlPage + productDto.Link, conf)
                            });
                        }
                    }
                }
            }
            if (products.Count > 0)
            {
                _logger.Info("Download prices.");
                return new OkObjectResult(products);
            }
            _logger.Error("No products found in any store.");
            return new BadRequestObjectResult("No products found in any store.");
        }

        /// <summary>
        /// Gets the lowest price from the collection of the prices
        /// </summary>
        /// <param name="pricesResult">The collection of prices</param>
        /// <returns>An IActionResult representing the lowest price</returns>
        public async Task<IActionResult> GetLowestPrice(IActionResult pricesResult)
        {
            if (pricesResult is OkObjectResult)
            {
                var prices = ((OkObjectResult)pricesResult).Value;
                if (prices is ISearchResultDto)
                {
                    ISearchResultDto searchResult = (ISearchResultDto)prices;
                    IProductDto lowestPrice = FindLowestPrice(searchResult);
                    if (lowestPrice == null)
                    {
                        _logger.Error("Unable to find product price");
                        return new BadRequestObjectResult("Unable to find product price");
                    }
                    _logger.Info($"Lowest price is {lowestPrice.PriceValue}");
                    return new OkObjectResult(lowestPrice);
                }
                else
                {
                    _logger.Error($"Unexpected result type: {prices.GetType().Name}");
                    return new BadRequestObjectResult("Unexpected result type");
                }
            }
            else
            {
                _logger.Error("Unable to reach products");
                return new BadRequestObjectResult(((BadRequestObjectResult)pricesResult).Value);
            }
        }

        /// <summary>
        /// Retrieves prices from the provided url
        /// </summary>
        /// <param name="searchWord">The search word to be used</param>
        /// <param name="guid">The GUID of the store</param>
        /// <returns>An IActionResult representing the retrieved prices</returns>
        public async Task<IActionResult> GetPricesFromUrl(string searchWord, Guid guid)
        {
            string storeUrl = GetStoreSearchUrl(guid);
            if (storeUrl == null)
            {
                return new BadRequestObjectResult($"No search box url for store: {guid}");
            }
            string url = storeUrl + searchWord;

            using (HttpClient client = new HttpClient())
            {
                try
                {
                    HttpResponseMessage response = await client.GetAsync(url);

                    if (response.IsSuccessStatusCode)
                    {
                        string content = await response.Content.ReadAsStringAsync();

                        string resultType = GetResultType(guid);
                        string displayType = GetDisplayType(guid);
                        if (resultType.IsNullOrEmpty() || displayType.IsNullOrEmpty())
                        {
                            _logger.Error($"Invalid configuration for store: {guid}");
                            return new BadRequestObjectResult($"Invalid configuration for store: {guid}");
                        }

                        Type searchResultType = Type.GetType(resultType);
                        Type displayResultType = Type.GetType(displayType);
                        if (searchResultType == null || displayResultType == null)
                        {
                            _logger.Error($"Type '{resultType}' or '{displayType}'  not found.");
                            return new BadRequestObjectResult($"Type '{resultType}' or '{displayType}'  not found.");
                        }

                        var deserializedResult = JsonSerializer.Deserialize(content, searchResultType);
                        if (deserializedResult == null)
                        {
                            _logger.Error($"Deserialization failed for type '{resultType}'.");
                            return new BadRequestObjectResult($"Deserialization failed for type '{resultType}'.");
                        }

                        ISearchResultDto result = (ISearchResultDto)TypeAdapter.Adapt(deserializedResult, searchResultType, displayResultType);
                        if (!result.products.Any())
                        {
                            _logger.Info($"No products for search word: {searchWord}");
                            return new BadRequestObjectResult($"No products for search word: {searchWord}");
                        }
                   
                        _logger.Info("Download prices.");
                        return new OkObjectResult(result);
                    }
                    else
                    {
                        _logger.Error($"Failed to retrieve products. Status code: {response.StatusCode}");
                        return new BadRequestObjectResult($"Failed to retrieve products. Status code: {response.StatusCode}");
                    }
                }
                catch (Exception ex)
                {
                    _logger.Error($"An error occurred while retrieving products: {ex.Message}");
                    return new BadRequestObjectResult($"An error occurred while retrieving products: {ex.Message} {ex.StackTrace}");
                }
            }
        }

        /// <summary>
        /// Retrieves the collection of LowestPriceResult objects for products from a specified URL
        /// </summary>
        /// <param name="searchWord">The search word used to find products</param>
        /// <param name="guid">The unique identifier of the store configuration</param>
        /// <param name="page">The page number to retrieve</param>
        /// <param name="pageSize">The number of products per page</param>
        /// <returns>If successful, returns an OkObjectResult with a collection of LowestPriceResult objects. If an error occurs during the retrieval process, returns a BadRequestObjectResult with an error message</returns>
        public async Task<IActionResult> GetPricesFromUrlLowestPriceResult(string searchWord, Guid guid, int page = 1, int pageSize = 10)
        {
            var conf = _context.WebConfigurations.FirstOrDefault(c => c.StoreId == guid);
            var store = _context.WebStores.FirstOrDefault(c => c.Id == guid);
            if (conf == null || store == null)
            {
                return new BadRequestObjectResult("Store and web configuration are not valid.");
            }

            IActionResult response = await GetPricesFromUrl(searchWord, guid);
            if (response is OkObjectResult)
            {
                var product = (response as OkObjectResult).Value;
                if (product is ISearchResultDto productsDto)
                {
                    var paginatedProductsDto = productsDto.products
                        .Skip((page - 1) * pageSize)
                        .Take(pageSize)
                        .ToList();

                    var crawler = new Crawler();
                    var imageTasks = paginatedProductsDto.Select(async productDto =>
                    {
                        var imageUrl = await crawler.GetFirstImageUrlFromPage(conf.UrlPage + productDto.Link, conf);
                        return new LowestPriceResult
                        {
                            StoreName = store.Name,
                            ProductName = productDto.ProductName,
                            PriceValue = productDto.PriceValue,
                            Url = conf.UrlPage + productDto.Link,
                            ImageUrl = imageUrl
                        };
                    });

                    var products = await Task.WhenAll(imageTasks);
                    _logger.Info("Downloaded prices.");
                    return new OkObjectResult(products);
                }
                else
                {
                    _logger.Error("Failed to retrieve products.");
                    return new BadRequestObjectResult("Failed to retrieve products.");
                }
            }
            else
            {
                _logger.Error("An error occurred while retrieving products.");
                return new BadRequestObjectResult("An error occurred while retrieving products.");
            }
        }

        /// <summary>
        /// Retrieves the lowest price results for a given search term from a specific store without applying pagination.
        /// </summary>
        /// <param name="searchWord">The search term to use when querying for prices.</param>
        /// <param name="guid">The unique identifier for the store from which to retrieve prices.</param>
        /// <returns>
        /// An IActionResult representing the result of the price retrieval operation. If successful, returns a 200 OK response with a list of lowest price results. 
        /// If there are issues, such as invalid store or configuration, or if an error occurs during retrieval, returns a 400 Bad Request response with an appropriate error message.
        /// </returns>
        public async Task<IActionResult> GetPricesFromUrlLowestPriceResultWithoutPagination(string searchWord, Guid guid)
        {
            var conf = _context.WebConfigurations.FirstOrDefault(c => c.StoreId == guid);
            var store = _context.WebStores.FirstOrDefault(c => c.Id == guid);
            if (conf == null || store == null)
            {
                return new BadRequestObjectResult("Store and web configuration are not valid.");
            }

            IActionResult response = await GetPricesFromUrl(searchWord, guid);
            if (response is OkObjectResult)
            {
                var product = (response as OkObjectResult).Value;
                if (product is ISearchResultDto productsDto)
                {
                    var finalProductsDto = productsDto.products.ToList();

                    var crawler = new Crawler();
                    var imageTasks = finalProductsDto.Select(async productDto =>
                    {
                        var imageUrl = await crawler.GetFirstImageUrlFromPage(conf.UrlPage + productDto.Link, conf);
                        return new LowestPriceResult
                        {
                            StoreName = store.Name,
                            ProductName = productDto.ProductName,
                            PriceValue = productDto.PriceValue,
                            Url = conf.UrlPage + productDto.Link,
                            ImageUrl = imageUrl
                        };
                    });

                    var products = await Task.WhenAll(imageTasks);
                    _logger.Info("Downloaded prices.");
                    return new OkObjectResult(products);
                }
                else
                {
                    _logger.Error("Failed to retrieve products.");
                    return new BadRequestObjectResult("Failed to retrieve products.");
                }
            }
            else
            {
                _logger.Error("An error occurred while retrieving products.");
                return new BadRequestObjectResult("An error occurred while retrieving products.");
            }
        }

        /*
        /// <summary>
        /// Retrieves the collection of LowestPriceResult objects for products from a specified URL
        /// </summary>
        /// <param name="searchWord">The search word used to find products</param>
        /// <param name="guid">The unique identifier of the store configuration</param>
        /// <returns> If successful, returns an OkObjectResult with a collection of LowestPriceResult objects.
        /// If an error occurs during the retrieval process, returns a BadRequestObjectResult with an error message
        /// </returns>
        public async Task<IActionResult> GetPricesFromUrlLowestPriceResult(string searchWord, Guid guid, int page = 1, int pageSize = 10)
        {
            var conf = _context.WebConfigurations.FirstOrDefault(c => c.StoreId == guid);
            var store = _context.WebStores.FirstOrDefault(c => c.Id == guid);
            if (conf == null || store == null)
            {
                return new BadRequestObjectResult("Store and web configuration are not valid.");
            }

            IActionResult response = await GetPricesFromUrl(searchWord, guid);
            if (response is OkObjectResult)
            {
                var product = (response as OkObjectResult).Value;
                if (product is ISearchResultDto productsDto)
                {
                    var crawler = new Crawler();
                    var imageTasks = productsDto.products.Select(async productDto =>
                    {
                        var imageUrl = await crawler.GetFirstImageUrlFromPage(conf.UrlPage + productDto.Link, conf);
                        return new LowestPriceResult
                        {
                            StoreName = store.Name,
                            ProductName = productDto.ProductName,
                            PriceValue = productDto.PriceValue,
                            Url = conf.UrlPage + productDto.Link,
                            ImageUrl = imageUrl
                        };
                    });

                    var products = await Task.WhenAll(imageTasks);
                    _logger.Info("Downloaded prices.");
                    return new OkObjectResult(products);
                }
                else
                {
                    _logger.Error("Failed to retrieve products.");
                    return new BadRequestObjectResult("Failed to retrieve products.");
                }
            }
            else
            {
                _logger.Error("An error occurred while retrieving products.");
                return new BadRequestObjectResult("An error occurred while retrieving products.");
            }
        }
        */

        /// <summary>
        /// Retrieves the display type for the specified store ID
        /// </summary>
        /// <param name="guid">The unique identifier of the store</param>
        /// <returns>The display type associated with the specified store ID, or null if not found</returns>
        private string GetDisplayType(Guid guid)
        {
            try
            {
                var conf = _context.WebConfigurations.Where(s => s.StoreId == guid).FirstOrDefault();
                if (conf != null)
                {
                    return conf.DisplayType;
                }
                return null;
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        /// <summary>
        /// Retrieves the response type for the specified store ID
        /// </summary>
        /// <param name="guid">The unique identifier of the store</param>
        /// <returns>The response type associated with the specified store ID, or null if not found</returns>
        private string GetResultType(Guid guid)
        {
            try
            {
                var conf = _context.WebConfigurations.Where(s => s.StoreId == guid).FirstOrDefault();
                if (conf != null)
                {
                    return conf.ResultType;
                }
                return null;
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        /// <summary>
        /// Retrieves the search URL for the specified store ID
        /// </summary>
        /// <param name="guid">The unique identifier of the store</param>
        /// <returns>The search URL associated with the specified store ID, or null if not found</returns>
        private string GetStoreSearchUrl(Guid guid)
        {
            try
            {
                var conf = _context.WebConfigurations.Where(s => s.StoreId == guid).FirstOrDefault();
                if(conf != null)
                {
                    return conf.SearchBox;
                }
                return null;
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        /// <summary>
        /// Finds the product with the lowest price from the list of products
        /// </summary>
        /// <param name="searchResult">The search result containing the list of products</param>
        /// <returns>The product with the lowest price if it exists, otherwise null</returns>

        private IProductDto FindLowestPrice(ISearchResultDto searchResult)
        {

            if (searchResult.products == null || !searchResult.products.Any())
            {
                return null;
            }

            var minPriceProduct = searchResult.products.OrderBy(product => product.PriceValue).FirstOrDefault();

            return minPriceProduct;
        }

        /// <summary>
        /// Check if configuration is valid for a given store
        /// </summary>
        /// <param name="guid">The unique identifier of the store</param>
        /// <returns>True if the configuration is valid, otherwise false</returns>
        private bool ValidConfiguration(Guid guid)
        {
            var conf = _context.WebConfigurations.Where(s => s.StoreId == guid).FirstOrDefault();
            if (conf != null && !conf.SearchBox.IsNullOrEmpty() && !conf.ResultType.IsNullOrEmpty() && !conf.DisplayType.IsNullOrEmpty())
            {
                return true;
            }
            return false;
        }
    }
}
