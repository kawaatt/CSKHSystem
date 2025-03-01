using static TELEBOT_CSKH.Constant.SD;
using TELEBOT_CSKH.Models.DTO;
using TELEBOT_CSKH.Models.TELEGRAM_BOT;
using Newtonsoft.Json;
using System.Net.Http;
using System.Text.RegularExpressions;
using System.Text;
using TELEBOT_CSKH.Data;
using Microsoft.EntityFrameworkCore;
using TELEBOT_CSKH.Constant;
using Telegram.Bot.Types;
using System;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;
using System.Text.Json.Nodes;

namespace TELEBOT_CSKH.Services.Telegram
{
    public interface ITelegramService
    {
        Task<ResponseDTO> SetWebhookAsync(TelegramAccount TelegramAccount);
        Task<ResponseDTO> SendTelegramKeyboardAsync(string Token, long ChatID, string Text, object InlineKeyboard);
        Task<ResponseDTO> SendTelegramMessageAsync(string Token, long ChatID, string UploadImageURL, string Caption, object? KeyBoard);



        //Task<ResponseDTO> SendDirectMessageAsync(string Token, long ChatID, string Message);
        //Task<ResponseDTO> ReplyDirectMessageAsync(string Token, long ChatID, long MessageID, string Message);
        

        //Task<ResponseDTO> SendMessageAsync(TelegramCustomer _telegramCustomer, string message);
        //Task<ResponseDTO> SendDefaultKeyboardMessageAsync(TelegramCustomer _telegramCustomer, string MessageText);
        //Task<ResponseDTO> SendDefaultContentMessageAsync(TelegramCustomer _telegramCustomer, string Site, Guid BotID);
        //Task<ResponseDTO> SendLinkAFFAsync(TelegramCustomer telegramCustomer, string site, Guid botID);
        //Task<ResponseDTO> SendCodeAsync(TelegramCustomer telegramCustomer, string site, Guid botID);

        //Bot Share Telegram
        

        //Bot Upload Media
        
        //Task<ResponseDTO> GetImageURLFromFileIDMessageAsync(string Token, string FileID);
    }

    public class TelegramService : ITelegramService
    {
        private readonly IBaseServices _baseServices;
        private readonly AppDbContext _dbContext;
        private readonly IWebHostEnvironment _env;

        private ResponseDTO _responseDTO;

        public TelegramService(IBaseServices baseServices, AppDbContext dbContext, IWebHostEnvironment env)
        {
            _baseServices = baseServices;
            _responseDTO = new ResponseDTO();
            _dbContext = dbContext;
            _env = env;
        }

        public async Task<ResponseDTO?> SetWebhookAsync(TelegramAccount TelegramAccount)
        {
            RequestDTO _remoteHookRequestDTO = new RequestDTO()
            {
                APIType = SD.APIType.POST,
                Url = $"https://api.telegram.org/bot{TelegramAccount.Token}/setWebhook?remove"
            };
            ResponseDTO _removeHookRes = await _baseServices.LiveSendAsync(_remoteHookRequestDTO);
            if (_removeHookRes == null || _removeHookRes.IsSuccess == false)
            {
                _responseDTO.IsSuccess = false;
                _responseDTO.Message = "REMOVE HOOK FAILED";
                return _responseDTO;
            }

            var payload = new { url = TelegramAccount.URLHooking };
            RequestDTO _settingHookRequestDTO = new RequestDTO()
            {
                APIType = SD.APIType.POST,
                Data = payload,
                Url = $"https://api.telegram.org/bot{TelegramAccount.Token}/setWebhook"
            };

            ResponseDTO _settingHookResponseDTO = await _baseServices.LiveSendAsync(_settingHookRequestDTO);
            if (_settingHookResponseDTO == null || _settingHookResponseDTO.IsSuccess == false)
            {
                _responseDTO.IsSuccess = false;
                _responseDTO.Message = "SETTING HOOK FAILED";
                return _responseDTO;
            }
            Console.WriteLine(JsonConvert.SerializeObject(_settingHookResponseDTO));
            Console.WriteLine("END");

            return _responseDTO;
        }


        //public async Task<ResponseDTO> SendDefaultKeyboardMessageAsync(TelegramCustomer _telegramCustomer, string MessageText)
        //{
        //    try
        //    {
        //        Console.WriteLine("SendDefaultKeyboardMessageAsync");
        //        TelegramAccount _telegramAccount = await _dbContext.TelegramAccount.FirstOrDefaultAsync(x => x.ID == _telegramCustomer.BotID);

        //        long ChatID = long.Parse(_telegramCustomer.TelegramID);

