
using Newtonsoft.Json;

namespace WebApiCrawler.AbrakadabraModels
{
    public class TopCategory
    {
        public string code { get; set; }
        public string name { get; set; }
        public string url { get; set; }
        public object description { get; set; }
        public object image { get; set; }
        public object parentCategoryName { get; set; }
        public int sequence { get; set; }
    }
}
