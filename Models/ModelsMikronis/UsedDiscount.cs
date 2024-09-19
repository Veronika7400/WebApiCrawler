namespace WebApiCrawler.Models.ModelsMikronis
{
    public class UsedDiscount
    {
        public int DiscountId { get; set; }
        public double Price { get; set; }
        public double Percent { get; set; }
        public double BasePrice { get; set; }
        public double FlatOff { get; set; }
    }
}
