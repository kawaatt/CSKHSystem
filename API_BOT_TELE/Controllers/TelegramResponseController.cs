using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using TELEBOT_CSKH.Constant;
using TELEBOT_CSKH.Data;
using TELEBOT_CSKH.Models.DTO;
using TELEBOT_CSKH.Models.TELEGRAM_BOT;
using TELEBOT_CSKH.Services.Telegram;

namespace TELEBOT_CSKH.Controllers
{
    //[Authorize]
    [Route("api/TelegramResponse")]
    [ApiController]
    public class TelegramResponseController : ControllerBase
    {
        public ResponseDTO _responseDTO;
        public readonly AppDbContext _dbContext;
        public readonly ITelegramService _telegramServices;
        public readonly IMapper _mapper;

        public TelegramResponseController(AppDbContext dbContext, IMapper mapper, ITelegramService telegramServices)
        {
            _responseDTO = new ResponseDTO();
            _dbContext = dbContext;
            _mapper = mapper;
            _telegramServices = telegramServices;
        }

        [HttpGet]        
        public async Task<ResponseDTO> GetAsync([FromQuery] QueryParameterDTO ParamDTO)
        {
            try
            {
                if (ParamDTO.System.IsNullOrEmpty())
                {
                    _responseDTO.IsSuccess = false;
                    _responseDTO.Message = "#1 QUERY ERROR";
                    return _responseDTO;
                }

                var MainQuery = _dbContext.TelegramResponse.Where(x => x.BotID == Guid.Parse(ParamDTO.System)).AsQueryable();
                if (!string.IsNullOrEmpty(ParamDTO.SearchText))
                {
                    MainQuery = MainQuery.Where(w => w.RequestCode.Contains(ParamDTO.SearchText)
                                    || w.Content.Contains(ParamDTO.SearchText)
                                    || w.URLImage.Contains(ParamDTO.SearchText)
                                    || w.InlineKeyboard.Contains(ParamDTO.SearchText));
                }

                if (ParamDTO.StartTime != null)
                {
                    MainQuery = MainQuery.Where(w => w.CreateDate >= ParamDTO.StartTime);
                }

                if (ParamDTO.EndTime != null)
                {
                    MainQuery = MainQuery.Where(w => w.CreateDate <= ParamDTO.EndTime);
                }

                bool isOrderAsc = true;
                if (ParamDTO.SortDirection == "desc")
                {
                    isOrderAsc = false;
                }

                if (!string.IsNullOrEmpty(ParamDTO.SortBy))
                {
                    switch (ParamDTO.SortBy)
                    {
                        case "createDate":
                            if (isOrderAsc)
                            {
                                MainQuery = MainQuery.OrderBy(w => w.CreateDate);
                            }
                            else
                            {
                                MainQuery = MainQuery.OrderByDescending(w => w.CreateDate);
                            }
                            break;
                        default:
                            break;
                    }
                }

                List<TelegramResponse> TelegramResponseList = await MainQuery
                                        .Skip((ParamDTO.PageIndex - 1) * ParamDTO.PageSize)
                                        .Take(ParamDTO.PageSize)
                                        .ToListAsync();
                _responseDTO.TotalCount = MainQuery.Count();

                if (TelegramResponseList.Count > 0)
                {
                    _responseDTO.Result = _mapper.Map<List<TelegramResponseDTO>>(TelegramResponseList);
                }
                else
                {
                    _responseDTO.Message = "NO DATA FOUND";
                    _responseDTO.IsSuccess = false;
                }
            }
            catch (Exception ex)
            {
                _responseDTO.IsSuccess = false;
                _responseDTO.Message = ex.Message;
            }
            return _responseDTO;
        }

        [HttpGet]
        [Route("GetByID")]
        public async Task<ResponseDTO> GetByID(Guid ID)
        {
            Console.WriteLine("GetByID");
            TelegramResponse telegramResponses = await _dbContext.TelegramResponse.Where(x => x.ID==ID).FirstOrDefaultAsync();
            if (telegramResponses == null)
            {
                _responseDTO.IsSuccess = false;
                _responseDTO.Message = "DATA NOT EXISTED";
            }
            _responseDTO.Result = telegramResponses;
            return _responseDTO;
        }

        [HttpPost]
        public async Task<ResponseDTO> PostAsync(TelegramResponseDTO telegramResponseDTO)
        {
            try
            {
                Console.WriteLine("PostAsync");
                TelegramResponse _telegramResponse = await _dbContext.TelegramResponse.AsNoTracking().Where(x => x.BotID == telegramResponseDTO.BotID && x.RequestCode == telegramResponseDTO.RequestCode).FirstOrDefaultAsync();
                if (_telegramResponse != null)
                {
                    _responseDTO.IsSuccess = false;
                    _responseDTO.Message = "DATA EXISTED";
                    return _responseDTO;
                }
                _telegramResponse = _mapper.Map<TelegramResponse>(telegramResponseDTO);
                _telegramResponse.ID = Guid.NewGuid();
                _telegramResponse.CreateDate = DateTime.Now;

                SD.fTeleResposeDataUpdate = true;

                _dbContext.TelegramResponse.Add(_telegramResponse);
                await _dbContext.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                _responseDTO.Message = ex.Message;
                _responseDTO.IsSuccess = false;
            }
            return _responseDTO;
        }

        [HttpPut]
        public async Task<ResponseDTO> PutAsync(TelegramResponseDTO telegramResponseDTO)
        {
            try
            {
                Console.WriteLine("PutAsync");
                Console.WriteLine(JsonConvert.SerializeObject(telegramResponseDTO));

                TelegramResponse _telegramResponse = await _dbContext.TelegramResponse.AsNoTracking().Where(x => x.BotID == telegramResponseDTO.BotID && x.RequestCode == telegramResponseDTO.RequestCode).FirstOrDefaultAsync();
                if (_telegramResponse == null)
                {
                    _responseDTO.IsSuccess = false;
                    _responseDTO.Message = "NO DATA FOUND";
                    return _responseDTO;
                }

                TelegramResponse updateObject = _mapper.Map<TelegramResponse>(telegramResponseDTO);

                updateObject.CreateDate = _telegramResponse.CreateDate;
                SD.fTeleResposeDataUpdate = true;

                _dbContext.TelegramResponse.Update(updateObject);
                await _dbContext.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                _responseDTO.Message = ex.Message;
                _responseDTO.IsSuccess = false;
            }
            return _responseDTO;
        }

        [HttpDelete]
        public async Task<ResponseDTO> DeleteAsync(Guid ID)
        {
            try
            {
                Console.WriteLine("DeleteAsync");
                TelegramResponse _telegramResponse = await _dbContext.TelegramResponse.Where(x => x.ID == ID).FirstOrDefaultAsync();
                if (_telegramResponse == null)
                {
                    _responseDTO.IsSuccess = false;
                    _responseDTO.Message = "DATA NOT FOUND";
                    return _responseDTO;
                }
                SD.fTeleResposeDataUpdate = true;
                _dbContext.TelegramResponse.Remove(_telegramResponse);
                await _dbContext.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                _responseDTO.Message = ex.Message;
                _responseDTO.IsSuccess = false;
            }
            return _responseDTO;
        }

        
    }
}
