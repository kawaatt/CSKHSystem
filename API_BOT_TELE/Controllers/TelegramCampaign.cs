using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using TELEBOT_CSKH.Data;
using TELEBOT_CSKH.Models.DTO;
using TELEBOT_CSKH.Models.TELEGRAM_BOT;
using TELEBOT_CSKH.Services.Telegram;

namespace TELEBOT_CSKH.Controllers
{
    //[Authorize]
    [Route("api/TelegramCampaign")]
    [ApiController]
    public class TelegramCampaignController : ControllerBase
    {
        public ResponseDTO _responseDTO;
        public readonly AppDbContext _dbContext;
        public readonly ITelegramService _telegramServices;
        public readonly IMapper _mapper;

        public TelegramCampaignController(AppDbContext dbContext, IMapper mapper, ITelegramService telegramServices)
        {
            _responseDTO = new ResponseDTO();
            _dbContext = dbContext;
            _mapper = mapper;
            _telegramServices = telegramServices;
        }

        [HttpGet]
        [Route("GetByBotID")]
        public async Task<ResponseDTO> GetByID(Guid BotID)
        {
            TelegramCampaign telegramCampaign = await _dbContext.TelegramCampaign.Where(x => x.ID == BotID).FirstOrDefaultAsync();
            if (telegramCampaign == null)
            {
                _responseDTO.IsSuccess = false;
                _responseDTO.Message = "DATA NOT EXISTED";
            }
            _responseDTO.Result = telegramCampaign;
            return _responseDTO;
        }


        [HttpPost]
        public async Task<ResponseDTO> PostAsync(TelegramCampaignDTO telegramCampaignDTO)
        {
            try
            {
                Console.WriteLine("PostAsync");
                TelegramCampaign _telegramCampaign = await _dbContext.TelegramCampaign.AsNoTracking().Where(x => x.IDBot == telegramCampaignDTO.IDBot).FirstOrDefaultAsync();
                if (_telegramCampaign != null)
                {
                    _responseDTO.IsSuccess = false;
                    _responseDTO.Message = "DATA EXISTED";
                    return _responseDTO;
                }
                _telegramCampaign = _mapper.Map<TelegramCampaign>(telegramCampaignDTO);
                _telegramCampaign.ID = Guid.NewGuid();
                
                _dbContext.TelegramCampaign.Add(_telegramCampaign);
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
        public async Task<ResponseDTO> PutAsync(TelegramCampaignDTO telegramCampaignDTO)
        {
            try
            {
                Console.WriteLine("PutAsync");
                Console.WriteLine(JsonConvert.SerializeObject(telegramCampaignDTO));

                TelegramCampaign _telegramCampaign = await _dbContext.TelegramCampaign.AsNoTracking().Where(x => x.ID == telegramCampaignDTO.ID).FirstOrDefaultAsync();
                if (_telegramCampaign == null)
                {
                    _responseDTO.IsSuccess = false;
                    _responseDTO.Message = "NO DATA FOUND";
                    return _responseDTO;
                }

                TelegramCampaign updateObject = _mapper.Map<TelegramCampaign>(telegramCampaignDTO);

                _dbContext.TelegramCampaign.Update(updateObject);
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
                TelegramCampaign _telegramCampaign = await _dbContext.TelegramCampaign.Where(x => x.ID == ID).FirstOrDefaultAsync();
                if (_telegramCampaign == null)
                {
                    _responseDTO.IsSuccess = false;
                    _responseDTO.Message = "DATA NOT FOUND";
                    return _responseDTO;
                }
                _dbContext.TelegramCampaign.Remove(_telegramCampaign);
                await _dbContext.SaveChangesAsync();
                _responseDTO.IsSuccess = true;
                _responseDTO.Message = "DATA DELETED SUCCESSFULLY";
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
