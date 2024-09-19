using System.Globalization;
using WebApiCrawler.SearchModels;

namespace WebApiCrawler.Models.ModelsMikronis
{
    public class ProductMicronisDto : IProductDto
    {
        public string Name { get; set; }
        public string ProductName { get => Name; set { ProductName = Name; } }

        public double FinalPriceToPay { get; set; }

        public double PriceValue => FinalPriceToPay;
        public string GenerateReadableUrl { get; set; }
        public string Link => GenerateReadableUrl;
    }
}
