
using Newtonsoft.Json;

namespace WebApiCrawler.AbrakadabraModels
{
    
    public class SmallImageData
    {
        public object imageType { get; set; }
        public object format { get; set; }
        public string url { get; set; }
        public object altText { get; set; }
        public object galleryIndex { get; set; }
        public object width { get; set; }
        public object imageMapHTML { get; set; }
        public object containerId { get; set; }
    }
}
