using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using Serilog;
using System.Globalization;
using System.Text;
using System.Text.Json.Nodes;
using System.Text.RegularExpressions;
using TELEBOT_CSKH.Constant;
using TELEBOT_CSKH.Data;
using TELEBOT_CSKH.Models.DTO;
using TELEBOT_CSKH.Models.TELEGRAM_BOT;
using TELEBOT_CSKH.Services;
using TELEBOT_CSKH.Services.Telegram;

namespace TELEBOT_CSKH.Controllers
{
    [Route("api/Telegram")]
    [ApiController]
    public class TelegramController : ControllerBase
    {
        //https://api.telegram.org/bot7582060841:AAGYjU1LwOcgZFXjl5khoB3oLOq_Dm86DsI/setWebhook?url=https://6c8d-202-183-210-160.ngrok-free.app/api/Telegram
        
        private AppDbContext _appDbContext;
        private readonly ITelegramService _telegramServices;
        private readonly IBOServices _boServices;
        private static List<TelegramAccount> _botCheckAgent;
        private static List<TelegramAccount> _botShareTele;
        private static List<TelegramResponse> _telegramResposeList;

        private static string _regexPattern = @"^\/([\w\d]+) (\d{2}\/\d{2}\/\d{4}) (\d{2}\/\d{2}\/\d{4})$";
        public TelegramController(AppDbContext appDbContext, ITelegramService telegramServices, IBOServices boServices)
        {
            Console.WriteLine("TelegramController");
            _appDbContext = appDbContext;
            _telegramServices = telegramServices;
            _boServices = boServices;
        }

        [HttpPost]
        [Route("BotCheckAgent/{Site}/{Bot}")]
        //public async Task<IActionResult> BotCheckAgent(string Site, string Bot)
        //{
        //    try
        //    {
        //        if (SD.fBotShareTeleUpdate == false || _botCheckAgent == null)
        //        {
        //            _botCheckAgent = await _appDbContext.TelegramAccount.Where(x => x.BotType == 99).ToListAsync();
        //            SD.fBotShareTeleUpdate = true;
        //        }

        //        if (Site.IsNullOrEmpty() || Bot.IsNullOrEmpty())
        //        {
        //            return BadRequest("Query Error");
        //        }

        //        using var reader = new StreamReader(Request.Body);

        //        var body = await reader.ReadToEndAsync();
        //        if (string.IsNullOrEmpty(body))
        //        {
        //            return BadRequest("Request body is empty.");
        //        }

        //        //dynamic requestObject = JsonConvert.DeserializeObject<dynamic>(body.ToString());
        //        //Console.WriteLine(JsonConvert.SerializeObject(requestObject, Formatting.Indented));

        //        TelegramUpdate update = JsonConvert.DeserializeObject<TelegramUpdate>(body.ToString());
        //        if (update == null)
        //        {
        //            return BadRequest("Failed to deserialize update.");
        //        }

        //        Guid BotID = Guid.Parse(Bot.ToUpper());

        //        // Log the received update or process it as needed
        //        //Console.WriteLine($"BotCheckAgent Received: " + JsonConvert.SerializeObject(update, Formatting.Indented));
                
        //        DateTimeOffset dateTimeOffset = DateTimeOffset.FromUnixTimeSeconds(update.message.date);
        //        TimeSpan timeDifference = DateTimeOffset.Now - dateTimeOffset;
        //        if (timeDifference.TotalSeconds>=10)
        //        {
        //            return Ok();
        //        }

        //        //Process the update(e.g., log the message or handle a callback query)
        //        if (update.message != null && update.message.text != null)
        //        {
        //            TelegramAccount _targetTelegramAccount = _botCheckAgent.Where(x => x.ID == Guid.Parse(Bot.ToUpper())).FirstOrDefault();
        //            if (_targetTelegramAccount==null)
        //            {
        //                return Ok();
        //            }

        //            if (update.message.text == "/id" || update.message.text == $"/id{_targetTelegramAccount.UserName}")
        //            {
        //                await _telegramServices.ReplyDirectMessageAsync(_targetTelegramAccount.Token, update.message.chat.id, update.message.message_id, update.message.chat.id.ToString());
        //                return Ok();
        //            }

