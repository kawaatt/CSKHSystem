namespace SHBET_CLIENT.Models.DTO
{
    public class QueryParameterDTO
    {
        public string ProjectCode { get; set; } //site_upper
        public string? Search { get; set; }
        public int PageIndex { get; set; } = 1;
        public int PageSize { get; set; } = 50;
    }
}
