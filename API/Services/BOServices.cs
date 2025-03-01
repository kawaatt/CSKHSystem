using API.Models.DTO;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using API.Constant;

namespace API.Services
{
    public interface IBOServices
    {
        Task<ResponseDTO> F168_CheckAccount(string Account);
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

        public async Task<ResponseDTO> F168_CheckAccount(string Account)
        {
            String F168CheckEndpoint = $"https://api-f168platform.khuyenmaiapp.com/member/info-detail?site=f168";
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
