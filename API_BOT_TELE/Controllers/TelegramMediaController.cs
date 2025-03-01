using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using TELEBOT_CSKH.Constant;
using TELEBOT_CSKH.Data;
using TELEBOT_CSKH.Models.DTO;
using TELEBOT_CSKH.Models.TELEGRAM_BOT;
using TELEBOT_CSKH.Services;
using TELEBOT_CSKH.Services.MediaServices;
using TELEBOT_CSKH.Services.Telegram;

namespace TELEBOT_CSKH.Controllers
{
    [Route("api/TelegramMedia")]
    [ApiController]
    public class TelegramMediaController
    {
        private AppDbContext _appDbContext;
        private readonly ITelegramService _telegramServices;
        private readonly IImageUploadService _imageUpdateServices;
        private readonly IBOServices _boServices;
        private static TelegramAccount _telegramBotAccount;
        private ResponseDTO _responseDTO;

        public TelegramMediaController(AppDbContext dbContext, ITelegramService telegramServices,IImageUploadService imageUploadServices, IBOServices boServices)
        {
            Console.WriteLine("TelegramController");
            _appDbContext = dbContext;
            _telegramServices = telegramServices;
            _imageUpdateServices = imageUploadServices;
            _boServices = boServices;
            _responseDTO = new ResponseDTO();
            
        }


        
        [HttpPost]
        [Route("BotUploadMedia/{Site}/{Bot}")]
        public async Task<ResponseDTO> BotUploadMedia(string Site, string Bot, [FromBody] string ImageURLBase64)
        {
            //try
            //{
            //    Guid TelegramID = Guid.Parse("fcbddf3e-b5b5-4749-b4a0-583e834da729");

            //    if (SD.fBotCheckAgentUpdate == false || _telegramBotAccount == null)
            //    {
            //        _telegramBotAccount = await _appDbContext.TelegramAccount.Where(x => x.BotType == 2 && x.System==Site && x.ID== TelegramID).FirstOrDefaultAsync();
            //        SD.fBotCheckAgentUpdate = true;
            //    }

            //    if (Site.IsNullOrEmpty() || Bot.IsNullOrEmpty() || ImageURLBase64.IsNullOrEmpty())
            //    {
            //        _responseDTO.IsSuccess = false;
            //        _responseDTO.Message = "#1 QUERY ERROR";
            //        return _responseDTO;
            //    }
            //    Console.WriteLine("BotUploadMedia");

            //    string uploadImageURL = await _imageUpdateServices.UploadImageBase64Async(ImageURLBase64, Site);
            //    if (string.IsNullOrEmpty(uploadImageURL))
            //    {
            //        _responseDTO.IsSuccess = false;
            //        _responseDTO.Message = "Quá trình tải ảnh thất bại. Quý khách vui lòng thực hiện lại";
            //        return _responseDTO;
            //    }
            //    uploadImageURL = "https://cskh-auto-api.attcloud.org/UploadImage/F168/30dec12c-ecde-4e4f-9710-ffb768396168.jpg";
                
            //    if (_telegramBotAccount == null)
            //    {
            //        _responseDTO.IsSuccess = false;
            //        _responseDTO.Message = "NO TELEGRAM ACCOUNT FOUND";
            //        return _responseDTO;
            //    }


            //    var messageResponse = await _telegramServices.SendPhotoMessageAsync(_telegramBotAccount.Token, long.Parse(_telegramBotAccount.ChatID), uploadImageURL,"");
            //    Console.WriteLine(JsonConvert.SerializeObject(messageResponse,Formatting.Indented));
            //    if (messageResponse==null || messageResponse.IsSuccess==false)
            //    {
            //        _responseDTO.IsSuccess = false;
            //        _responseDTO.Message = "Failed to send image to Telegram group";
            //        return _responseDTO;
            //    }

            //    dynamic sendPhotoObj = JsonConvert.DeserializeObject<dynamic>(messageResponse.Result.ToString());
            //    var sendPhotoFileIDList = sendPhotoObj["result"]["photo"];
            //    string FileID= sendPhotoFileIDList[sendPhotoFileIDList.Count - 1]["file_id"].ToString();


            //    messageResponse = await _telegramServices.GetImageURLFromFileIDMessageAsync(_telegramBotAccount.Token, FileID);
            //    Console.WriteLine(JsonConvert.SerializeObject(messageResponse, Formatting.Indented));
            //    if (messageResponse == null || messageResponse.IsSuccess == false)
            //    {
            //        _responseDTO.IsSuccess = false;
            //        _responseDTO.Message = "Failed to get image to Telegram group";
            //        return _responseDTO;
            //    }

            //    _responseDTO.IsSuccess = true;
            //    _responseDTO.Message = "Image uploaded and sent to Telegram group successfully";
            //    _responseDTO.Result = FileID;
            //}
            //catch (Exception ex)
            //{
            //    _responseDTO.IsSuccess = false;
            //    _responseDTO.Message = ex.Message;
            //}

            return _responseDTO;
        }

    }
}
