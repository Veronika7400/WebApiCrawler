
using Newtonsoft.Json;

namespace WebApiCrawler.AbrakadabraModels
{
    public class Image
    {
        public string imageType { get; set; }
        public string format { get; set; }
        public string url { get; set; }
        public object altText { get; set; }
        public object galleryIndex { get; set; }
        public object width { get; set; }
        public object imageMapHTML { get; set; }
        public object containerId { get; set; }
    }

}