        //            Console.WriteLine($"{update.message.from.username} - {update.message.chat.type} - {update.message.text}");

        //            //Checking condition for IsIndividualWorking account working+
        //            if (update.message.chat.type == "private" && _targetTelegramAccount.IsIndividualWorking == false)
        //            {
        //                return Ok();
        //            }

        //            if (update.message.chat.type == "group" && !(_targetTelegramAccount.ChatID == "*" || _targetTelegramAccount.ChatID.Contains(update.message.chat.id.ToString())))
        //            {
        //                return Ok();
        //            }
                  
        //            if (update.message.text == "/update" || update.message.text == $"/update{_targetTelegramAccount.UserName}")
        //            {
        //                SD.fBotShareTeleUpdate = false;
        //                return Ok();
        //            }

        //            Match match = Regex.Match(update.message.text, _regexPattern);
        //            if (match.Success)
        //            {
        //                TimeZoneInfo tz = TimeZoneInfo.FindSystemTimeZoneById("SE Asia Standard Time"); // Or "China Standard Time"
        //                string format = "dd/MM/yyyy";
                        
        //                string AgentName = match.Groups[1].Value;   // "abcd"
        //                string FromDate = match.Groups[2].Value;  // "01/12/2025"
        //                string ToDate = match.Groups[3].Value;  // "12/12/2025"

        //                DateTime dtFromDate = DateTime.ParseExact(FromDate, format, CultureInfo.InvariantCulture);
        //                DateTime dtUtc8 = TimeZoneInfo.ConvertTime(dtFromDate, tz);
        //                long fromDateUTC = new DateTimeOffset(dtUtc8).ToUnixTimeSeconds()-3600;

        //                DateTime dtToDate = DateTime.ParseExact(ToDate, format, CultureInfo.InvariantCulture).AddHours(23).AddMinutes(59).AddSeconds(59);
        //                dtUtc8 = TimeZoneInfo.ConvertTime(dtToDate, tz);
        //                long toDateUTC = new DateTimeOffset(dtUtc8).ToUnixTimeSeconds() - 3600;

        //                Log.Information(AgentName + "|" + fromDateUTC + "|" + toDateUTC);

        //                CheckAgentResultDTO _checkAgentResult = await HandleCheckAgentAsync(AgentName, fromDateUTC, toDateUTC, Site, BotID);
        //                if (_checkAgentResult!=null)
        //                {
        //                    StringBuilder sb = new StringBuilder();
        //                    sb.AppendLine($"- Khách đăng ký:  {_checkAgentResult.NewRegister.ToString("N0")}");
        //                    sb.AppendLine($"- Khách mới:      {_checkAgentResult.NewCustomer.ToString("N0")}");
        //                    sb.AppendLine($"- Khách nạp tiền: {_checkAgentResult.DepositCustomer.ToString("N0")}");
        //                    sb.AppendLine($"- Tổng nạp:       {_checkAgentResult.TotalDeposit.ToString("N0")}");
        //                    sb.AppendLine($"- Tổng rút:       {_checkAgentResult.TotalWithdraw.ToString("N0")}");
        //                    sb.AppendLine($"- Khách online:   {_checkAgentResult.TotalOnline.ToString("N0")}");
        //                    sb.AppendLine($"- Cược hợp lệ:    {_checkAgentResult.TotalValidBet.ToString("N0")}");
        //                    if (_checkAgentResult.TotalValidBet<=0)
        //                    {
        //                        sb.AppendLine($"- Vòng cược:      0");
        //                    }else
        //                    {
        //                        if (_checkAgentResult.TotalDeposit == 0)
        //                        {
        //                            sb.AppendLine($"- Vòng cược:      Không nạp");
        //                        } else
        //                        {
        //                            sb.AppendLine($"- Vòng cược:      {_checkAgentResult.TurnAround.ToString("N0")}");
        //                        }
        //                    }

