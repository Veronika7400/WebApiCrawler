using WebApiCrawler.SearchModels;

namespace WebApiCrawler.Models.ModelsInstarInformatika
{
    public class ProductInstarInformatikaDto : IProductDto
    {
        public string artikl { get; set; }

        public string ProductName { get => artikl; set =>artikl=value; }

        public double price { get; set; }

        public double PriceValue =>price;
        public string Link { get; set; }
    }
}
