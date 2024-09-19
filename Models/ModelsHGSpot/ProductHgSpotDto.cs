using System.Globalization;
using WebApiCrawler.AbrakadabraModels;
using WebApiCrawler.SearchModels;

namespace WebApiCrawler.ModelsHGSpot
{
    public class ProductHgSpotDto : IProductDto
    {
        public string product_id { get; set; }
        public string price_euro { get; set; }
        public string name
        {
            get; set;
        }

        public double PriceValue => ConvertToDouble(price_euro);

        public string Link => Link;

        string IProductDto.ProductName
        {
            get => name.ToString();
            set => name = value;
        }

        private double ConvertToDouble(string price_euro)
        {
            double priceEuro;
            price_euro = price_euro.Replace(",", ".");

            if (double.TryParse(price_euro, NumberStyles.Any, CultureInfo.InvariantCulture, out priceEuro))
            {
                return priceEuro;
            }
            else
            {
                return 0; 
            }
        }
    }

}