        //        var replyMarkup = new
        //        {
        //            keyboard = new[]
        //            {
        //                new[]
        //                {
        //                    new { text = "Link của bạn 🔗"},
        //                    new { text = "Nhận thưởng 🎁"}
        //                }
        //            },
        //            resize_keyboard = true,
        //            one_time_keyboard = true
        //        };

        //        RequestDTO requestDTO = new RequestDTO()
        //        {
        //            APIType = SD.APIType.POST,
        //            Url = $"https://api.telegram.org/bot{_telegramAccount.Token}/sendMessage",
        //            Data = new
        //            {
        //                chat_id = ChatID,
        //                text = MessageText,
        //                reply_markup = replyMarkup
        //            }
        //        };

        //        ResponseDTO sendMessageRes = await _baseServices.LiveSendAsync(requestDTO);
        //        if (sendMessageRes == null || sendMessageRes.IsSuccess == false)
        //        {
        //            _responseDTO.IsSuccess = false;
        //            _responseDTO.Message = "SEND MESSAGE FAILED";
        //            return _responseDTO;
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        _responseDTO.IsSuccess = false;
        //        _responseDTO.Message = ex.Message;
        //    }
        //    return _responseDTO;
        //}

        public async Task<ResponseDTO> SendTelegramMessageAsync(string Token, long ChatID, string UploadImageURL, string Caption, object? KeyBoard)
        {
            Console.WriteLine("SendTelegramMessageAsync");
            RequestDTO requestDTO = new RequestDTO();
            requestDTO.APIType = APIType.POST;
            
            if (UploadImageURL!=null)
            {
                requestDTO.Url = $"https://api.telegram.org/bot{Token}/sendPhoto";
                if (KeyBoard!=null)
                {
                    requestDTO.Data = new
                    {
                        chat_id = ChatID,
                        photo = UploadImageURL,
                        caption = Caption,
                        parse_mode = "HTML",
                        reply_markup = KeyBoard
                    };
                }
                else
                {
                    requestDTO.Data = new
                    {
                        chat_id = ChatID,
                        photo = UploadImageURL,
                        caption = Caption,
                        parse_mode = "HTML"
                    };
                }
            }
            else
            {
                requestDTO.Url = $"https://api.telegram.org/bot{Token}/sendMessage";
                if (KeyBoard != null)
                {
                    requestDTO.Data = new
                    {
                        chat_id = ChatID,
                        text = Caption,
                        parse_mode = "HTML",
                        reply_markup = KeyBoard
                    };
                }
                else
                {
                    requestDTO.Data = new
                    {
                        chat_id = ChatID,
                        text = Caption,
                        parse_mode = "HTML"
                    };
                }
            }
            Console.WriteLine(JsonConvert.SerializeObject(requestDTO,Formatting.Indented));
            ResponseDTO sendMessageRes = await _baseServices.LiveSendAsync(requestDTO);
            if (sendMessageRes == null || sendMessageRes.IsSuccess == false)
            {
                _responseDTO.IsSuccess = false;
                _responseDTO.Message = "SEND PHOTO MESSAGE FAILED";
                return _responseDTO;
            }
            return sendMessageRes;
        }

        public async Task<ResponseDTO> SendTelegramKeyboardAsync(string Token, long ChatID, string Text, object InlineKeyboard)
        {
            try
            {
                RequestDTO requestDTO = new RequestDTO()
                {
                    APIType = SD.APIType.POST,
                    Url = $"https://api.telegram.org/bot{Token}/sendMessage",
                    Data = new
                    {
                        chat_id = ChatID,
                        text = Text,
                        reply_markup = InlineKeyboard
                    },
                };

                ResponseDTO sendMessageRes = await _baseServices.LiveSendAsync(requestDTO);
                if (sendMessageRes == null || sendMessageRes.IsSuccess == false)
                {
                    _responseDTO.IsSuccess = false;
                    _responseDTO.Message = "SEND MESSAGE FAILED";
                    return _responseDTO;
                }
            }
            catch (Exception ex)
            {
                _responseDTO.IsSuccess = false;
                _responseDTO.Message = ex.Message;
            }
            return _responseDTO;
        }


        //public async Task<ResponseDTO> SendDefaultKeyboardMessageAsync(TelegramCustomer _telegramCustomer,string MessageText)
        //{
        //    try
        //    {
        //        Console.WriteLine("SendDefaultKeyboardMessageAsync");
        //        TelegramAccount _telegramAccount = await _dbContext.TelegramAccount.FirstOrDefaultAsync(x => x.ID == _telegramCustomer.BotID);

        //        long ChatID = long.Parse(_telegramCustomer.TelegramID);

