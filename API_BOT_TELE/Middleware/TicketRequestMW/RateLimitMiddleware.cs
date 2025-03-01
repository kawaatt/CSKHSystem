using Newtonsoft.Json;
using System.Text;
using TELEBOT_CSKH.Models.CSKHAuto;
using TELEBOT_CSKH.Models.DTO;
using TELEBOT_CSKH.Services.Redis;

namespace TELEBOT_CSKH.Middleware.TicketRequestMW
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
            //Console.WriteLine("RateLimitMiddleware");
            //// Check if the request is a POST to the desired endpoint
            //if (context.Request.Method == HttpMethods.Post &&
            //   context.Request.Path.StartsWithSegments("/api/TicketRequest/SendTicketRequest"))
            //{
            //    context.Request.EnableBuffering(); // Allow multiple reads
            //    using (var reader = new StreamReader(context.Request.Body, Encoding.UTF8, leaveOpen: true))
            //    {
            //        string body = await reader.ReadToEndAsync();

            //        // Reset body stream position for the next middleware/controller
            //        context.Request.Body.Position = 0;

            //        if (string.IsNullOrWhiteSpace(body))
            //        {
            //            context.Response.ContentType = "application/json";
            //            context.Response.StatusCode = StatusCodes.Status200OK;
            //            await context.Response.WriteAsync(JsonConvert.SerializeObject(new ResponseDTO { IsSuccess = false, Message = "#1 - Truy vấn không hợp lệ" }));
            //            return; // Stop further processing
            //        }

            //        TicketRequestDTO ticketRequestData = JsonConvert.DeserializeObject<TicketRequestDTO>(body);
            //        context.Items["TicketRequestData"] = ticketRequestData;
            //        using (var scope = _serviceProvider.CreateScope())
            //        {
            //            if (string.IsNullOrWhiteSpace(ticketRequestData.Account)
            //                || string.IsNullOrWhiteSpace(ticketRequestData.CardHolder)
            //                || string.IsNullOrWhiteSpace(ticketRequestData.TicketCode)
            //                || string.IsNullOrWhiteSpace(ticketRequestData.System))
            //            {
            //                context.Response.ContentType = "application/json";
            //                context.Response.StatusCode = StatusCodes.Status200OK;
            //                await context.Response.WriteAsync(JsonConvert.SerializeObject(new ResponseDTO { IsSuccess = false, Message = "#1 - Truy vấn không hợp lệ" }));
            //                return; // Stop further processing
            //            }

            //            // Lấy dịch vụ Scoped từ scope mới
            //            var _redisServices = scope.ServiceProvider.GetRequiredService<IRedisServices>();

            //            //Gọi service xử lý
            //            string _accountKey = $"account:{ticketRequestData.Account}";
            //            string _lastRequestTimeKey = $"requestTimes:{ticketRequestData.Account}";

            //            var lastRequestTime = await _redisServices.GetValueAsync(_lastRequestTimeKey);
            //            if (string.IsNullOrEmpty(lastRequestTime) || DateTime.UtcNow - DateTime.Parse(lastRequestTime) > TimeSpan.FromSeconds(15))
            //            {
            //                Console.WriteLine("1");
            //                await _redisServices.SetValueAsync(_accountKey, "1", TimeSpan.FromHours(1));
            //                await _redisServices.SetValueAsync(_lastRequestTimeKey, DateTime.UtcNow.ToString(), TimeSpan.FromHours(1));
            //            }
            //            else
            //            {
            //                var requestCount = int.Parse(await _redisServices.GetValueAsync(_accountKey));
            //                Console.WriteLine("REQUEST COUNT - " + requestCount);
            //                if (requestCount >= 5)
            //                {
            //                    context.Response.ContentType = "application/json";
            //                    context.Response.StatusCode = StatusCodes.Status200OK;
            //                    await context.Response.WriteAsync(JsonConvert.SerializeObject(new ResponseDTO { IsSuccess = false, Message = "#2 - Số lượt truy vấn đã quá giới hạn. Quý khách vui lòng thử lại sau ít phút" }));
            //                    return; // Stop further processing
            //                }

            //                //// Tăng số yêu cầu lên 1
            //                requestCount += 1;
            //                await _redisServices.SetValueAsync(_accountKey, requestCount.ToString(), TimeSpan.FromHours(1));
            //                await _redisServices.SetValueAsync(_lastRequestTimeKey, DateTime.UtcNow.ToString(), TimeSpan.FromHours(1));
            //            }
            //        }
            //    }
            //}
            await _next(context);
        }
    }
}
