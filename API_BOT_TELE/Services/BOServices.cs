using Newtonsoft.Json;
using TELEBOT_CSKH.Constant;
using TELEBOT_CSKH.Models.DTO;
using static System.Net.WebRequestMethods;

namespace TELEBOT_CSKH.Services
{
    public interface IBOServices
    {
        Task<ResponseDTO> CheckNewRegisterAgentF168Async(string AgentName, long FromDateUTC, long ToDateUTC);
        Task<ResponseDTO> CheckNewCustomerAgentF168Async(string AgentName, long FromDateUTC, long ToDateUTC);
        Task<ResponseDTO> CheckTotalAgentF168Async(string agentName, long fromDateUTC, long toDateUTC);
        Task<ResponseDTO> F168_CheckAccount(string Account, string SiteSystem);
    }

    public class BOServices : IBOServices
    {
        private IBaseServices _baseServices;
        private ResponseDTO _responseDTO;

        public BOServices(IBaseServices baseServices)
        {
            _responseDTO = new ResponseDTO();
            _baseServices = baseServices;
        }

        public async Task<ResponseDTO> CheckTotalAgentF168Async(string AgentName, long FromDateUTC, long ToDateUTC)
        {
            var dataPost = new
            {
                parentName = AgentName.ToLower(),
                startTime = FromDateUTC,
                endTime = ToDateUTC
            };
            return await _baseServices.LiveSendAsync(new RequestDTO()
            {
                APIType = SD.APIType.POST,
                Data = dataPost,
                Url = "https://api-bo-f168wg.khuyenmaiapp.com/statistics/get-account-agent?site=f168"
            });
        }

        public async Task<ResponseDTO> CheckNewRegisterAgentF168Async(string AgentName, long FromDateUTC, long ToDateUTC)
        {
            var dataPost = new
            {
                parentName = AgentName.ToLower(),
                startTime = FromDateUTC,
                endTime = ToDateUTC,
                size = 10
            };
            return await _baseServices.LiveSendAsync(new RequestDTO()
            {
                APIType = SD.APIType.POST,
                Data = dataPost,
                Url = "https://api-bo-f168wg.khuyenmaiapp.com/statistics/get-register-analytics?site=f168"
            });
        }

        public async Task<ResponseDTO> CheckNewCustomerAgentF168Async(string AgentName, long FromDateUTC, long ToDateUTC)
        {
            var dataPost = new
            {
                parentName = AgentName.ToLower(),
                startTime = FromDateUTC,
                endTime = ToDateUTC,
                size = 10
            };
            return await _baseServices.LiveSendAsync(new RequestDTO()
            {
                APIType = SD.APIType.POST,
                Data = dataPost,
                Url = "https://api-bo-f168wg.khuyenmaiapp.com/statistics/get-firstPay-analytics?site=f168"
            });
        }

        public async Task<ResponseDTO> F168_CheckAccount(string Account, string CardHolder)
        {
            string F168CheckEndpoint = $"https://api-f168platform.khuyenmaiapp.com/member/info-detail?site=f168";
            var dataPost = new
            {
                username = Account.ToLower(),
            };
            return await _baseServices.SendAsync(new RequestDTO()
            {
                APIType = SD.APIType.POST,
                Data = dataPost,
                Url = F168CheckEndpoint
            });
        }
    }
}
