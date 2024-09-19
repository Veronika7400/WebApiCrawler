namespace WebApiCrawler.AbrakadabraModels
{
    public class PriceDto
    {
        public string currencyIso { get; set; }
        public double value { get; set; }
        public string formattedValue { get; set; }
        public string decimalSeparator { get; set; }
    }
}
