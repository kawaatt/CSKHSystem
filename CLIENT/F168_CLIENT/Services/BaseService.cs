using Newtonsoft.Json;
using SHBET_CLIENT.Models.DTO;
using SHBET_CLIENT.Provider;
using System.Text;
using System.Text.Json.Nodes;
using static SHBET_CLIENT.Constant.SD;

namespace SHBET_CLIENT.Services
{
    public interface IBaseServices
    {
        Task<ResponseDTO?> BaseSendAsync(RequestDTO requestDTO, bool withBearer = true);
        Task<ResponseDTO?> LiveSendFormURLEncodeContentAsync(RequestDTO requestDTO, bool withBearer = true);
    }

    public class BaseServices : IBaseServices
    {
        private readonly IHttpClientFactory _httpclientFactory;
        private readonly ITokenProvider _tokenProvider;

        public BaseServices(IHttpClientFactory httpClientFactory, ITokenProvider tokenProvider)
        {
            _httpclientFactory = httpClientFactory;
            _tokenProvider = tokenProvider;
        }

        public async Task<ResponseDTO?> LiveSendFormURLEncodeContentAsync(RequestDTO requestDTO, bool withBearer = true)
        {
            try
            {
                HttpClient client = _httpclientFactory.CreateClient();
                HttpRequestMessage message = new HttpRequestMessage();
                message.RequestUri = new Uri(requestDTO.Url);

                if (withBearer)
                {
                    var Token = await _tokenProvider.GetToken();
                    message.Headers.Add("Authorization", $"Bearer {Token}");
                }

                var payloadData = new List<KeyValuePair<string, string>>();
                foreach (var prop in requestDTO.Data.GetType().GetProperties())
                {
                    payloadData.Add(new KeyValuePair<string, string>(prop.Name, prop.GetValue(requestDTO.Data).ToString()));
                }    
                message.Content = new FormUrlEncodedContent(payloadData);

                HttpResponseMessage apiResponse = null;
                switch (requestDTO.APIType)
                {
                    case APIType.POST:
                        message.Method = HttpMethod.Post;
                        break;
                    case APIType.DELETE:
                        message.Method = HttpMethod.Delete;
                        break;
                    case APIType.PUT:
                        message.Method = HttpMethod.Put;
                        break;
                    default:
                        message.Method = HttpMethod.Get;
                        break;
                }

                apiResponse = await client.SendAsync(message);
                switch (apiResponse.StatusCode)
                {
                    case System.Net.HttpStatusCode.NotFound:
                        return new() { IsSuccess = false, Message = "Not Found" };
                    case System.Net.HttpStatusCode.Forbidden:
                        return new() { IsSuccess = false, Message = "Access Denied" };
                    case System.Net.HttpStatusCode.Unauthorized:
                        return new() { IsSuccess = false, Message = "Unauthorized" };
                    case System.Net.HttpStatusCode.InternalServerError:
                        return new() { IsSuccess = false, Message = "Internal Server Error" };
                    default:
                        apiResponse.EnsureSuccessStatusCode();
                        var apiContent = await apiResponse.Content.ReadAsStringAsync();
                        ResponseDTO responseDTO = new ResponseDTO { Result = JsonConvert.DeserializeObject<dynamic>(apiContent)};
                        return responseDTO;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                var dto = new ResponseDTO
                {
                    Message = ex.Message.ToString(),
                    IsSuccess = false
                };
                return dto;
            }
        }

        public async Task<ResponseDTO?> BaseSendAsync(RequestDTO requestDTO, bool withBearer = true)
        {
            try
            {
                HttpClient client = _httpclientFactory.CreateClient();
                HttpRequestMessage message = new HttpRequestMessage();
                message.RequestUri = new Uri(requestDTO.Url);
                if (requestDTO.ContentType == ContentType.MultiPartFormData || requestDTO.ContentType == ContentType.ExcelFile)
                {
                    message.Headers.Add("Accept", "*/*");
                }
                else
                {
                    message.Headers.Add("Accept", "application/json");
                }

                if (withBearer)
                {
                    var Token = await _tokenProvider.GetToken();
                    message.Headers.Add("Authorization", $"Bearer {Token}");
                }

                if (requestDTO.ContentType == ContentType.MultiPartFormData)
                {
                    var content = new MultipartFormDataContent();
                    foreach (var prop in requestDTO.Data.GetType().GetProperties())
                    {
                        var value = prop.GetValue(requestDTO.Data);
                        if (value is FormFile)
                        {
                            var file = (FormFile)value;
                            if (file != null)
                            {
                                content.Add(new StreamContent(file.OpenReadStream()), prop.Name, file.FileName);
                            }
                        }
                        else
                        {
                            if (value == null)
                            {
                                content.Add(new StringContent(""), prop.Name);
                            }
                            else
                            {
                                content.Add(new StringContent(value.ToString()!), prop.Name);
                            }
                        }
                    }
                    message.Content = content;
                }
                else
                {
                    if (requestDTO.Data != null)
                    {
                        message.Content = new StringContent(JsonConvert.SerializeObject(requestDTO.Data), Encoding.UTF8, "application/json");
                    }
                }

                HttpResponseMessage apiResponse = null;
                switch (requestDTO.APIType)
                {
                    case APIType.POST:
                        message.Method = HttpMethod.Post;
                        break;
                    case APIType.DELETE:
                        message.Method = HttpMethod.Delete;
                        break;
                    case APIType.PUT:
                        message.Method = HttpMethod.Put;
                        break;
                    default:
                        message.Method = HttpMethod.Get;
                        break;
                }

                //Console.WriteLine("REQUEST  =>>  " + JsonConvert.SerializeObject(message,Formatting.Indented));
                apiResponse = await client.SendAsync(message);
                //Console.WriteLine("RESPONSE  =>>  " + JsonConvert.SerializeObject(apiResponse));
                if (apiResponse.IsSuccessStatusCode == true && requestDTO.ContentType == ContentType.ExcelFile)
                {
                    return new ResponseDTO { IsSuccess = true, Message = "Export Successful", Result = await apiResponse.Content.ReadAsStreamAsync() };
                }
                else
                {
                    switch (apiResponse.StatusCode)
                    {
                        case System.Net.HttpStatusCode.NotFound:
                            return new() { StatusCode = StatusCodes.Status404NotFound, IsSuccess = false, Message = "Not Found" };
                        case System.Net.HttpStatusCode.Forbidden:
                            return new() { StatusCode = StatusCodes.Status403Forbidden, IsSuccess = false, Message = "Access Denied" };
                        case System.Net.HttpStatusCode.Unauthorized:
                            return new() { StatusCode = StatusCodes.Status401Unauthorized, IsSuccess = false, Message = "Unauthorized" };
                        case System.Net.HttpStatusCode.InternalServerError:
                            return new() { StatusCode = StatusCodes.Status500InternalServerError, IsSuccess = false, Message = "Internal Server Error" };
                        default:
                            apiResponse.EnsureSuccessStatusCode();
                            var apiContent = await apiResponse.Content.ReadAsStringAsync();
                            var responseDTO = JsonConvert.DeserializeObject<ResponseDTO>(apiContent);
                            return responseDTO;
                    }
                }
            }
            catch (Exception ex)
            {
                //TODO: route to ErrorHandlingMiddleware to handle
                //Console.WriteLine(ex.ToString());
                var dto = new ResponseDTO
                {
                    StatusCode = StatusCodes.Status500InternalServerError,
                    Message = ex.Message.ToString(),
                    IsSuccess = false
                };
                return dto;
            }

        }
    }
}
