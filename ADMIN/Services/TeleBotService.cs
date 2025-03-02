using ADMIN.Constant;
using ADMIN.Extensions;
using ADMIN.Models.BotTele;
using ADMIN.Models.CSKHAuto;
using ADMIN.Models.DTO;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;

namespace ADMIN.Services
{
    
    public interface ITeleBotService
    {
        //Hooking Telegram Account
        Task<ResponseDTO> SetTelegramHook(Guid botID, string system);

        //TelegramAccount Services
        Task<ResponseDTO> GetBotTeleByIDAsync(Guid TicketID, string System);
        Task<ResponseDTO> GetBotTeleAsync(QueryParameterDTO ParamDTO);
        Task<ResponseDTO> AddNewTelegramBotAccount(TelegramAccountDTO botShareTeleDTO);
        Task<ResponseDTO> UpdateTelegramBotAccount(TelegramAccountDTO botShareTeleDTO);
        
        //TelegramResponse Services
        Task<ResponseDTO> GetTelegramResponseAsync(QueryParameterDTO ParamDTO);
        Task<ResponseDTO> AddNewTelegramResponse(TelegramResponseDTO telegramResponseDTO);
        Task<ResponseDTO> GetTelegramResponseByIDAsync(Guid iD);
        Task<ResponseDTO> UpdateTelegramResponse(TelegramResponseDTO responseDTO);

        Task<ResponseDTO> GetTeleCampaignByBotIDAsync(Guid iD);
    }

    public class TeleBotService : ITeleBotService
    {
        private readonly IBaseServices _baseService;
        private readonly ApiEndPointConstant _apiEndPointConstant;

        public TeleBotService(IBaseServices baseService, IOptions<ApiEndPointConstant> apiEndPointConstant)
        {
            _baseService = baseService;
            _apiEndPointConstant = apiEndPointConstant.Value;
        }

        public async Task<ResponseDTO> GetBotTeleByIDAsync(Guid TicketID,string System)
        {
            return await _baseService.BaseSendAsync(new RequestDTO()
            {
                APIType = SD.APIType.GET,
                Url = $"{_apiEndPointConstant.API_BOT_SHARE_TELE_ENDPOINT}/api/TelegramAccount/GetByID?ID={TicketID}&System={System}"
            });
        }

        public async Task<ResponseDTO?> GetBotTeleAsync(QueryParameterDTO ParamDTO)
        {
            return await _baseService.BaseSendAsync(new RequestDTO()
            {
                APIType = SD.APIType.GET,
                Url = $"{_apiEndPointConstant.API_BOT_SHARE_TELE_ENDPOINT}/api/TelegramAccount?{ApiBaseExtension.ToQueryString(ParamDTO)}",
            });
        }

        public async Task<ResponseDTO> AddNewTelegramBotAccount(TelegramAccountDTO botShareTeleDTO)
        {
            return await _baseService.BaseSendAsync(new RequestDTO()
            {
                APIType = SD.APIType.POST,
                Data= botShareTeleDTO,
                Url = $"{_apiEndPointConstant.API_BOT_SHARE_TELE_ENDPOINT}/api/TelegramAccount",
            });
        }

        public async Task<ResponseDTO> UpdateTelegramBotAccount(TelegramAccountDTO botShareTeleDTO)
        {
            return await _baseService.BaseSendAsync(new RequestDTO()
            {
                APIType = SD.APIType.PUT,
                Data = botShareTeleDTO,
                Url = $"{_apiEndPointConstant.API_BOT_SHARE_TELE_ENDPOINT}/api/TelegramAccount",
            });
        }

        public async Task<ResponseDTO> SetTelegramHook(Guid botID, string system)
        {
            return await _baseService.BaseSendAsync(new RequestDTO()
            {
                APIType = SD.APIType.POST,
                Url = $"{_apiEndPointConstant.API_BOT_SHARE_TELE_ENDPOINT}/api/TelegramAccount/SetHook?BotID={botID}&System={system}",
            });
        }

        public async Task<ResponseDTO> GetTelegramResponseAsync(QueryParameterDTO ParamDTO)
        {
            return await _baseService.BaseSendAsync(new RequestDTO()
            {
                APIType = SD.APIType.GET,
                Url = $"{_apiEndPointConstant.API_BOT_SHARE_TELE_ENDPOINT}/api/TelegramResponse?{ApiBaseExtension.ToQueryString(ParamDTO)}",
            });
        }

        public async Task<ResponseDTO> AddNewTelegramResponse(TelegramResponseDTO telegramResponseDTO)
        {
            return await _baseService.BaseSendAsync(new RequestDTO()
            {
                APIType = SD.APIType.POST,
                Data = telegramResponseDTO,
                Url = $"{_apiEndPointConstant.API_BOT_SHARE_TELE_ENDPOINT}/api/TelegramResponse",
            });
        }

        public async Task<ResponseDTO> GetTelegramResponseByIDAsync(Guid ID)
        {
            return await _baseService.BaseSendAsync(new RequestDTO()
            {
                APIType = SD.APIType.GET,
                Url = $"{_apiEndPointConstant.API_BOT_SHARE_TELE_ENDPOINT}/api/TelegramResponse/GetByID?ID={ID}",
            });
        }

        public async Task<ResponseDTO> UpdateTelegramResponse(TelegramResponseDTO responseDTO)
        {
            return await _baseService.BaseSendAsync(new RequestDTO()
            {
                APIType = SD.APIType.PUT,
                Data = responseDTO,
                Url = $"{_apiEndPointConstant.API_BOT_SHARE_TELE_ENDPOINT}/api/TelegramResponse",
            });
        }

        public async Task<ResponseDTO> GetTeleCampaignByBotIDAsync(Guid BotID)
        {
            return await _baseService.BaseSendAsync(new RequestDTO()
            {
                APIType = SD.APIType.GET,
                Url = $"{_apiEndPointConstant.API_BOT_SHARE_TELE_ENDPOINT}/api/TelegramCampaign/GetByBotID?BotID={BotID}",
            });
        }
    }

}