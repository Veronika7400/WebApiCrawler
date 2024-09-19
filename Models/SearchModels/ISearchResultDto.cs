using WebApiCrawler.Models;

namespace WebApiCrawler.SearchModels
{
    public interface ISearchResultDto
    {
        List<IProductDto> products { get; set; }
    }
}
