using WebApiCrawler.SearchModels;

namespace WebApiCrawler.AbrakadabraModels
{
    public class ProductAbrakadabraDto : IProductDto
    {
        public string name { get; set; }
        public string ProductName
        {
            get => name.ToString();
            set => name = value;
        }
   
        public string url { get; set; }
        public PriceDto price { get; set; }

        public double PriceValue => price.value;

        public string Link { get => url;  }
    }
}
