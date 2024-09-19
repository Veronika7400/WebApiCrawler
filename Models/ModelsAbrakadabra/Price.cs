
using Newtonsoft.Json;

namespace WebApiCrawler.AbrakadabraModels
{
   
    public class Price
    {
        public string currencyIso { get; set; }
        public double value { get; set; }
        public string priceType { get; set; }
        public string formattedValue { get; set; }
        public object minQuantity { get; set; }
        public object maxQuantity { get; set; }
        public string currencySymbol { get; set; }
        public string formattedRoundedPriceValue { get; set; }
        public string decimalSeparator { get; set; }
        public bool isCustomPrice { get; set; }
        public string priceColorCode { get; set; }
    }

}
