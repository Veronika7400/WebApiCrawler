
using Newtonsoft.Json;
using WebApiCrawler.Models;
using WebApiCrawler.SearchModels;

namespace WebApiCrawler.AbrakadabraModels
{ 


    public class SearchResultAbrakadabra 
    {
        public List<Suggestion> suggestions { get; set; }
        public List<ProductAbrakadabra> products { get; set; }
        public List<TopCategory> topCategories { get; set; }
        public string topSuggestion { get; set; }
        public InternationalizationMap internationalizationMap { get; set; }
    }

}
