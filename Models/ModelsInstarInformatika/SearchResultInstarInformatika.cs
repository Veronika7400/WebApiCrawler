namespace WebApiCrawler.Models.ModelsInstarInformatika
{
    public class SearchResultInstarInformatika
    {
        public List<ProductInstarInformatika> product { get; set; }
        public List<Category> category { get; set; }
        public string subCategory { get; set; }
        public List<Manufacture> manufacture { get; set; }
        public int totalResult { get; set; }
    }
}
