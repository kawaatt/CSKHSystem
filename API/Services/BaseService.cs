using API.Models.DTO;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using System.Text;
using static API.Constant.SD;

namespace API.Services
{
    public interface IBaseServices
    {
        Task<ResponseDTO?> SendAsync(RequestDTO requestDTO);
        Task<ResponseDTO?> SendFormURLEncodeContentAsync(RequestDTO requestDTO, bool withBearer = true);
    }

    public class BaseServices : IBaseServices
    {
        private readonly IHttpClientFactory _httpclientFactory;
        private readonly IUserContextService _userContextService;

        public BaseServices(IHttpClientFactory httpClientFactory, IUserContextService UserContextService)
        {
            _httpclientFactory = httpClientFactory;
            _userContextService = UserContextService;
        }

        public async Task<ResponseDTO?> SendFormURLEncodeContentAsync(RequestDTO requestDTO, bool withBearer = true)
        {
            try
            {
                HttpClient client = _httpclientFactory.CreateClient();
                HttpRequestMessage message = new HttpRequestMessage();
                message.RequestUri = new Uri(requestDTO.Url);

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
                        var responseDTO = JsonConvert.DeserializeObject<ResponseDTO>(apiContent);
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

        public async Task<ResponseDTO?> SendAsync(RequestDTO requestDTO)
        {
            try
            {
                HttpClient client = _httpclientFactory.CreateClient();
                HttpRequestMessage message = new HttpRequestMessage();
                message.RequestUri = new Uri(requestDTO.Url);

                if (requestDTO.Data != null)
                {
                    message.Content = new StringContent(JsonConvert.SerializeObject(requestDTO.Data), Encoding.UTF8, "application/json");
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
