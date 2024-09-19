using Mapster;
using WebApiCrawler.SearchModels;

namespace WebApiCrawler.Models.ModelsMikronis
{
    public class SearchResultMikronisDto : ISearchResultDto
    {
        public Result Result { get; set; }

        public List<IProductDto> products
        {
            get => Result.Products?.Adapt<List<IProductDto>>();
            set
            {
                products = Result.Products.Adapt<List<IProductDto>>();
            }

        }
    }
}
