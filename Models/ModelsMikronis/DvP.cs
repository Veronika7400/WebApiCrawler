namespace WebApiCrawler.Models.ModelsMikronis
{
    public class DvP
    {
        public int Id { get; set; }
        public int DiscountId { get; set; }
        public int ProductId { get; set; }
        public double NewMpc { get; set; }
        public double NewVpc { get; set; }
        public double PercentOff { get; set; }
        public double BaseVpc { get; set; }
        public double QuantityFrom { get; set; }
        public double QuantityTo { get; set; }
        public string getKey { get; set; }
    }
}
