using Microsoft.AspNetCore.Mvc;
using WebApiCrawler.Models;

namespace WebApiCrawler.Services
{
    public interface IImageService
    {
        Task<IActionResult> AddNewImage(ImageDto imageDto);
        Task<bool> ImageExists(ImageDto imageDto); 
    }
}
