using API.Data;
using API.Models.CSKHAuto;
using API.Models.DTO;
using API.Services;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using StackExchange.Redis;
using System.Collections.Immutable;
using System.Diagnostics.Eventing.Reader;
using System.Net;
using System.Text;
using System.Text.Json;

namespace API.Middleware.TicketRequestMW
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
            Console.WriteLine("CheckAccountMiddleware");
            

            // Check if the request is a POST to the desired endpoint
            if (context.Request.Method == HttpMethods.Post &&
               context.Request.Path.StartsWithSegments("/api/BOAccount/F168CheckAccount"))
            {
                using (var scope = _serviceProvider.CreateScope())
                {
                    var _boServices = scope.ServiceProvider.GetRequiredService<IBOServices>();
                    var _dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();

                    BOAccountRequestDTO checkAccountObj = context.Items["AccountRequest"] as BOAccountRequestDTO;
                    if (_dbContext.CheckAccountFilters.Any(x=>x.Account==checkAccountObj.Account && x.System=="F168" && x.CardHolder==checkAccountObj.CardHolder))
                    {
                        await _next(context);
                        return;
                    }

                    Console.WriteLine("HERE - " + JsonConvert.SerializeObject(checkAccountObj));
                    ResponseDTO CheckAccountRes = await _boServices.F168_CheckAccount(checkAccountObj.Account);
                    Console.WriteLine(JsonConvert.SerializeObject(CheckAccountRes));
                    if (CheckAccountRes != null && CheckAccountRes.IsSuccess == true)
                    {
                        var resObjects = (JObject)JsonConvert.DeserializeObject(CheckAccountRes.Result.ToString());
                        var BankAccountList = resObjects["accountDetailInfo"]["withdrawalAccount"].ToList();
                        bool checkBankAccountCondition = false;

                        for (int i = 0; i < BankAccountList.Count(); i++)
                        {
                            string _tempBankAccount = BankAccountList[i]["account"].ToString();
                            if (checkAccountObj.CardHolder == _tempBankAccount.Substring(_tempBankAccount.Length - 4))
                            {
                                BOAccount _tempBoAccount = new BOAccount();
                                _tempBoAccount.ID = new Guid();
                                _tempBoAccount.Account = checkAccountObj.Account;
                                _tempBoAccount.CardHolder = checkAccountObj.CardHolder;
                                _tempBoAccount.System = "F168";
                                _dbContext.CheckAccountFilters.Add(_tempBoAccount);
                                await _dbContext.SaveChangesAsync();
                                
                                checkBankAccountCondition = true;
                                break;
                            }
                        }

                        if (checkBankAccountCondition == false)
                        {
                            context.Response.ContentType = "application/json";
                            context.Response.StatusCode = StatusCodes.Status200OK;
                            await context.Response.WriteAsync(JsonConvert.SerializeObject(new ResponseDTO { IsSuccess = false, Message = "#2 - Lỗi xác thực tài khoản, quý khách vui lòng thử lại." }));
                            return; // Stop further processing
                        }
                    }
                }
            }
            await _next(context);
        }
    }
}
