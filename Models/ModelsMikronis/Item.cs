namespace WebApiCrawler.Models.ModelsMikronis
{
    public class Item
    {
        public string Name { get; set; }
        public string UrlName { get; set; }
        public int Hits { get; set; }
        public bool IsChecked { get; set; }
        public string FilterType { get; set; }
        public bool IsMenu { get; set; }
        public List<object> Children { get; set; }
        public string fullUrl { get; set; }
        public int Sort { get; set; }
        public string Id { get; set; }
        public string ParentId { get; set; }
    }
}
