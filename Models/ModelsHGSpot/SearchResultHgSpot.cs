using WebApiCrawler.SearchModels;

namespace WebApiCrawler.ModelsHGSpot
{
    public class SearchResultHgSpot 
    {
        public Dictionary<string, Category> categories { get; set; }
        public Dictionary<string, Brand> brands { get; set; }
        public List<ProductHgSpot> products { get; set; }
    }
}