        //                    await _telegramServices.ReplyDirectMessageAsync(_targetTelegramAccount.Token, update.message.chat.id, update.message.message_id, sb.ToString());
        //                }else
        //                {
        //                    await _telegramServices.ReplyDirectMessageAsync(_targetTelegramAccount.Token, update.message.chat.id, update.message.message_id, "Không tìm thấy dữ liệu");
        //                }    
        //            }
        //            else
        //            {
        //                await _telegramServices.ReplyDirectMessageAsync(_targetTelegramAccount.Token, update.message.chat.id, update.message.message_id, "Cú pháp k hợp lệ");
        //                return Ok();
        //            }
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        Console.WriteLine(ex.Message);
        //    }
        //    return Ok();
        //}

        private async Task<CheckAgentResultDTO> HandleCheckAgentAsync(string agentName, long fromDateUTC, long toDateUTC, string site, Guid botID)
        {
            CheckAgentResultDTO _checkAgentResult = new CheckAgentResultDTO();
            try
            {
                if (site=="F168")
                {
                    ResponseDTO _checkTotalAgentRes = await _boServices.CheckTotalAgentF168Async(agentName, fromDateUTC, toDateUTC);
                    
                    if (_checkTotalAgentRes != null && _checkTotalAgentRes.IsSuccess == true)
                    {
                        dynamic totalAgentObj = JsonConvert.DeserializeObject<dynamic>(_checkTotalAgentRes.Result.ToString());
                        if (totalAgentObj["valid"] == true && totalAgentObj["status_code"] == 200 && totalAgentObj["result"].Count > 0)
                        {
                            List<UserFinancialInfoDTO> totalUserFinancialInfoDTOs = JsonConvert.DeserializeObject<List<UserFinancialInfoDTO>>(totalAgentObj["result"].ToString());
                            _checkAgentResult.DepositCustomer = totalUserFinancialInfoDTOs.Where(x => x.DepositTimes > 0).Count();
                            _checkAgentResult.TotalDeposit = totalUserFinancialInfoDTOs.Sum(x => Convert.ToInt32(double.Parse(x.Deposit)));
                            _checkAgentResult.TotalWithdraw = totalUserFinancialInfoDTOs.Sum(x => Convert.ToInt32(double.Parse(x.Withdraw)));
                            _checkAgentResult.TotalOnline = totalUserFinancialInfoDTOs.Count;
                            _checkAgentResult.TotalValidBet = totalUserFinancialInfoDTOs.Sum(x => Convert.ToInt32(double.Parse(x.ValidBet)));
                            
                            if (_checkAgentResult.TotalValidBet==0)
                            {
                                _checkAgentResult.TurnAround = 0;
                            }else
                            {  
                                _checkAgentResult.TurnAround = (int)(_checkAgentResult.TotalValidBet / _checkAgentResult.TotalDeposit);
                            }
                        }
                        else
                        {
                            return null;
                        }
                    }

                    ResponseDTO _checkNewRegisRes = await _boServices.CheckNewRegisterAgentF168Async(agentName, fromDateUTC, toDateUTC);
                    if (_checkNewRegisRes != null && _checkNewRegisRes.IsSuccess == true)
                    {
                        dynamic newAgentObj = JsonConvert.DeserializeObject<dynamic>(_checkNewRegisRes.Result.ToString());
                        if (newAgentObj["valid"] == true && newAgentObj["status_code"] == 200)
                        {
                            _checkAgentResult.NewRegister = Int32.Parse(newAgentObj["result"]["total"].ToString());
                        }
                    }

                    ResponseDTO _checkNewCustomerRes = await _boServices.CheckNewCustomerAgentF168Async(agentName, fromDateUTC, toDateUTC);
                    if (_checkNewCustomerRes != null && _checkNewCustomerRes.IsSuccess == true)
                    {
                        dynamic checkNewCustomerObj = JsonConvert.DeserializeObject<dynamic>(_checkNewCustomerRes.Result.ToString());
                        if (checkNewCustomerObj["valid"] == true && checkNewCustomerObj["status_code"] == 200)
                        {
                            _checkAgentResult.NewCustomer = Int32.Parse(checkNewCustomerObj["result"]["total"].ToString());
                        }
                    }
                }
                else
                {
                    Log.Information("ERROR HERE");
                    return null;
                }
            }
            catch (Exception ex)
            {
                Log.Information("#1 - "+ex.Message);
            }
            return _checkAgentResult;
        }


        

