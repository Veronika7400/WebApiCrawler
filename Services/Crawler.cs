using HtmlAgilityPack;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using NLog;
using System.IO;
using System.Net;
using WebApiCrawler.Models;

namespace WebApiCrawler.Services
{
    public class Crawler : ICrawler
    {
        private static readonly NLog.ILogger logger = LogManager.GetCurrentClassLogger();


        /// <summary>
        /// Creates a directory at the specified file path if it does not already exist
        /// </summary>
        /// <param name="filePath">The path of the directory to create</param>
        /// <returns>True if the directory was created successfully or already exists, otherwise, false</returns>
        public async Task<bool> CreateDowloadFolder(string filePath)
        {
            if (!await FileExists(filePath))
            {
                try
                {
                    Directory.CreateDirectory(filePath);
                    logger.Info($"Directory created on path {filePath}");
                    return true; 
                }
                catch (Exception ex)
                {
                    logger.Error($"Error occurred while creating directory: " + ex.Message);
                    return false;
                }
            }
            return true; 
        }

        /// <summary>
        /// Downloads the image of the specified product from the web page
        /// </summary>
        /// <param name="product">The product for which to download the image</param>
        /// <param name="configuration">The web configuration </param>
        /// <returns>The image data transfer object</returns>
        public async Task<ImageDto> DownloadProductImage(Product product, WebConfiguration configuration)
        {
                HtmlWeb web = new HtmlWeb();
                HtmlDocument document = web.Load(configuration.UrlPage + product.Code);

                var images = document.DocumentNode.SelectNodes(configuration.ImgTag);
                if (images.IsNullOrEmpty())
                {

                    logger.Error($"There are no images for product with code {product.Code}.");
                    return null; 
                }

                var firstImage = GetFirstImage(images) ;
                if (firstImage == null)
                {
                    logger.Error($"No image exists for the product with the code {product.Code}.");
                    return null;
                }

                string url = GetPageUrl(configuration, firstImage);
                if (url.IsNullOrEmpty())
                {
                    logger.Error("Image url is null or empty.");
                    return null;
                }

                var imageName = product.Code + configuration.StorageExtension;
                var imagePath = Path.Combine(Directory.GetCurrentDirectory(), configuration.StorageFolder, imageName);

                using (var client = new WebClient())
                {
                    try
                    {
                        client.DownloadFile(url, imagePath);
                        ImageDto imageDto = new ImageDto
                        {
                            Path = imagePath,
                            ProductId = product.Id
                        };
                        return imageDto;
                    }
                    catch (Exception ex)
                    {
                        logger.Error($"Error occured while dowloading image: {ex.Message}");
                        return null;
                    }
                }
        }

        /// <summary>
        /// Retrieves the URL of the page where the image is located
        /// </summary>
        /// <param name="configuration">Web configuration</param>
        /// <param name="firstImage">First image on the page</param>
        /// <returns>The URL of the page where the image is located</returns>
        private string GetPageUrl(WebConfiguration configuration, HtmlNode firstImage)
        {
            var url = firstImage.GetAttributeValue(configuration.Img, "");
            if (!string.IsNullOrEmpty(url) && !Uri.IsWellFormedUriString(url, UriKind.Absolute))
            {
                url = new Uri(new Uri(configuration.UrlPage), url).AbsoluteUri;
            }
            return url;
        }

        /// <summary>
        /// Retrieves the first image from a collection 
        /// </summary>
        /// <param name="images">Collection of HTML nodes</param>
        /// <returns>The first HTML node representing an image, or null if the collection is empty</returns>

        private HtmlNode GetFirstImage(HtmlNodeCollection images)
        {
            return images.FirstOrDefault();
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
                return File.Exists(path);
            }
            catch (Exception ex)
            {

                logger.Error("Error occurred while checking the existence of the file. " + ex.Message);
                return false;
            }
        }

        /// <summary>
        /// Retrieves the URL of the first image from a web page specified by the given URL and web configuration
        /// </summary>
        /// <param name="pageUrl">The URL of the web page from which to retrieve the image URL</param>
        /// <param name="conf">The web configuration containing the XPath expression to locate the image on the web page</param>
        /// <returns>
        /// If successful, returns the URL of the first image. If no image is found or an error occurs during the process
        /// returns null
        /// </returns>
        public async Task<string> GetFirstImageUrlFromPage(string pageUrl, WebConfiguration conf)
        {
            try
            {
                var web = new HtmlWeb();
                var document = await web.LoadFromWebAsync(pageUrl);

                var firstImage = document.DocumentNode.SelectNodes(conf.ImgTag)?.FirstOrDefault();
                if (firstImage == null)
                {
                    logger.Error($"No image exists for the product.");
                    return null;
                }
                string url = GetPageUrl(conf, firstImage);
                if (url.IsNullOrEmpty())
                {
                    logger.Error("Image url is null or empty.");
                    return null;
                }
                return url;
            }
            catch (Exception ex)
            {
                logger.Error($"An error occurred while fetching the first image URL: {ex.Message}");
                return null;
            }
        }

    }
}
