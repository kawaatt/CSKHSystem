using static API.Constant.SD;

namespace API.Models.DTO
{
    public class RequestDTO
    {
        public APIType APIType { get; set; } = APIType.GET;
        public string Url { get; set; }
        public object Data { get; set; }
        public string AccessToken { get; set; }
    }

    public class BOAccountRequestDTO
    {
        public string Account { get; set; }
        public string CardHolder { get; set; }
        public int? RequestCount { get; set; }
        public DateTime? TheLastRequest { get; set; }
    }
}
