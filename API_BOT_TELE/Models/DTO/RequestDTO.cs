using static TELEBOT_CSKH.Constant.SD;

namespace TELEBOT_CSKH.Models.DTO
{
    public class RequestDTO
    {
        public APIType APIType { get; set; } = APIType.GET;
        public string Url { get; set; }
        public object Data { get; set; }
        public string AccessToken { get; set; }
    }
}
