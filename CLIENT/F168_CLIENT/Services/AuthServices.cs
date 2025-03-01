using Microsoft.Extensions.Options;
using SHBET_CLIENT.Constant;
using SHBET_CLIENT.Models.DTO;
using static SHBET_CLIENT.Constant.SD;

namespace SHBET_CLIENT.Services
{
    public interface IAuthServices
    {
        Task<ResponseDTO?> GetClientToken();
        Task<ResponseDTO?> PingServer();
    }

    public class AuthServices : IAuthServices
    {
        private readonly IBaseServices _baseService;
        private readonly ApiEndPointConstant _apiEndPointConstant;
        private readonly IdentityConstant _identityConstant;

        public AuthServices(IBaseServices baseService, IOptions<IdentityConstant> identityConstant, IOptions<ApiEndPointConstant> apiEndPointConstant)
        {
            _baseService = baseService;
            _apiEndPointConstant = apiEndPointConstant.Value;
            _identityConstant = identityConstant.Value;
        }

        public async Task<ResponseDTO?> GetClientToken()
        {
            var identityInfo = new
            {
                grant_type = "client_credentials",
                scope = "api1",
                client_id = "cskhauto",
                client_secret = "secret_cskhauto"
            };

            return await _baseService.LiveSendFormURLEncodeContentAsync(new RequestDTO()
            {
                APIType = SD.APIType.POST,
                Data = identityInfo,
                Url = _identityConstant.IdentityUrl + "/connect/token",
                ContentType = ContentType.FormURLEncodedContent
            }, withBearer: false);
        }

        public async Task<ResponseDTO?> PingServer()
        {
            //throw new Exception("Test exception");
            return await _baseService.BaseSendAsync(new RequestDTO()
            {
                APIType = SD.APIType.GET,
                Url = _apiEndPointConstant.API_CSKH_AUTOMATICALLY_ENDPOINT + "/api/ping"
            });
        }
    }

}
