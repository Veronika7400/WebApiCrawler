namespace WebApiCrawler.Models.ModelsMikronis
{
    public class Group
    {
        public int Id { get; set; }
        public int ParentId { get; set; }
        public int GrandParentId { get; set; }
        public string ParentCode { get; set; }
        public string Name { get; set; }
        public string UrlName { get; set; }
        public string FrontendName { get; set; }
        public bool Enabled { get; set; }
        public int Sort { get; set; }
        public int Level { get; set; }
        public bool Deleted { get; set; }
        public string ErpId { get; set; }
        public string Code { get; set; }
        public bool isMenu { get; set; }
        public bool isHighlighted { get; set; }
        public string SeoTitle { get; set; }
        public string SeoDescription { get; set; }
        public string CatalogueUrl { get; set; }
        public List<object> Attributes { get; set; }
        public List<object> Children { get; set; }
        public int ChildrenCount { get; set; }
        public Translations Translations { get; set; }
        public bool show { get; set; }
        public string type { get; set; }
        public int ProductNum { get; set; }
        public List<int> RelatedGroupIds { get; set; }
        public List<Related> Related { get; set; }
        public List<object> RelatedProducts { get; set; }
        public List<object> SortProducts { get; set; }
        public List<object> RelatedManufacturers { get; set; }
        public List<object> Files { get; set; }
        public bool showChildren { get; set; }
        public bool Main { get; set; }
        public string ShortDescription { get; set; }
        public string Description { get; set; }
    }
}
