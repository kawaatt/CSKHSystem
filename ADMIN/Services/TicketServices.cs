using ADMIN.Constant;
using ADMIN.Extensions;
using ADMIN.Models.CSKHAuto;
using ADMIN.Models.DTO;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using System.IdentityModel.Tokens.Jwt;

namespace ADMIN.Services
{
    
    public interface ITicketServices
    {
        Task<ResponseDTO> AcceptTicketProcess(string TicketID, string System, string Username);
        Task<ResponseDTO> UpdateTicketProcess(string TicketID, string UserName, string CheckedResult, string System);
        Task<ResponseDTO> FinishTicketProcess(string TicketID, string UserName, string CheckedResult, string System);
        Task<ResponseDTO> GetTicketByID(string TicketID, string System);
        Task<ResponseDTO> GetTicketData(QueryParameterDTO parameters);
    }

    public class TicketServices : ITicketServices
    {
        private readonly IBaseServices _baseService;
        private readonly ApiEndPointConstant _apiEndPointConstant;

        public TicketServices(IBaseServices baseService, IOptions<ApiEndPointConstant> apiEndPointConstant)
        {
            _baseService = baseService;
            _apiEndPointConstant = apiEndPointConstant.Value;
        }

        

        public async Task<ResponseDTO> AcceptTicketProcess(string TicketID, string System, string Username)
        {
            return await _baseService.BaseSendAsync(new RequestDTO()
            {
                APIType = SD.APIType.POST,
                Url = $"{_apiEndPointConstant.API_CSKH_AUTOMATICALLY_ENDPOINT}/api/TicketRequest/AcceptTicketProcess?TicketID={TicketID}&System={System}&Username={Username}"
            });
        }

        public async Task<ResponseDTO> GetTicketByID(string TicketID,string System)
        {
            var payload = new
            {
                TicketID = TicketID,
                System = System
            };

            return await _baseService.BaseSendAsync(new RequestDTO()
            {
                APIType = SD.APIType.POST,
                Data = payload,
                Url = $"{_apiEndPointConstant.API_CSKH_AUTOMATICALLY_ENDPOINT}/api/TicketRequest/GetTicketByID"
            });
        }

        public async Task<ResponseDTO?> GetTicketData(QueryParameterDTO parameters)
        {
            string URL = $"{_apiEndPointConstant.API_CSKH_AUTOMATICALLY_ENDPOINT}/api/TicketRequest?{ApiBaseExtension.ToQueryString(parameters)}";
            Console.WriteLine(URL);
            return await _baseService.BaseSendAsync(new RequestDTO()
            {
                APIType = SD.APIType.GET,
                Url = URL
            });
        }

        public async Task<ResponseDTO> UpdateTicketProcess(string TicketID, string UserName, string CheckedResult, string System)
        {
            TicketHistoryDTO _tempTicketHistory = new TicketHistoryDTO();
            _tempTicketHistory.TicketRequestID = Guid.Parse(TicketID);
            _tempTicketHistory.UpdatedByUser = UserName;
            _tempTicketHistory.TicketStatusDescription = CheckedResult;
            _tempTicketHistory.System = System;
            _tempTicketHistory.TicketStatusValue = 2;

            return await _baseService.BaseSendAsync(new RequestDTO()
            {
                APIType = SD.APIType.POST,
                Data = _tempTicketHistory,
                Url = $"{_apiEndPointConstant.API_CSKH_AUTOMATICALLY_ENDPOINT}/api/TicketRequest/UpdateTicketProcess"
            });
        }

        public async Task<ResponseDTO> FinishTicketProcess(string TicketID, string UserName, string CheckedResult, string System)
        {
            TicketHistoryDTO _tempTicketHistory = new TicketHistoryDTO();
            _tempTicketHistory.TicketRequestID = Guid.Parse(TicketID);
            _tempTicketHistory.UpdatedByUser = UserName;
            _tempTicketHistory.TicketStatusDescription = CheckedResult;
            _tempTicketHistory.System = System;
            _tempTicketHistory.TicketStatusValue = 3;

            return await _baseService.BaseSendAsync(new RequestDTO()
            {
                APIType = SD.APIType.POST,
                Data = _tempTicketHistory,
                Url = $"{_apiEndPointConstant.API_CSKH_AUTOMATICALLY_ENDPOINT}/api/TicketRequest/FinishTicketProcess"
            });
        }
    }

}