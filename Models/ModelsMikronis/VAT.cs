namespace WebApiCrawler.Models.ModelsMikronis
{
    public class VAT
    {
        public int Id { get; set; }
        public int VAT_percent { get; set; }
        public double Value { get; set; }
        public bool is_default { get; set; }
        public string Description { get; set; }
        public int ErpId { get; set; }
    }
}
