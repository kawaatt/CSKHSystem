using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using TELEBOT_CSKH.Models.CSKHAuto;
using TELEBOT_CSKH.Models.DTO;
using TELEBOT_CSKH.Services;

namespace TELEBOT_CSKH.Middleware.TicketRequestMW
{
    public class CheckAccountMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly IServiceProvider _serviceProvider;

        public CheckAccountMiddleware(RequestDelegate next, IServiceProvider serviceProvider)
        {
            _next = next;
            _serviceProvider = serviceProvider;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            //Console.WriteLine("CheckAccountMiddleware");
            //Check if the request is a POST to the desired endpoint
            //if (context.Request.Method == HttpMethods.Post &&
            //   context.Request.Path.StartsWithSegments("/api/TicketRequest/SendTicketRequest"))
            //{
            //    TicketRequestDTO ticketRequestData = (TicketRequestDTO)context.Items["TicketRequestData"];

            //    using (var scope = _serviceProvider.CreateScope())
            //    {
            //        var _boServices = scope.ServiceProvider.GetRequiredService<IBOServices>();

            //        ResponseDTO CheckAccountRes = await _boServices.F168_CheckAccount(ticketRequestData.Account, ticketRequestData.CardHolder);
            //        Console.WriteLine(JsonConvert.SerializeObject(CheckAccountRes));
            //        if (CheckAccountRes != null && CheckAccountRes.IsSuccess == true)
            //        {
            //            var resObjects = (JObject)JsonConvert.DeserializeObject(CheckAccountRes.Result.ToString());
            //            Console.WriteLine(JsonConvert.SerializeObject(resObjects));
            //            var BankAccountList = resObjects["accountDetailInfo"]["withdrawalAccount"].ToList();
            //            bool checkBankAccountCondition = false;

            //            for (int i = 0; i < BankAccountList.Count(); i++)
            //            {
            //                string _tempBankAccount = BankAccountList[i]["account"].ToString();
            //                if (ticketRequestData.CardHolder == _tempBankAccount.Substring(_tempBankAccount.Length - 4))
            //                {
            //                    checkBankAccountCondition = true;
            //                    break;
            //                }
            //            }

            //            if (checkBankAccountCondition == false)
            //            {
            //                context.Response.ContentType = "application/json";
            //                context.Response.StatusCode = StatusCodes.Status200OK;
            //                await context.Response.WriteAsync(JsonConvert.SerializeObject(new ResponseDTO { IsSuccess = false, Message = "#2 - Lỗi xác thực tài khoản, quý khách vui lòng thử lại." }));
            //                return; // Stop further processing
            //            }
            //        }
            //    }
            //}
            await _next(context);
        }
    }
}
