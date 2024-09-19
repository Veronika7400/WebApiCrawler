using WebApiCrawler.SearchModels;

namespace WebApiCrawler.ModelsHGSpot
{

    public class SearchResultHgSpotDto : ISearchResultDto
    {
        public List<IProductDto> products { get; set; }
    }
}
