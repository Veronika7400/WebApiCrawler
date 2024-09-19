
using Newtonsoft.Json;
using WebApiCrawler.SearchModels;

namespace WebApiCrawler.AbrakadabraModels
{
    public class SearchResultAbrakadabraDto: ISearchResultDto
    {
        public List<IProductDto> products { get; set; }
    }
}