        //[HttpPost]
        //[Route("BotShareTele/{Site}/{Bot}")]
        //public async Task<IActionResult> BotShareTele(string Site,string Bot)
        //{
        //    try
        //    {
        //        if (SD.fBotShareTeleUpdate == false || _botShareTele == null)
        //        {
        //            _botShareTele = await _appDbContext.TelegramAccount.Where(x => x.BotType == 0).ToListAsync();
        //            SD.fBotShareTeleUpdate = true;
        //        }

        //        if (Site.IsNullOrEmpty() || Bot.IsNullOrEmpty())
        //        {
        //            return BadRequest("Query Error");
        //        }

        //        using var reader = new StreamReader(Request.Body);
        //        var body = await reader.ReadToEndAsync();
        //        if (string.IsNullOrEmpty(body))
        //        {
        //            return BadRequest("Request body is empty.");
        //        }

        //        TelegramUpdate update = JsonConvert.DeserializeObject<TelegramUpdate>(body.ToString());
        //        if (update == null)
        //        {
        //            return BadRequest("Failed to deserialize update.");
        //        }

        //        Guid BotID = Guid.Parse(Bot.ToUpper());

        //        // Log the received update or process it as needed
        //        Console.WriteLine($"Received Update: " + JsonConvert.SerializeObject(update, Formatting.Indented));

        //        // Process the update (e.g., log the message or handle a callback query)
        //        if (update.message != null && update.message.text != null)
        //        {
        //            Console.WriteLine($"{update.message.from.username} - {update.message.text}");

        //            //Import New Telegram Customer if Command is /start
        //            if (update.message.text.Contains("/start"))
        //            {
        //                await HandleStartCommandAsync(update, Site, BotID);
        //                return Ok(); 
        //            }

        //            if (update.message.text == "Link của bạn 🔗" || update.message.text == "/getlink")
        //            {
        //                await HandleGetAFFLinkAsync(update, Site, BotID);
        //                return Ok();
        //            }

        //            if (update.message.text == "Nhận thưởng 🎁" || update.message.text == "/getcode")
        //            {
        //                await HandleGetCodeAsync(update, Site, BotID);
        //                return Ok();
        //            }
        //        }
        //        else if (update.callback_query != null)
        //        {
        //            if (update.callback_query.data == "GetAFFLink")
        //            {
        //                await HandleGetAFFLinkAsync(update, Site, BotID);
        //                return Ok();
        //            }

        //            if (update.callback_query.data == "GetAFFCode")
        //            {
        //                await HandleGetCodeAsync(update, Site, BotID);
        //                return Ok();
        //            }
        //            Console.WriteLine($"{update.callback_query.data}");
        //        }

        //        // Respond with 200 OK to acknowledge receipt of the update
        //        return Ok();
        //    }
        //    catch (Exception ex)
        //    {
        //        Console.WriteLine($"Error: {ex.Message}");
        //        return StatusCode(500, "Internal Server Error");
        //    }
        //}

        
        private async Task HandleGetCodeAsync(TelegramUpdate Update, string Site, Guid BotID)
        {
            string _targetID = "";
            if (Update.callback_query != null)
            {
                _targetID = Update.callback_query.from.id.ToString();

            }
            else
            {
                _targetID = Update.message.from.id.ToString();
            }

            if (_targetID != "")
            {
                TelegramCustomer _telegramCustomer = await _appDbContext.TelegramCustomers
                                                                .Where(x => x.TelegramID == _targetID && x.System == Site)
                                                                .FirstOrDefaultAsync();

                //await _telegramServices.SendCodeAsync(_telegramCustomer, Site, BotID);
            }
        }

        


