
using Newtonsoft.Json;

namespace WebApiCrawler.AbrakadabraModels
{
   

    public class FlagInfoDataList
    {
        public string code { get; set; }
        public object priority { get; set; }
        public object mainImageData { get; set; }
        public SmallImageData smallImageData { get; set; }
        public object alwaysShow { get; set; }
        public object startDate { get; set; }
        public object lastDate { get; set; }
    }

}
