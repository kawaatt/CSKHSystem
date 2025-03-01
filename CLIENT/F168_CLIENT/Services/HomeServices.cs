using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using SHBET_CLIENT.Constant;
using SHBET_CLIENT.Models.DTO;
using SHBET_CLIENT.Provider;
using System.Security.Principal;
using System.Text;
using static SHBET_CLIENT.Constant.SD;

namespace SHBET_CLIENT.Services
{
    public interface IHomeServices
    {
        Task<ResponseDTO> GetTicketStatus();
    }

    public class HomeServices : IHomeServices
    {
        private readonly IBaseServices _baseService;
        private readonly ApiEndPointConstant _apiEndPointConstant;

        public HomeServices(IBaseServices baseService, IOptions<ApiEndPointConstant> apiEndPointConstant)
        {
            _baseService = baseService;
            _apiEndPointConstant = apiEndPointConstant.Value;
        }

        public async Task<ResponseDTO> GetTicketStatus()
        {
            string MyURL = _apiEndPointConstant.API_CSKH_AUTOMATICALLY_ENDPOINT + $"/api/TicketStatus";
            Console.WriteLine(MyURL);
            return await _baseService.BaseSendAsync(new RequestDTO()
            {
                APIType = SD.APIType.GET,
                Url = MyURL,
            });
        }
    }
}
