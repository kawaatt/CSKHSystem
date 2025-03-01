using ADMIN.Constant;
using ADMIN.Models.OidcModel;
using ADMIN.Services;
using ADMIN.Constant;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using System.Net;
using Microsoft.AspNetCore.Http.Extensions;
using ADMIN.Models.DTO;
using Microsoft.AspNetCore.Http;

namespace ADMIN.Middleware
{
    public class TokenProcessingMiddleware
    {
        private readonly RequestDelegate _next;
        
        public TokenProcessingMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            if (context.User.Identity.IsAuthenticated)
            {
                var routeData = context.GetRouteData();
                if (routeData != null && routeData.Values.Count!=0 && routeData.Values["controller"].ToString() != "Error")
                {
                    var _authServices = context.RequestServices.GetRequiredService<IAuthServices>();
                    ResponseDTO _checkTokenValidateRes = await _authServices.PingServer();
                    Console.WriteLine(JsonConvert.SerializeObject(_checkTokenValidateRes));
                    if (_checkTokenValidateRes == null || _checkTokenValidateRes.IsSuccess == false)
                    {
                        context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
                        context.Response.Redirect("/Error/Unauthorized");
                        await context.Response.CompleteAsync();
                        return;
                    }
                }
            }
            await _next(context);
        }
    }

}
