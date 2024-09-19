namespace WebApiCrawler.AbrakadabraModels
{

    public class ProductAbrakadabra
    {
        public string code { get; set; }
        public string name { get; set; }
        public string url { get; set; }
        public string description { get; set; }
        public object purchasable { get; set; }
        public Stock stock { get; set; }
        public object futureStocks { get; set; }
        public bool availableForPickup { get; set; }
        public object averageRating { get; set; }
        public object numberOfReviews { get; set; }
        public string summary { get; set; }
        public string manufacturer { get; set; }
        public object variantType { get; set; }
        public Price price { get; set; }
        public object baseProduct { get; set; }
        public List<Image> images { get; set; }
        public object categories { get; set; }
        public object reviews { get; set; }
        public object classifications { get; set; }
        public object potentialPromotions { get; set; }
        public object variantOptions { get; set; }
        public object baseOptions { get; set; }
        public bool volumePricesFlag { get; set; }
        public object volumePrices { get; set; }
        public object productReferences { get; set; }
        public object variantMatrix { get; set; }
        public PriceRange priceRange { get; set; }
        public object firstCategoryNameList { get; set; }
        public object multidimensional { get; set; }
        public object configurable { get; set; }
        public object configuratorType { get; set; }
        public object addToCartDisabled { get; set; }
        public object addToCartDisabledMessage { get; set; }
        public object tags { get; set; }
        public object keywords { get; set; }
        public Brand brand { get; set; }
        public object sku { get; set; }
        public object gtin14 { get; set; }
        public object displayNameToCompleteItem { get; set; }
        public object videos { get; set; }
        public object files { get; set; }
        public object genders { get; set; }
        public object minPrice { get; set; }
        public int pickupTime { get; set; }
        public object pickupSlaTime { get; set; }
        public object externalid { get; set; }
        public List<FlagInfoDataList> flagInfoDataList { get; set; }
        public List<string> categoryNames { get; set; }
        public object potentialOrderPromotions { get; set; }
        public object unit { get; set; }
        public string groupName { get; set; }
        public object groupIcon { get; set; }
        public object lastSupercategoryName { get; set; }
        public object color { get; set; }
        public List<string> categoryCodes { get; set; }
        public object loyaltyPointsMultiplier { get; set; }
        public object loyaltyPoints { get; set; }
        public object availableOnStorefront { get; set; }
        public object availablePercentDiscount { get; set; }
        public object excludedDeliveryTypes { get; set; }
        public object manufacturerAID { get; set; }
        public object energyCertificate { get; set; }
        public object promotionDetails { get; set; }
        public List<VariantsDatum> variantsData { get; set; }
        public object percentDiscount { get; set; }
        public bool isAgeRestricted { get; set; }
        public object isDigit { get; set; }
        public object measure { get; set; }
        public object pricePerVolume { get; set; }
        public object minPricePerVolume { get; set; }
        public object allPromotions { get; set; }
        public object promotionPrice { get; set; }
        public object variantCode { get; set; }

    }
}