        [HttpPost]
        [Route("BotShareTele/{Site}/{Bot}")]
        public async Task<IActionResult> BotShareTele(string Site, string Bot)
        {
            try
            {
                if (SD.fBotShareTeleUpdate == false || _botShareTele == null)
                {
                    _botShareTele = await _appDbContext.TelegramAccount.Where(x => x.BotType == 0).ToListAsync();
                    SD.fBotShareTeleUpdate = true;
                }

                if (SD.fTeleResposeDataUpdate == true || _telegramResposeList == null)
                {
                    _telegramResposeList = await _appDbContext.TelegramResponse.Where(x => x.BotID == Guid.Parse(Bot)).ToListAsync();
                    SD.fTeleResposeDataUpdate = false;
                }

                if (Site.IsNullOrEmpty() || Bot.IsNullOrEmpty())
                {
                    return BadRequest("Query Error");
                }

                using var reader = new StreamReader(Request.Body);
                var body = await reader.ReadToEndAsync();
                if (string.IsNullOrEmpty(body))
                {
                    return BadRequest("Request body is empty.");
                }

                TelegramUpdate update = JsonConvert.DeserializeObject<TelegramUpdate>(body.ToString());
                if (update == null)
                {
                    return BadRequest("Failed to deserialize update.");
                }

                TelegramAccount _telegramAccount = _botShareTele.Where(x => x.ID == Guid.Parse(Bot.ToUpper())).FirstOrDefault();
                if (_telegramAccount == null)
                {
                    return BadRequest("No Telegram Account Found");
                }

                // Log the received update or process it as needed
                Console.WriteLine($"Received Update: " + JsonConvert.SerializeObject(update, Formatting.Indented));

                if (update.message != null && update.message.text != null)
                { 
                    //Handle Message Text 
                    TelegramResponse _telegramResponse = _telegramResposeList.Where(x => x.BotID == Guid.Parse(Bot) && x.RequestCode == update.message.text).FirstOrDefault();
                    if(_telegramResponse!=null)
                    {
                        await _telegramServices.SendTelegramMessageAsync(_telegramAccount.Token, update.message.chat.id, _telegramResponse.URLImage, _telegramResponse.Content, _telegramResponse.InlineKeyboard);
                    }
                        
                    if (update.message.text == "/start")
                    {
                        TelegramCustomer _telegramCustomer = await _appDbContext.TelegramCustomers
                                                                    .Where(x => x.TelegramID == update.message.from.id.ToString() && x.System == Site)
                                                                    .FirstOrDefaultAsync();
                        if (_telegramCustomer == null)
                        {
                            //Import New Telegram Customer if Command is /start
                            TelegramCustomer newTelegramCustomer = new TelegramCustomer
                            {
                                ID = Guid.NewGuid(),
                                TelegramID = update.message.from.id.ToString(),
                                Name = update.message.from.first_name,
                                UserName = update.message.from.username,
                                System = Site,
                                iPremium = update.message.from.is_premium,
                                CreateDate = DateTime.Now,
                                BotID = Guid.Parse(Bot),
                                BotAffiliateID = null
                            };
                            Console.WriteLine($"IMPORT - {newTelegramCustomer.UserName}");
                            _appDbContext.TelegramCustomers.Add(newTelegramCustomer);
                            await _appDbContext.SaveChangesAsync();
                        }

                        await _telegramServices.SendTelegramKeyboardAsync(_telegramAccount.Token, update.message.from.id, $"Chào mừng {update.message.from.first_name} đã cùng tham gia.", _telegramAccount.KeyboardData);
                    }
                    else
                    {
                        string pattern = @"aff(\d+)";
                        pattern += Site.ToUpper();
                        Match match = Regex.Match(update.message.text, pattern);
                        if (match.Success)
                        {
                            string BotAffiliateID = match.Groups[1].Value;

                            await _telegramServices.SendTelegramKeyboardAsync(_telegramAccount.Token, update.message.from.id, $"Chào mừng {update.message.from.first_name} đã cùng tham gia.", _telegramAccount.KeyboardData);

                            if (!_appDbContext.TelegramCustomers.Any(x=>x.TelegramID==update.message.chat.id.ToString() && x.System == Site))
                            { 
                                TelegramCustomer _telegramAFF = await _appDbContext.TelegramCustomers
                                    .Where(x => x.TelegramID == BotAffiliateID)
                                    .FirstOrDefaultAsync();
                                if (_telegramAFF != null)
                                {
                                    TelegramCustomer telegramCustomer = new TelegramCustomer
                                    {
                                        ID = Guid.NewGuid(),
                                        TelegramID = update.message.from.id.ToString(),
                                        Name = update.message.from.first_name,
                                        UserName = update.message.from.username,
                                        System = Site,
                                        iPremium = update.message.from.is_premium,
                                        CreateDate = DateTime.Now,
                                        BotID = Guid.Parse(Bot),
                                        BotAffiliateID = _telegramAFF.ID
                                    };
                                    _appDbContext.TelegramCustomers.Add(telegramCustomer);

                                    _telegramAFF.ShareCount += 1;
                                    _appDbContext.TelegramCustomers.Update(_telegramAFF);

                                    await _appDbContext.SaveChangesAsync();
                                    await _telegramServices.SendTelegramKeyboardAsync(_telegramAccount.Token, long.Parse(telegramCustomer.TelegramID), $"Tài khoản của bạn được giới thiệu bởi {_telegramAFF.Name}", _telegramAccount.KeyboardData);
                                    await _telegramServices.SendTelegramKeyboardAsync(_telegramAccount.Token, long.Parse(_telegramAFF.TelegramID), $"Xin chào quý khách, hệ thống ghi nhận {telegramCustomer.Name} đã được giới thiệu bởi link chia sẻ của bạn. Số lượt chia sẻ hiện tại là {_telegramAFF.ShareCount}/15", _telegramAccount.KeyboardData);
                                }
                            }   
                        }
                    }    
                }
                else if (update.callback_query != null)
                {
                    TelegramCustomer _telegramCustomer = await _appDbContext.TelegramCustomers
                                                                    .Where(x => x.TelegramID == update.callback_query.from.id.ToString() && x.System == Site)
                                                                    .FirstOrDefaultAsync();
                    if (_telegramCustomer!=null)
                    {
                        switch (update.callback_query.data)
                        {
                            case "GetAFFLink":
                                StringBuilder affLinkStringBuilder = new StringBuilder();
                                affLinkStringBuilder.AppendLine($"Đây là link chia sẻ của bạn! Hãy nhanh tay chia sẻ nó với bạn bè và người thân để nhận phần thưởng hấp dẫn ngay hôm nay. Đừng bỏ lỡ cơ hội này!\n");
                                affLinkStringBuilder.AppendLine($"<a href=\"https://t.me/{_telegramAccount.UserName.Substring(1)}?start=aff{_telegramCustomer.TelegramID}{Site}\">https://t.me/{_telegramAccount.UserName.Substring(1)}?start=aff{_telegramCustomer.TelegramID}{Site}</a>");

                                await _telegramServices.SendTelegramMessageAsync(_telegramAccount.Token, update.callback_query.from.id, null, affLinkStringBuilder.ToString(), null);
                                break;
                            case "GetAFFCode":
                                StringBuilder getCodeLStringBuilder = new StringBuilder();
                                getCodeLStringBuilder.AppendLine($"✨🔥 KẾT QUẢ CHIA SẺ LINK - RINH GIFTCODE {Site}! 🔥✨\n");
                                if (_telegramCustomer.ShareCount >= 15)
                                {
                                    getCodeLStringBuilder.AppendLine($"✅ Tài khoản của bạn đã chia sẻ {_telegramCustomer.ShareCount}/15 lượt. Quý khách vui lòng liên hệ bộ CSKH để nhận thưởng! ⏳✨\n");
                                }
                                else
                                {
                                    getCodeLStringBuilder.AppendLine($"Tài khoản của bạn đã chia sẻ {_telegramCustomer.ShareCount}/15 lượt. Hãy nhanh tay hoàn thành ngay trước khi chương trình kết thúc! ⏳✨\n");
                                }
                                getCodeLStringBuilder.AppendLine("🎉 Cảm ơn tất cả các bạn đã tham gia và đóng góp, sự ủng hộ của các bạn là động lực to lớn cho chúng tôi! 💖");

                                await _telegramServices.SendTelegramMessageAsync(_telegramAccount.Token, update.callback_query.from.id, null, getCodeLStringBuilder.ToString(), null);
                                break;
                            default:
                                break;
                        }
                        
                        Console.WriteLine($"{update.callback_query.data}");
                        return Ok();
                    }
                }

                // Respond with 200 OK to acknowledge receipt of the update
                return Ok();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
                return StatusCode(500, "Internal Server Error");
            }
        }

    }
}
