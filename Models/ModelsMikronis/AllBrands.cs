namespace WebApiCrawler.Models.ModelsMikronis
{
    public class AllBrands
    {
        public int Id { get; set; }
        public int Sort { get; set; }
        public bool isFilter { get; set; }
        public bool IsChecked { get; set; }
        public List<object> types { get; set; }
        public List<Item> items { get; set; }
    }
}
