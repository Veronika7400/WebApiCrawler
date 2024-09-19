namespace WebApiCrawler.Models.ModelsMikronis
{
    public class AttributeValue
    {
        public int Id { get; set; }
        public int AttributeNameId { get; set; }
        public int ProductId { get; set; }
        public string Value { get; set; }
        public string Name { get; set; }
        public List<object> Files { get; set; }
        public double Value_Decimal { get; set; }
        public int Value_Int { get; set; }
        public bool IsNaslovnica { get; set; }
        public bool IsVariation { get; set; }
        public int Sort { get; set; }
        public bool IsChecked { get; set; }
        public Translations Translations { get; set; }
    }
}
