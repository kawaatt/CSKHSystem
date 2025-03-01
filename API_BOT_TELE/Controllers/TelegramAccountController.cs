using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using TELEBOT_CSKH.Constant;
using TELEBOT_CSKH.Data;
using TELEBOT_CSKH.Models.DTO;
using TELEBOT_CSKH.Models.TELEGRAM_BOT;
using TELEBOT_CSKH.Services.Telegram;

namespace TELEBOT_CSKH.Controllers
{
    //[Authorize]
    [Route("api/TelegramAccount")]
    [ApiController]
    public class TelegramAccountController : ControllerBase
    {
        public ResponseDTO _responseDTO;
        public readonly AppDbContext _dbContext;
        public readonly ITelegramService _telegramServices;
        public readonly IMapper _mapper;

        public TelegramAccountController(AppDbContext dbContext, IMapper mapper, ITelegramService telegramServices)
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

                var MainQuery = _dbContext.TelegramAccount.Where(x => x.System == ParamDTO.System).AsQueryable();
                if (!string.IsNullOrEmpty(ParamDTO.SearchText))
                {
                    MainQuery = MainQuery.Where(w => w.UserName.Contains(ParamDTO.SearchText)
                                    || w.UserName.Contains(ParamDTO.SearchText)
                                    || w.Token.Contains(ParamDTO.SearchText));
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

                List<TelegramAccount> ListTelegramBotAccount = await MainQuery
                                        .Skip((ParamDTO.PageIndex - 1) * ParamDTO.PageSize)
                                        .Take(ParamDTO.PageSize)
                                        .ToListAsync();
                _responseDTO.TotalCount = MainQuery.Count();

                if (ListTelegramBotAccount.Count > 0)
                {
                    _responseDTO.Result = _mapper.Map<List<TelegramAccountDTO>>(ListTelegramBotAccount);
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
        public async Task<ResponseDTO> GetByID(Guid ID,string System)
        {
            Console.WriteLine("GetByID");
            TelegramAccount telegramAccounts = await _dbContext.TelegramAccount.Where(x => x.ID==ID && x.System == System).FirstOrDefaultAsync();
            if (telegramAccounts == null)
            {
                _responseDTO.IsSuccess = false;
                _responseDTO.Message = "DATA NOT EXISTED";
            }
            _responseDTO.Result = telegramAccounts;
            return _responseDTO;
        }

        [HttpPost]
        public async Task<ResponseDTO> PostAsync(TelegramAccountDTO telegramAccountDTO)
        {
            try
            {
                Console.WriteLine("PostAsync");
                TelegramAccount _telegramAccount = await _dbContext.TelegramAccount.Where(x => x.Token == telegramAccountDTO.Token && x.System == telegramAccountDTO.System).FirstOrDefaultAsync();
                if (_telegramAccount!=null)
                {
                    _responseDTO.IsSuccess = false;
                    _responseDTO.Message = "DATA EXISTED";
                    return _responseDTO;
                }
                _telegramAccount = _mapper.Map<TelegramAccount>(telegramAccountDTO);
                _telegramAccount.ID = Guid.NewGuid();
                _telegramAccount.CreateDate = DateTime.Now;

                SD.fBotShareTeleUpdate = false;

                _dbContext.TelegramAccount.Add(_telegramAccount);
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
        public async Task<ResponseDTO> PutAsync(TelegramAccountDTO telegramAccountDTO)
        {
            try
            {
                Console.WriteLine("PutAsync");
                TelegramAccount _telegramAccount = await _dbContext.TelegramAccount.AsNoTracking().Where(x => x.ID == telegramAccountDTO.ID && x.System == telegramAccountDTO.System).FirstOrDefaultAsync();
                if (_telegramAccount == null)
                {
                    _responseDTO.IsSuccess = false;
                    _responseDTO.Message = "DATA NOT FOUND";
                    return _responseDTO;
                }

                if (telegramAccountDTO.UserName != null)
                {
                    _telegramAccount.UserName = telegramAccountDTO.UserName;
                }
                if (telegramAccountDTO.Token != null)
                {
                    _telegramAccount.Token = telegramAccountDTO.Token;
                }
                if (telegramAccountDTO.IsIndividualWorking != null)
                {
                    _telegramAccount.IsIndividualWorking = telegramAccountDTO.IsIndividualWorking;
                }
                if (telegramAccountDTO.ChatID != null)
                {
                    _telegramAccount.ChatID = telegramAccountDTO.ChatID;
                }
                if (telegramAccountDTO.URLHooking != null)
                {
                    _telegramAccount.URLHooking = telegramAccountDTO.URLHooking;
                }
                if (telegramAccountDTO.BotType != null)
                {
                    _telegramAccount.BotType = telegramAccountDTO.BotType;
                }
                if (telegramAccountDTO.Status != null)
                {
                    _telegramAccount.Status = telegramAccountDTO.Status;
                }
                if (telegramAccountDTO.System != null)
                {
                    _telegramAccount.System = telegramAccountDTO.System;
                }
                if (telegramAccountDTO.KeyboardData != null)
                {
                    _telegramAccount.KeyboardData = telegramAccountDTO.KeyboardData;
                }

                SD.fBotShareTeleUpdate = false;
                _dbContext.TelegramAccount.Update(_telegramAccount);
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
        public async Task<ResponseDTO> DeleteAsync(Guid ID, string System)
        {
            try
            {
                Console.WriteLine("DeleteAsync");
                TelegramAccount _telegramAccount = await _dbContext.TelegramAccount.Where(x => x.ID == ID && x.System == System).FirstOrDefaultAsync();
                if (_telegramAccount == null)
                {
                    _responseDTO.IsSuccess = false;
                    _responseDTO.Message = "DATA NOT FOUND";
                    return _responseDTO;
                }
                SD.fBotShareTeleUpdate = false;
                _dbContext.TelegramAccount.Remove(_telegramAccount);
                await _dbContext.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                _responseDTO.Message = ex.Message;
                _responseDTO.IsSuccess = false;
            }
            return _responseDTO;
        }

        [HttpPost]
        [Route("SetHook")]
        public async Task<ResponseDTO> SetHook([FromQuery] Guid BotID, [FromQuery] string System)
        {
            try
            {
                Console.WriteLine("SetHook");
                TelegramAccount _telegramAccount = await _dbContext.TelegramAccount.Where(x => x.ID == BotID && x.System == System).FirstOrDefaultAsync();
                if (_telegramAccount == null)
                {
                    _responseDTO.IsSuccess = false;
                    _responseDTO.Message = "DATA NOT FOUND";
                    return _responseDTO;
                }

                ResponseDTO _activeHookRes = await _telegramServices.SetWebhookAsync(_telegramAccount);
                if (_activeHookRes==null || _activeHookRes.IsSuccess==false)
                {
                    _responseDTO.IsSuccess = false;
                    _responseDTO.Message = "SETTING HOOK FAILED";
                    return _responseDTO;
                }

                SD.fBotShareTeleUpdate = false;
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
