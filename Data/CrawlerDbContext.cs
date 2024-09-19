using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using WebApiCrawler.Models;

namespace WebApiCrawler.Data
{
    public class CrawlerDbContext : DbContext
    {
        public CrawlerDbContext(DbContextOptions<CrawlerDbContext> options) : base(options)
        {

        }

        public DbSet<Product> Products { get; set; }
        public DbSet<Image> Images { get; set; }
        public DbSet<WebConfiguration> WebConfigurations { get; set; }
        public DbSet<WebStore> WebStores { get; set; }

        /// <summary>
        /// This method is used to configure the model for the DbSet
        /// </summary>
        /// <param name="builder">The builder being used to construct the model for this context</param>
        protected override void OnModelCreating(ModelBuilder builder)
        {
            Guid firstStoreId = Guid.NewGuid();
            Guid secondStoreId = Guid.NewGuid();
            Guid thirdStoreId = Guid.NewGuid();
            Guid fourthStoreId = Guid.NewGuid();
            Guid fifthStoreId = Guid.NewGuid();
            Guid sixthStoreId = Guid.NewGuid();
            builder.Entity<WebStore>().HasData(
                new WebStore
                {
                    Id = firstStoreId,
                    Name = "eKupi"
                },
                new WebStore
                {
                    Id = secondStoreId,
                    Name = "Abrakadabra"
                },
                new WebStore
                {
                    Id = thirdStoreId,
                    Name = "eBay"
                },
                 new WebStore
                 {
                     Id = fourthStoreId,
                     Name = "HG Spot"
                 },
                new WebStore
                {
                    Id = fifthStoreId,
                    Name = "Mikronis"
                },
                new WebStore
                {
                    Id = sixthStoreId,
                    Name = "Instar informatika"
                }
            );

            builder.Entity<WebConfiguration>().HasData(
                new WebConfiguration
                {
                    Id= Guid.NewGuid(),
                    UrlPage = "https://www.ekupi.hr/p/",
                    ImgTag = "//img[@data-src]",
                    Img = "data-src",
                    StorageExtension = ".jpg",
                    StorageFolder = "ImagesDatabase", 
                    StoreId = firstStoreId,
                    SearchBox = "https://www.ekupi.hr/hr/search/autocomplete/SearchBox?term=",
                    ResultType = "WebApiCrawler.AbrakadabraModels.SearchResultAbrakadabra",
                    DisplayType = "WebApiCrawler.AbrakadabraModels.SearchResultAbrakadabraDto"

        },
                new WebConfiguration
                {
                    Id = Guid.NewGuid(),
                    UrlPage = "https://www.abrakadabra.com/p/",
                    ImgTag = "//img[@data-zoom-image]/@data-zoom-image",
                    Img = "src",
                    StorageExtension = ".png",
                    StorageFolder = "ImagesDatabase",
                    StoreId = secondStoreId,
                    SearchBox = "https://www.abrakadabra.com/hr-HR/search/autocomplete/SearchBox?term=",
                    ResultType = "WebApiCrawler.AbrakadabraModels.SearchResultAbrakadabra",
                    DisplayType = "WebApiCrawler.AbrakadabraModels.SearchResultAbrakadabraDto"
                },
                new WebConfiguration
                {
                    Id = Guid.NewGuid(),
                    UrlPage = "https://www.ebay.com/itm/",
                    ImgTag = "//img[@data-src]",
                    Img = "data-src",
                    StorageExtension = ".jpeg",
                    StorageFolder = "ImagesDatabase",
                    StoreId = thirdStoreId,
                    SearchBox = "",
                    ResultType = "",
                    DisplayType = ""
                },
                new WebConfiguration
                {
                    Id = Guid.NewGuid(),
                    UrlPage = "https://www.hgspot.hr/",
                    ImgTag = "//div[@id=\"product-gallery\"]//img",
                    Img = "src",
                    StorageExtension = ".jpeg",
                    StorageFolder = "ImagesDatabase",
                    StoreId = fourthStoreId,
                    SearchBox = "https://www.hgspot.hr/api/search-final.php?keyword=",
                    ResultType = "WebApiCrawler.ModelsHGSpot.SearchResultHgSpot",
                    DisplayType = "WebApiCrawler.ModelsHGSpot.SearchResultHgSpotDto"
                },
                new WebConfiguration
                {
                    Id = Guid.NewGuid(),
                    UrlPage = "https://www.mikronis.hr/Proizvod/",
                    ImgTag = "//div[@class=\"product-pic-inner\"]//img",
                    Img = "src",
                    StorageExtension = ".jpeg",
                    StorageFolder = "ImagesDatabase",
                    StoreId = fifthStoreId,
                    SearchBox = "https://www.mikronis.hr/api/shopapi/getSearchSuggestion?search=",
                    ResultType = "WebApiCrawler.Models.ModelsMikronis.SearchResultMikronis",
                    DisplayType = "WebApiCrawler.Models.ModelsMikronis.SearchResultMikronisDto"
                },
                new WebConfiguration
                {
                    Id = Guid.NewGuid(),
                    UrlPage = "https://www.instar-informatika.hr/",
                    ImgTag = "//a[@class=\"fancybox\"]/img",
                    Img = "src",
                    StorageExtension = ".jpg",
                    StorageFolder = "ImagesDatabase",
                    StoreId = sixthStoreId,
                    SearchBox = "https://www.instar-informatika.hr/api/search/multi/?term=",
                    ResultType = "WebApiCrawler.Models.ModelsInstarInformatika.SearchResultInstarInformatika",
                    DisplayType = "WebApiCrawler.Models.ModelsInstarInformatika.SearchResultInstarInformatikaDto"
                }
            );

            Guid productId1 = Guid.NewGuid();
            Guid productId2 = Guid.NewGuid();
            Guid productId3 = Guid.NewGuid();
            Guid productId4 = Guid.NewGuid();

            builder.Entity<Product>().HasData(
                new Product
                {
                    Id = productId1,
                    Code = "EK000525407",
                    Name = "Electrolux aparat za kavu",
                    StoreId = firstStoreId
                },
                new Product
                {
                    Id = productId2,
                    Code = "EK000467594",
                    Name = "TP-Link Deco E4, Mesh Wi-Fi sistem",
                    StoreId = firstStoreId
                },
                new Product
                {
                    Id = productId3,
                    Code = "EK000439180",
                    Name = "Logitech Gaming G435 Lightspeed, bežične slušalice",
                    StoreId = firstStoreId
                },
                new Product
                {
                    Id = productId4,
                    Code = "EK000586865",
                    Name = "POLAR gradski bicikl",
                    StoreId = firstStoreId
                },
                new Product
                {
                    Id = Guid.NewGuid(),
                    Code = "225463979956",
                    Name = "Cambridge Audio R100 FM/AM Stereo Receiver",
                    StoreId = thirdStoreId
                },
                new Product
                {
                    Id = Guid.NewGuid(),
                    Code = "155770850364",
                    Name = "Bluetooth Earbuds Headset",
                    StoreId = thirdStoreId
                }
            );
        }
    }
}
