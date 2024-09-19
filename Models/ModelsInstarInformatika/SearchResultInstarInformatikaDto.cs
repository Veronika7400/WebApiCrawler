using Mapster;
using WebApiCrawler.SearchModels;

namespace WebApiCrawler.Models.ModelsInstarInformatika
{
    public class SearchResultInstarInformatikaDto : ISearchResultDto
    {
        public List<IProductDto> product { get; set; }
        public List<IProductDto> products
        {
            get => product;
            set {product = product; }
        }

    }
}
