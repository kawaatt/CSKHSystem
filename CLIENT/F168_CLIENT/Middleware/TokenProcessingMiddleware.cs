using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using SHBET_CLIENT.Constant;
using SHBET_CLIENT.Models;
using SHBET_CLIENT.Models.DTO;
using SHBET_CLIENT.Provider;
using SHBET_CLIENT.Services;
using System.Net;
using System.Net.Http;

namespace SHBET_CLIENT.Middleware
{
    public class TokenProcessingMiddleware
    {
        private readonly RequestDelegate _next;
        private IAuthServices _authServices;
        private ITokenProvider _tokenProvider;
        private IHttpContextAccessor _httpContextAccessor;
        private IBaseServices _baseServices;

        public TokenProcessingMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync (HttpContext context)
        {
            var routeData = context.GetRouteData();
            if (routeData != null && routeData.Values.Count != 0 && routeData.Values["controller"].ToString() != "Error")
            {
                _tokenProvider = context.RequestServices.GetRequiredService<ITokenProvider>();
                _authServices = context.RequestServices.GetRequiredService<IAuthServices>();

                string AccessToken = await _tokenProvider.GetToken();
                
                if (string.IsNullOrEmpty(AccessToken))
                {
                    ResponseDTO getTokenRes = await _authServices.GetClientToken();
                    Console.WriteLine("getTokenRes " + JsonConvert.SerializeObject(getTokenRes));
                    if (getTokenRes != null && getTokenRes.IsSuccess == true)
                    {
                        TokenResponse tokenResponse = JsonConvert.DeserializeObject<TokenResponse>(getTokenRes.Result.ToString());
                        _tokenProvider.SetToken(tokenResponse.access_token);
                        await _next(context);
                        return;
                    }

                    context.Response.StatusCode = (int)HttpStatusCode.Unauthorized;
                    context.Response.Redirect("/Error/Unauthorized");
                    await context.Response.CompleteAsync();
                    return;
                }
                else
                {
                    ResponseDTO _checkTokenValidateRes = await _authServices.PingServer();
                    if (_checkTokenValidateRes != null && _checkTokenValidateRes.IsSuccess == true)
                    {
                        //TODO: Will handle later
                        await _next(context);
                    }
                    else 
                    {
                        switch (_checkTokenValidateRes.StatusCode)
                        {
                            case 401:
                                Console.WriteLine("401 CATCHED");
                                ResponseDTO getTokenRes = await _authServices.GetClientToken();
                                if (getTokenRes != null && getTokenRes.IsSuccess == true && getTokenRes.Result != null)
                                {
                                    TokenResponse tokenResponse = JsonConvert.DeserializeObject<TokenResponse>(getTokenRes.Result.ToString());
                                    _tokenProvider.SetToken(tokenResponse.access_token);
                                    await _next(context);
                                    return;
                                }
                                context.Response.StatusCode = (int)HttpStatusCode.Unauthorized;
                                context.Response.Redirect("/Error/Unauthorized");
                                break;
                            case 404:
                                Console.WriteLine("NOTFOUND CATCHED");
                                context.Response.StatusCode = (int)HttpStatusCode.NotFound;
                                context.Response.Redirect("~/Error/NotFound");
                                break;
                            default:
                                Console.WriteLine("DEFAULT CATCHED");
                                context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
                                context.Response.Redirect("/Error/InternalServerError");
                                break;
                        }
                        await context.Response.CompleteAsync();
                        return;
                    }
                }
            }
            else
            {
                await _next(context);
            }
          //  await _next(context);
        }
    }

}
