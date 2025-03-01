using ADMIN.Constant;
using ADMIN.Models.DTO;
using Microsoft.Extensions.Options;
using static ADMIN.Constant.SD;

namespace ADMIN.Services
{
    public interface IAuthServices
    {
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
