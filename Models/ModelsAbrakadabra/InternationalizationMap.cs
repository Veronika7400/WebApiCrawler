
using Newtonsoft.Json;

namespace WebApiCrawler.AbrakadabraModels
{
  
    public class InternationalizationMap
    {
        public string suggestionSearchText { get; set; }

        [JsonProperty("search.all.categories")]
        public string searchallcategories { get; set; }

        [JsonProperty("see.all.products")]
        public string seeallproducts { get; set; }
    }

}