        //        var replyMarkup = new
        //        {
        //            keyboard = new[]
        //            {
        //                new[]
        //                {
        //                    new { text = "Link của bạn 🔗"},
        //                    new { text = "Nhận thưởng 🎁"}
        //                }
        //            },
        //            resize_keyboard = true,
        //            one_time_keyboard = true
        //        };

        //        RequestDTO requestDTO = new RequestDTO()
        //        {
        //            APIType = SD.APIType.POST,
        //            Url = $"https://api.telegram.org/bot{_telegramAccount.Token}/sendMessage",
        //            Data = new
        //            {
        //                chat_id = ChatID,
        //                text = MessageText,
        //                reply_markup = replyMarkup
        //            }
        //        };

        //        ResponseDTO sendMessageRes = await _baseServices.LiveSendAsync(requestDTO);
        //        if (sendMessageRes == null || sendMessageRes.IsSuccess == false)
        //        {
        //            _responseDTO.IsSuccess = false;
        //            _responseDTO.Message = "SEND MESSAGE FAILED";
        //            return _responseDTO;
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        _responseDTO.IsSuccess = false;
        //        _responseDTO.Message = ex.Message;
        //    }
        //    return _responseDTO;
        //}

        //public async Task<ResponseDTO> SendMessageAsync(TelegramCustomer _telegramCustomer, string message)
        //{
        //    try
        //    {
        //        Console.WriteLine("SendMessageAsync");
        //        TelegramAccount _telegramAccount = await _dbContext.TelegramAccount.FirstOrDefaultAsync(x => x.ID == _telegramCustomer.BotID);

        //        long ChatID = long.Parse(_telegramCustomer.TelegramID);

        //        var replyMarkup = new
        //        {
        //            keyboard = new[]
        //            {
        //                new[]
        //                {
        //                    new { text = "Link của bạn 🔗"},
        //                    new { text = "Nhận thưởng 🎁"}
        //                }
        //            },
        //            resize_keyboard = true,
        //            one_time_keyboard = true
        //        };

        //        RequestDTO requestDTO = new RequestDTO()
        //        {
        //            APIType = SD.APIType.POST,
        //            Url = $"https://api.telegram.org/bot{_telegramAccount.Token}/sendMessage",
        //            Data = new
        //            {
        //                chat_id = ChatID,
        //                text = message,
        //                reply_markup = replyMarkup
        //            }
        //        };

        //        Console.WriteLine(JsonConvert.SerializeObject(requestDTO));

        //        ResponseDTO sendMessageRes = await _baseServices.LiveSendAsync(requestDTO);
        //        Console.WriteLine(JsonConvert.SerializeObject(sendMessageRes));
        //        if (sendMessageRes == null || sendMessageRes.IsSuccess == false)
        //        {
        //            _responseDTO.IsSuccess = false;
        //            _responseDTO.Message = "SEND MESSAGE FAILED";
        //            return _responseDTO;
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        _responseDTO.IsSuccess = false;
        //        _responseDTO.Message = ex.Message;
        //    }
        //    return _responseDTO;
        //}

        //public async Task<ResponseDTO> SendDefaultContentMessageAsync(TelegramCustomer _telegramCustomer, string Site, Guid BotID)
        //{
        //    try
        //    {
        //        TelegramAccount _telegramAccount = await _dbContext.TelegramAccount.FirstOrDefaultAsync(x => x.ID == _telegramCustomer.BotID);

        //        StringBuilder sb = new StringBuilder();
        //        sb.AppendLine($"🎁 CHIA SẺ LINK - RINH GIFTCODE {Site}! 🎁");
        //        sb.AppendLine("Bạn đã sẵn sàng nhận quà chưa?\n");
        //        sb.AppendLine("📌 Cách tham gia cực dễ:");
        //        sb.AppendLine("✅ Lấy link giới thiệu của bạn ở menu bot.");
        //        sb.AppendLine("✅ Chia sẻ cho bạn bè, người thân.");
        //        sb.AppendLine("✅ Khi đủ 15 người tham gia qua link, bạn sẽ nhận ngay 1 GIFTCODE giá trị!\n");
        //        sb.AppendLine("🔥 Chia sẻ càng nhiều – Càng có cơ hội nhận quà giá trị! 🔥");
        //        sb.AppendLine("Nhanh tay tham gia ngay trước khi chương trình kết thúc! ⏳✨\n");
        //        sb.AppendLine($"Đây là link chia sẻ của bạn! Hãy nhanh tay chia sẻ nó với bạn bè và người thân để nhận phần thưởng hấp dẫn ngay hôm nay. Đừng bỏ lỡ cơ hội này!\n");
        //        sb.AppendLine($"<a href=\"https://t.me/{_telegramAccount.UserName.Substring(1)}?start=aff{_telegramCustomer.TelegramID}{Site}\">https://t.me/{_telegramAccount.UserName.Substring(1)}?start=aff{_telegramCustomer.TelegramID}{Site}</a>");

