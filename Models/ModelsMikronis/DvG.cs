namespace WebApiCrawler.Models.ModelsMikronis
{
    public class DvG
    {
        public int Id { get; set; }
        public int DiscountId { get; set; }
        public int GroupId { get; set; }
        public double PercentOff { get; set; }
        public double FlatOff { get; set; }
        public string getKey { get; set; }
    }
}
