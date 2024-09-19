namespace WebApiCrawler.Models.ModelsMikronis
{
    public class Manufacturers
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int Sort { get; set; }
        public bool Enabled { get; set; }
        public int luceed_id { get; set; }
        public int luceed_sid { get; set; }
        public string UrlName { get; set; }
        public List<object> Files { get; set; }
    }
}