        //        var replyMarkup = new
        //        {
        //            inline_keyboard = new[]
        //            {
        //                new[]
        //                {
        //                    new { text = "Link của bạn 🔗", callback_data = "GetAFFLink" },
        //                    new { text = "Nhận thưởng 🎁", callback_data = "GetAFFCode" }
        //                }
        //            }
        //        };

        //        string LinkWelcomImage = "https://attcloud1.work/bot/sharetele";
        //        switch (Site)
        //        {
        //            case "F168":
        //                LinkWelcomImage = "https://f1689.online/bot/sharetele";
        //                break;
        //            default:
        //                break;
        //        }

        //        RequestDTO requestDTO = new RequestDTO()
        //        {
        //            APIType = SD.APIType.POST,
        //            Url = $"https://api.telegram.org/bot{_telegramAccount.Token}/sendPhoto",
        //            Data = new
        //            {
        //                chat_id = long.Parse(_telegramCustomer.TelegramID),
        //                photo = LinkWelcomImage,
        //                caption = sb.ToString(),
        //                reply_markup = replyMarkup,
        //                parse_mode = "HTML"
        //            }
        //        };



        //        Console.WriteLine(JsonConvert.SerializeObject(requestDTO));

        //        ResponseDTO sendMessageRes = await _baseServices.LiveSendAsync(requestDTO);
        //        Console.WriteLine(JsonConvert.SerializeObject(sendMessageRes));
        //        if (sendMessageRes == null || sendMessageRes.IsSuccess == false)
        //        {
        //            _responseDTO.IsSuccess = false;
        //            _responseDTO.Message = "SEND MESSAGE FAILED";
        //            return _responseDTO;
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        _responseDTO.IsSuccess = false;
        //        _responseDTO.Message = ex.Message;
        //    }
        //    return _responseDTO;
        //}




        

        //public async Task<ResponseDTO> SendCodeAsync(TelegramCustomer TelegramCustomer, string Site, Guid BotID)
        //{
        //    try
        //    {
        //        TelegramAccount _telegramAccount = await _dbContext.TelegramAccount.FirstOrDefaultAsync(x => x.ID == TelegramCustomer.BotID);

        //        //StringBuilder sb = new StringBuilder();
        //        //sb.AppendLine($"🎁 CHIA SẺ LINK - RINH GIFTCODE {Site}! 🎁");
        //        //sb.AppendLine("Bạn đã sẵn sàng nhận quà chưa?\n");
        //        //sb.AppendLine("📌 Cách tham gia cực dễ:");
        //        //sb.AppendLine("✅ Lấy link giới thiệu của bạn ở menu bot.");
        //        //sb.AppendLine("✅ Chia sẻ cho bạn bè, người thân.");
        //        //sb.AppendLine("✅ Khi đủ 15 người tham gia qua link, bạn sẽ nhận ngay 1 GIFTCODE giá trị!\n");
        //        //sb.AppendLine("🔥 Chia sẻ càng nhiều – Càng có cơ hội nhận quà giá trị! 🔥");
        //        //sb.AppendLine("Nhanh tay tham gia ngay trước khi chương trình kết thúc! ⏳✨\n");
        //        //sb.AppendLine($"Đây là link chia sẻ của bạn! Hãy nhanh tay chia sẻ nó với bạn bè và người thân để nhận phần thưởng hấp dẫn ngay hôm nay. Đừng bỏ lỡ cơ hội này!\n");
        //        //sb.AppendLine($"<a href=\"https://t.me/{_telegramAccount.UserName.Substring(1)}?start=aff{TelegramCustomer.TelegramID}{Site}\">https://t.me/{_telegramAccount.UserName.Substring(1)}?start=aff{TelegramCustomer.TelegramID}{Site}</a>");


