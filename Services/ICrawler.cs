using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebApiCrawler.Models;

namespace WebApiCrawler
{
    internal interface ICrawler
    {
        Task<bool> CreateDowloadFolder(string filePath);
        Task<ImageDto> DownloadProductImage(Product product, WebConfiguration configuration);
    }
}
