using Microsoft.Extensions.Options;
using SHBET_CLIENT.Constant;
using SHBET_CLIENT.Models.DTO;
using System.Security.Principal;
using static SHBET_CLIENT.Constant.SD;

namespace SHBET_CLIENT.Services
{
    public interface ITicketServices
    {
        Task<ResponseDTO> CheckAccountRequest(string Account, string CardHolder);
        Task<ResponseDTO> GetTicketByID(Guid ticketID);
        Task<ResponseDTO> SendTicketRequest(TicketRequestDTO _ticketRequestDTO);
    }

    public class TicketServices : ITicketServices
    {
        private readonly IBaseServices _baseService;
        private readonly ApiEndPointConstant _apiEndPointConstant;
        private readonly IdentityConstant _identityConstant;

        public TicketServices(IBaseServices baseService, IOptions<IdentityConstant> identityConstant, IOptions<ApiEndPointConstant> apiEndPointConstant)
        {
            _baseService = baseService;
            _apiEndPointConstant = apiEndPointConstant.Value;
            _identityConstant = identityConstant.Value;
        }

        public async Task<ResponseDTO> CheckAccountRequest(string Account, string CardHolder)
        {
            object payloadData = new
            {
                Account = Account,
                CardHolder = CardHolder
            };

            return await _baseService.BaseSendAsync(new RequestDTO()
            {
                APIType = SD.APIType.POST,
                Data = payloadData,
                Url = _apiEndPointConstant.API_CSKH_AUTOMATICALLY_ENDPOINT + "/api/BOAccount/F168CheckAccount"
            });
        }

        public async Task<ResponseDTO> GetTicketByID(Guid ticketID)
        {
            object payloadData = new
            {
                TicketID = ticketID,
                System = "F168"
            };
            return await _baseService.BaseSendAsync(new RequestDTO()
            {
                APIType = SD.APIType.POST,
                Data = payloadData,
                Url = _apiEndPointConstant.API_CSKH_AUTOMATICALLY_ENDPOINT + "/api/TicketRequest/GetTicketByID"
            });
        }

        public async Task<ResponseDTO> SendTicketRequest(TicketRequestDTO _ticketRequestDTO)
        {
            return await _baseService.BaseSendAsync(new RequestDTO()
            {
                APIType = SD.APIType.POST,
                Data = _ticketRequestDTO,
                Url = _apiEndPointConstant.API_CSKH_AUTOMATICALLY_ENDPOINT + "/api/TicketRequest/SendTicketRequest"
            });
        }
    }

}