        //        StringBuilder sb = new StringBuilder();
        //        sb.AppendLine($"✨🔥 KẾT QUẢ CHIA SẺ LINK - RINH GIFTCODE {Site}! 🔥✨\n");
        //        if (TelegramCustomer.ShareCount>=15)
        //        {
        //            sb.AppendLine($"✅ Tài khoản của bạn đã chia sẻ {TelegramCustomer.ShareCount}/15 lượt. Quý khách vui lòng liên hệ bộ CSKH để nhận thưởng! ⏳✨\n");
        //        }
        //        else
        //        {
        //            sb.AppendLine($"Tài khoản của bạn đã chia sẻ {TelegramCustomer.ShareCount}/15 lượt. Hãy nhanh tay hoàn thành ngay trước khi chương trình kết thúc! ⏳✨\n");
        //        }    
        //        sb.AppendLine("🎉 Cảm ơn tất cả các bạn đã tham gia và đóng góp, sự ủng hộ của các bạn là động lực to lớn cho chúng tôi! 💖");

        //        var replyMarkup = new
        //        {
        //            inline_keyboard = new[]
        //            {
        //                new[]
        //                {
        //                    new { text = "Link của bạn 🔗", callback_data = "GetAFFLink" },
        //                    new { text = "Nhận thưởng 🎁", callback_data = "GetAFFCode" }
        //                }
        //            }
        //        };

        //        RequestDTO requestDTO = new RequestDTO()
        //        {
        //            APIType = SD.APIType.POST,
        //            Url = $"https://api.telegram.org/bot{_telegramAccount.Token}/sendMessage",
        //            Data = new
        //            {
        //                chat_id = long.Parse(TelegramCustomer.TelegramID),
        //                text = sb.ToString(),
        //                reply_markup = replyMarkup,
        //                parse_mode = "HTML"
        //            }
        //        };


        //        ResponseDTO sendMessageRes = await _baseServices.LiveSendAsync(requestDTO);
        //        Console.WriteLine(JsonConvert.SerializeObject(sendMessageRes));
        //        if (sendMessageRes == null || sendMessageRes.IsSuccess == false)
        //        {
        //            _responseDTO.IsSuccess = false;
        //            _responseDTO.Message = "SEND MESSAGE FAILED";
        //            return _responseDTO;
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        _responseDTO.IsSuccess = false;
        //        _responseDTO.Message = ex.Message;
        //    }
        //    return _responseDTO;
        //}

        //public async Task<ResponseDTO> SendDirectMessageAsync(string Token, long ChatID, string Message)
        //{

        //    RequestDTO requestDTO = new RequestDTO()
        //    {
        //        APIType = SD.APIType.POST,
        //        Url = $"https://api.telegram.org/bot{Token}/sendMessage",
        //        Data = new
        //        {
        //            chat_id = ChatID,
        //            text = $"<code>{Message}</code>",
        //            //reply_markup = replyMarkup,
        //            parse_mode = "HTML"
        //        }
        //    };

        //    ResponseDTO sendMessageRes = await _baseServices.LiveSendAsync(requestDTO);
        //    if (sendMessageRes == null || sendMessageRes.IsSuccess == false)
        //    {
        //        _responseDTO.IsSuccess = false;
        //        _responseDTO.Message = "SEND MESSAGE FAILED";
        //    }
        //    return _responseDTO;
        //}

        //public async Task<ResponseDTO> ReplyDirectMessageAsync(string Token, long ChatID, long MessageID, string Message)
        //{
        //    RequestDTO requestDTO = new RequestDTO()
        //    {
        //        APIType = SD.APIType.POST,
        //        Url = $"https://api.telegram.org/bot{Token}/sendMessage",
        //        Data = new
        //        {
        //            chat_id = ChatID,
        //            text = $"<code>{Message}</code>",
        //            reply_to_message_id = MessageID,
        //            //reply_markup = replyMarkup,
        //            parse_mode = "HTML"
        //        }
        //    };

        //    ResponseDTO sendMessageRes = await _baseServices.LiveSendAsync(requestDTO);
        //    if (sendMessageRes == null || sendMessageRes.IsSuccess == false)
        //    {
        //        _responseDTO.IsSuccess = false;
        //        _responseDTO.Message = "SEND MESSAGE FAILED";
        //    }
        //    return _responseDTO;
        //}



        //public async Task<ResponseDTO> GetImageURLFromFileIDMessageAsync(string Token, string FileID)
        //{
        //    RequestDTO requestDTO = new RequestDTO()
        //    {
        //        APIType = SD.APIType.GET,
        //        Url = $"https://api.telegram.org/bot{Token}/getFile?file_id={FileID}",
        //    };

        //    ResponseDTO sendMessageRes = await _baseServices.LiveSendAsync(requestDTO);
        //    if (sendMessageRes == null || sendMessageRes.IsSuccess == false)
        //    {
        //        _responseDTO.IsSuccess = false;
        //        _responseDTO.Message = "SEND PHOTO MESSAGE FAILED";
        //        return _responseDTO;
        //    }
        //    return sendMessageRes;
        //}
    }
}
