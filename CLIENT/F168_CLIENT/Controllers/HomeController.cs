using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using SHBET_CLIENT.Models;
using SHBET_CLIENT.Models.DTO;
using SHBET_CLIENT.Services;
using System.Diagnostics;
using System.Security.Principal;
using System.Text.Json.Serialization;

namespace SHBET_CLIENT.Controllers
{
    public class HomeController : Controller
    {
        private ResponseDTO _responseDTO;
        private readonly ITicketServices _ticketServices;
        
        public HomeController(ITicketServices ticketServices)
        {
            _responseDTO = new ResponseDTO();
            _ticketServices = ticketServices;
        }

        public async Task<IActionResult> Index()
        {

            return View();
        }

        public async Task<IActionResult> SendTicketRequest(string account, string ticketContent, string cardHolder, string imageBase64,Guid ticketCategoryID)
        {
            try
            {
                var form = HttpContext.Request.Form;

                TicketRequestDTO _ticketRequestDTO = new TicketRequestDTO
                {
                    Account = account,
                    TicketContent = ticketContent,
                    System = "F168",
                    ImageURL = null,
                    ImageBase64 = imageBase64,
                    RequestDate = null,
                    CardHolder = cardHolder,
                    TicketCategoryID = ticketCategoryID
                };
                Console.WriteLine(JsonConvert.SerializeObject(_ticketRequestDTO, Formatting.Indented));
                ResponseDTO _sendTicketRes = await _ticketServices.SendTicketRequest(_ticketRequestDTO);
                if (_sendTicketRes == null || _sendTicketRes.IsSuccess == false)
                {
                    _responseDTO.IsSuccess = false;
                    _responseDTO.Message = _sendTicketRes.Message;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                _responseDTO.IsSuccess = false;
                _responseDTO.Message = "Lỗi hệ thống. Quý khách vui lòng gửi yêu cầu hỗ trợ sau ít phút";
            }

            return Json(_responseDTO);
        }

        public async Task<IActionResult> SendCheckAccount(string Account, string CardHolder)
        {
            Console.WriteLine("SendCheckAccount");
            try
            {
                ResponseDTO _sendTicketRes = await _ticketServices.CheckAccountRequest(Account,CardHolder); 
                Console.WriteLine(JsonConvert.SerializeObject(_sendTicketRes, Formatting.Indented));
                if (_sendTicketRes == null || _sendTicketRes.IsSuccess == false)
                {
                    _responseDTO.IsSuccess = false;
                    _responseDTO.Message = _sendTicketRes.Message;
                }
                else
                {
                    _responseDTO.Result = JsonConvert.DeserializeObject<List<TicketRequestDTO>>(_sendTicketRes.Result.ToString());
                }
            }
            catch (Exception ex)
            {
                _responseDTO.IsSuccess = false;
                _responseDTO.Message = "Lỗi hệ thống. Quý khách vui lòng gửi yêu cầu hỗ trợ sau ít phút";
            }

            return Json(_responseDTO);
        }

        [HttpPost]
        public async Task<IActionResult> GetTicketByID(Guid TicketID)
        {
            try
            {
                ResponseDTO _getTicketByIDRes = await _ticketServices.GetTicketByID(TicketID);
                if (_getTicketByIDRes == null || _getTicketByIDRes.IsSuccess == false)
                {
                    _responseDTO.IsSuccess = false;
                    _responseDTO.Message = _getTicketByIDRes.Message;
                }
                else
                {
                    _responseDTO.Result = JsonConvert.DeserializeObject<TicketRequestDTO>(_getTicketByIDRes.Result.ToString());
                }
            }
            catch (Exception ex)
            {
                _responseDTO.IsSuccess = false;
                _responseDTO.Message = "Lỗi hệ thống. Quý khách vui lòng gửi yêu cầu hỗ trợ sau ít phút";
            }
            Console.WriteLine(JsonConvert.SerializeObject(_responseDTO,Formatting.Indented));
            return Json(_responseDTO);
        }
    }
}
