namespace WebApiCrawler.Models.ModelsMikronis
{
    public class Discount
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string UrlName { get; set; }
        public DateTime StartsFrom { get; set; }
        public DateTime EndsOn { get; set; }
        public bool Enabled { get; set; }
        public string Code { get; set; }
        public double PercentOff { get; set; }
        public double FlatOff { get; set; }
        public double OriginalPrice { get; set; }
        public List<object> Products { get; set; }
        public List<object> RelatedProducts { get; set; }
        public List<object> Groups { get; set; }
        public List<object> RelatedGroups { get; set; }
        public List<object> Brands { get; set; }
        public List<object> RelatedBrands { get; set; }
        public List<object> Series { get; set; }
        public List<object> Users { get; set; }
        public string DiscountAggregation { get; set; }
        public List<object> DvU { get; set; }
        public List<object> DvUR { get; set; }
        public List<object> DvPromo { get; set; }
        public List<DvP> DvP { get; set; }
        public List<DvG> DvG { get; set; }
        public List<object> DvB { get; set; }
        public List<object> Restrictions { get; set; }
        public List<object> DvRP { get; set; }
        public List<object> DvRG { get; set; }
        public List<object> DvRB { get; set; }
        public List<object> DvS { get; set; }
        public bool SingleUse { get; set; }
        public double MinBasketPrice { get; set; }
        public double MaxBasketPrice { get; set; }
        public string MinBasketPrice_print { get; set; }
        public string MaxBasketPrice_print { get; set; }
        public int MinBasketQuantity { get; set; }
        public int MaxBasketQuantity { get; set; }
        public bool OverrideMpc { get; set; }
        public bool OverrideDiscount { get; set; }
        public bool IsSummable { get; set; }
        public bool IsCumulative { get; set; }
        public UsedDiscount UsedDiscount { get; set; }
        public bool MultipleDiscounts { get; set; }
        public int VoucherProductId { get; set; }
        public List<File> Files { get; set; }
        public bool IsB2B { get; set; }
        public Translations Translations { get; set; }
        public int ProductLimit { get; set; }
        public int MaxLoyalty { get; set; }
        public int MinLoyalty { get; set; }
        public string Description { get; set; }
    }
}
