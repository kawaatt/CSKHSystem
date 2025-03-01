using API.Constant;
using API.Data;
using API.Models.CSKHAuto;
using API.Models.DTO;
using API.Services.Redis;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using StackExchange.Redis;
using System.Diagnostics.Eventing.Reader;
using System.Net;
using System.Text;
using System.Text.Json;
using Telegram.Bot.Types;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace API.Middleware.TicketRequestMW
{
    public class RateLimitMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly IServiceProvider _serviceProvider;

        public RateLimitMiddleware(RequestDelegate next, IServiceProvider serviceProvider)
        {
            _next = next;
            _serviceProvider = serviceProvider;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            Console.WriteLine("RateLimitMiddleware");

            SD.DomainImageURL = $"{context.Request.Scheme}://{context.Request.Host}{context.Request.PathBase}"; ;
            Console.WriteLine($"DomainImageURL: {SD.DomainImageURL}");

            if (context.Request.Method == HttpMethods.Post &&
               context.Request.Path.StartsWithSegments("/api/BOAccount/F168CheckAccount"))
            {
                context.Request.EnableBuffering(); // Allow multiple reads
                using (var reader = new StreamReader(context.Request.Body, Encoding.UTF8, leaveOpen: true))
                {
                    string body = await reader.ReadToEndAsync();

                    // Reset body stream position for the next middleware/controller
                    context.Request.Body.Position = 0;

                    if (string.IsNullOrWhiteSpace(body))
                    {
                        context.Response.ContentType = "application/json";
                        context.Response.StatusCode = StatusCodes.Status200OK;
                        await context.Response.WriteAsync(JsonConvert.SerializeObject(new ResponseDTO { IsSuccess = false, Message = "#1 - Truy vấn không hợp lệ" }));
                        return; // Stop further processing
                    }

                    BOAccountRequestDTO checkAccountObj = JsonConvert.DeserializeObject<BOAccountRequestDTO>(body);
                    context.Items["AccountRequest"] = checkAccountObj;
                    using (var scope = _serviceProvider.CreateScope())
                    {
                        if (string.IsNullOrWhiteSpace(checkAccountObj.Account)
                            || string.IsNullOrWhiteSpace(checkAccountObj.CardHolder))
                        {
                            context.Response.ContentType = "application/json";
                            context.Response.StatusCode = StatusCodes.Status200OK;
                            await context.Response.WriteAsync(JsonConvert.SerializeObject(new ResponseDTO { IsSuccess = false, Message = "#1 - Truy vấn không hợp lệ" }));
                            return; // Stop further processing
                        }

                        // Lấy dịch vụ Scoped từ scope mới
                        var _redisServices = scope.ServiceProvider.GetRequiredService<IRedisServices>();

                        var key = $"f168-{checkAccountObj.Account}";
                        var requestObj= await _redisServices.GetValueAsync(key);
                        if (string.IsNullOrEmpty(requestObj))
                        {
                            checkAccountObj.TheLastRequest = DateTime.Now;
                            checkAccountObj.RequestCount = 1;
                            await _redisServices.SetValueAsync(key, JsonConvert.SerializeObject(checkAccountObj), TimeSpan.FromMinutes(30));
                        }
                        else
                        {
                            BOAccountRequestDTO targetRequest = JsonConvert.DeserializeObject<BOAccountRequestDTO>(requestObj);

                            TimeSpan timeDifference = DateTimeOffset.Now - DateTimeOffset.Parse(targetRequest.TheLastRequest.ToString());
                            targetRequest.RequestCount -= Int32.Parse(Math.Floor(timeDifference.TotalSeconds).ToString()) / 10;
                            if (targetRequest.RequestCount <= 0)
                            {
                                targetRequest.RequestCount = 0;
                            }
                            
                            targetRequest.RequestCount += 1;
                            if (targetRequest.RequestCount >5)
                            {
                                targetRequest.RequestCount = 5;
                            }

                            targetRequest.TheLastRequest = DateTime.Now;
                            await _redisServices.SetValueAsync(key, JsonConvert.SerializeObject(targetRequest), TimeSpan.FromMinutes(30));

                            if (targetRequest.RequestCount >= 5)
                            {
                                context.Response.ContentType = "application/json";
                                context.Response.StatusCode = StatusCodes.Status200OK;
                                await context.Response.WriteAsync(JsonConvert.SerializeObject(new ResponseDTO { IsSuccess = false, Message = "#2 - Số lượt truy vấn đã quá giới hạn. Quý khách vui lòng thử lại sau ít phút" }));
                                return; // Stop further processing
                            }
                        }
                    }
                }
            }
            await _next(context);
        }
    }
}
