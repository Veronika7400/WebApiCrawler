namespace WebApiCrawler.Models.ModelsMikronis
{
    public class Result
    {
        public List<object> GrupeAkcija { get; set; }
        public List<ProductMicronis> Products { get; set; }
        public List<object> AllAttributes { get; set; }
        public List<object> AllSelectedAttributes { get; set; }
        public AllBrands AllBrands { get; set; }
        public AllGroups AllGroups { get; set; }
        public int NumberOfHits { get; set; }
        public List<object> PickedGroupNames { get; set; }
        public int Sort { get; set; }
        public List<object> PickedAttributes { get; set; }
        public List<object> PickedSeriesNames { get; set; }
        public List<object> Breadcrumbs { get; set; }
        public List<object> KatalogDiscounts { get; set; }
        public bool isNaslovna { get; set; }

    }
}
