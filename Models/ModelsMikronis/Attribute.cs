namespace WebApiCrawler.Models.ModelsMikronis
{
    public class Attribute
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int ValueSort { get; set; }
        public string Code { get; set; }
        public List<object> AttributeGroups { get; set; }
        public AttributeValue AttributeValue { get; set; }
        public List<object> AttributeValues { get; set; }
        public List<AttributeType> AttributeTypes { get; set; }
        public List<object> Files { get; set; }
        public bool deleted { get; set; }
        public Translations Translations { get; set; }
        public int? Sort { get; set; }
    }
}
