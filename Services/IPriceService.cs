using Microsoft.AspNetCore.Mvc;

namespace WebApiCrawler.Services
{
    public interface IPriceService
    {
        Task<IActionResult> GetLowestPriceInEachStore(string searchWord, int page, int pageSize);
        Task<IActionResult> GetPricesFromUrl(string searchWord, Guid guid);
        Task<IActionResult> GetLowestPrice(IActionResult pricesResult);
        Task<IActionResult> GetPricesFromUrlLowestPriceResult(string searchWord, Guid guid, int page, int pageSize);
        Task<IActionResult> GetLowestPriceInEachStoreWithoutPagination(string searchWord);
        Task<IActionResult> GetPricesFromUrlLowestPriceResultWithoutPagination(string searchWord, Guid guid); 


    }
}
