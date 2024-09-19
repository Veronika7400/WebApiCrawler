
using Newtonsoft.Json;

namespace WebApiCrawler.AbrakadabraModels
{
   

    public class Stock
    {
        public StockLevelStatus stockLevelStatus { get; set; }
        public object stockLevel { get; set; }
        public object stockThreshold { get; set; }
    }

    
}
