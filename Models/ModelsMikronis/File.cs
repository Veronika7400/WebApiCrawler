using Microsoft.VisualBasic.FileIO;

namespace WebApiCrawler.Models.ModelsMikronis
{
    public class File
    {
        public int Id { get; set; }
        public string Path { get; set; }
        public int ProductId { get; set; }
        public int GroupId { get; set; }
        public int BrandId { get; set; }
        public int BusinessUnitId { get; set; }
        public int SerieId { get; set; }
        public int DiscountId { get; set; }
        public int OrderId { get; set; }
        public int BannerId { get; set; }
        public int PageId { get; set; }
        public int Sort { get; set; }
        public int FileTypeId { get; set; }
        public int DeliveryId { get; set; }
        public int AttributeValueId { get; set; }
        public int AttributeId { get; set; }
        public string MimeType { get; set; }
        public string generatedImageUrl { get; set; }
        public FileType FileType { get; set; }
        public string Code { get; set; }
        public Translations Translations { get; set; }
    }
}
