namespace SKTISWebsite.Models.Common
{
    public abstract class BaseCriteria
    {
        public int PageIndex { get; set; }
        public int PageSize { get; set; }
        public string SortExpression { get; set; }
        public string SortOrder { get; set; }
    }
}