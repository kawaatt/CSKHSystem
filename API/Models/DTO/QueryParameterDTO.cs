namespace API.Models.DTO
{
    public class QueryParameterDTO
    {
        public string System { get; set; } //site_upper
        public int PageIndex { get; set; } = 1;
        public int PageSize { get; set; } = 50;
        public string? SortBy { get; set; }
        public string? SortDirection { get; set; }
        public string? SearchText { get; set; }
        public DateTime? StartTime { get; set; }
        public DateTime? EndTime { get; set; }
        public int Status { get; set; }
    }
}
