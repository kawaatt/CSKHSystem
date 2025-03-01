using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System.Text;
using System.Text.RegularExpressions;
using TELEBOT_CSKH.Data;
using TELEBOT_CSKH.Models.CSKHAuto;
using TELEBOT_CSKH.Models.DTO;
using TELEBOT_CSKH.Models.TELEGRAM_BOT;
using TELEBOT_CSKH.Services.Redis;

namespace TELEBOT_CSKH.Middleware.TicketRequestMW
{
    public class TelegramCustomerMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly IServiceProvider _serviceProvider;

        public TelegramCustomerMiddleware(RequestDelegate next, IServiceProvider serviceProvider)
        {
            _next = next;
            _serviceProvider = serviceProvider;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            //string pattern = @"/api/Telegram/(?<system>[^/]+)/(?<guid>[a-f0-9\-]+)";
            //var match = Regex.Match(context.Request.Path, pattern);
            //if (match.Success)
            //{
            //    string _system = match.Groups["system"].Value;
            //    Guid _uid = Guid.Parse(match.Groups["guid"].Value);

            //    string requestBody;
            //    var request = context.Request;
            //    request.EnableBuffering();
            //    using (var reader = new StreamReader(request.Body, leaveOpen: true))
            //    {
            //        requestBody = await reader.ReadToEndAsync();
            //        request.Body.Position = 0; // Reset stream for next middleware
            //    }

            //    TelegramUpdate update = JsonConvert.DeserializeObject<TelegramUpdate>(requestBody);
            //    Console.WriteLine(JsonConvert.SerializeObject(update));

            //    if (update.message != null)
            //    {
            //        //Check command
            //        if (update.message.text != null && update.message.text == "/start")
            //        {
            //            var _db = _serviceProvider.CreateScope().ServiceProvider.GetRequiredService<AppDbContext>();
            //            TelegramCustomer _telegramCustomer = await _db.TelegramCustomers.Where(x => x.TelegramID == update.message.from.id.ToString() && x.System == _system).FirstOrDefaultAsync();
            //            if (_telegramCustomer == null)
            //            {
            //                TelegramCustomer telegramCustomer = new TelegramCustomer
            //                {
            //                    ID = new Guid(),
            //                    TelegramID = update.message.from.id.ToString(),
            //                    Name = update.message.from.first_name,
            //                    UserName = update.message.from.username,
            //                    System = _system,
            //                    iPremium = update.message.from.is_premium,
            //                    CreateDate = DateTime.Now,
            //                    BotID = _uid
            //                };
            //                Console.WriteLine($"IMPORT - {telegramCustomer.UserName}");
            //                _db.TelegramCustomers.Add(telegramCustomer);
            //                await _db.SaveChangesAsync();
            //            }
            //        }
            //    }
            //}
            //else
            //{
            //    return;
            //}
            await _next(context);
        }
    }
}
