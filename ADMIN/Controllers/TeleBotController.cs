using ADMIN.Constant;
using ADMIN.Models.BotTele;
using ADMIN.Models.CSKHAuto;
using ADMIN.Models.DTO;
using ADMIN.Models.ViewModel.HtmlViewModel;
using ADMIN.Provider;
using ADMIN.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using System.Reflection;
using System.Runtime.ConstrainedExecution;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace ADMIN.Controllers
{
    public class TeleBotController: BaseController
    {
        private readonly ITokenProvider _tokenProvider;
        private ResponseDTO _res;
        //private readonly ITicketServices _ticketServices;
        private readonly IUserContextService _userContextServices;
        private readonly ITeleBotService _telebotServices;


        private readonly ApiEndPointConstant _apiEndPointConstant;

        public TeleBotController(ITokenProvider tokenProvider, ITeleBotService telebotServices, IUserContextService userContextServices, IOptions<ApiEndPointConstant> apiEndPointConstant) : base(tokenProvider)
        {
            _tokenProvider = tokenProvider;
            _res = new ResponseDTO();
            _telebotServices = telebotServices;
            _userContextServices = userContextServices;
            _apiEndPointConstant = apiEndPointConstant.Value;
        }

        public async Task<IActionResult> Index()
        {
            var currentUser = await _userContextServices.GetUserInfoAsync();

            Console.WriteLine(JsonConvert.SerializeObject(currentUser));

            ViewBag.SiteActive = currentUser.Site;
            return View();
        }

        [HttpGet]
        public async Task<IActionResult> Setting(Guid ID, string SystemApp)
        {
            if (ID==Guid.Empty || SystemApp.IsNullOrEmpty())
            {
                throw new Exception("QUERY ERROR");
            }

            ResponseDTO _getTeleBotByIDRes = await _telebotServices.GetBotTeleByIDAsync(ID, SystemApp);
            if (_getTeleBotByIDRes == null && _getTeleBotByIDRes.IsSuccess == false)
            {
                throw new Exception("NO TELEGRAM BOT ACCOUNT EXITS");
            }
            TelegramAccountDTO _telegramAccountResultDTO = JsonConvert.DeserializeObject<TelegramAccountDTO>(_getTeleBotByIDRes.Result.ToString());
            ViewBag.TeleBot = _telegramAccountResultDTO;
            if (_telegramAccountResultDTO.BotType == 99)
            {
                ViewBag.HookingLink = $"{_apiEndPointConstant.API_BOT_SHARE_TELE_ENDPOINT}/api/Telegram/BotCheckAgent/{_telegramAccountResultDTO.System}/{_telegramAccountResultDTO.ID}";
            }else
            {     
                ViewBag.HookingLink = $"{_apiEndPointConstant.API_BOT_SHARE_TELE_ENDPOINT}/api/Telegram/BotShareTele/{_telegramAccountResultDTO.System}/{_telegramAccountResultDTO.ID}";
            }

            return View();
        }

        [HttpPost]
        public async Task<IActionResult> GetBotShareTeleTableData(string SystemApp, int Status)
        {
            List<TelegramAccountDTO> _result = new List<TelegramAccountDTO>();
            var _myDraw = "";
            try
            {
                _myDraw = Request.Form["draw"].FirstOrDefault();
                var _sortColumn = Request.Form["columns[" + Request.Form["order[0][column]"].FirstOrDefault() + "][name]"].FirstOrDefault();
                var _sortColumnDirection = Request.Form["order[0][dir]"].FirstOrDefault();
                var _searchValue = Request.Form["search[value]"].FirstOrDefault();
                int pageSize = Convert.ToInt32(Request.Form["length"].FirstOrDefault() ?? "0");
                int skip = Convert.ToInt32(Request.Form["start"].FirstOrDefault() ?? "0");

                QueryParameterDTO iParameters = new QueryParameterDTO();
                iParameters.System = SystemApp;
                iParameters.PageSize = pageSize;
                iParameters.SearchText = _searchValue;
                iParameters.SortBy = _sortColumn;
                iParameters.SortDirection = _sortColumnDirection;
                iParameters.Status = Status;
                //iParameters.StartTime = DateTime.Now;
                //iParameters.EndTime = DateTime.Now;
                //Console.WriteLine(_sortColumn + "|" + _sortColumnDirection);

                _res = await _telebotServices.GetBotTeleAsync(iParameters);
                if (_res != null && _res.IsSuccess == true)
                {
                    _result = JsonConvert.DeserializeObject<List<TelegramAccountDTO>>(_res.Result.ToString());
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }

            return Json(new
            {
                draw = _myDraw,
                recordsTotal = _res.TotalCount,
                recordsFiltered = _res.TotalCount,
                data = _result
            });
        }

        
        [HttpPost]
        [Route("/TeleBot/GetBotTeleByIDAsync")]
        public async Task<IActionResult> GetBotTeleByIDAsync(Guid BotID, string System)
        {
            try
            {
                ResponseDTO _getTeleBotByIDRes = await _telebotServices.GetBotTeleByIDAsync(BotID, System);
                if (_getTeleBotByIDRes == null && _getTeleBotByIDRes.IsSuccess == false)
                {
                    _res.IsSuccess = false;
                    _res.Message = _getTeleBotByIDRes.Message;
                }
                _res.Result = JsonConvert.DeserializeObject<TelegramAccountDTO>(_getTeleBotByIDRes.Result.ToString());
            }
            catch (Exception ex)
            {
                _res.IsSuccess = false;
                _res.Message = "Lỗi hệ thống - " + ex.Message;
            }
            return Json(_res);
        }

        [HttpPost]
        [Route("/TeleBot/SubmitTeleBotAccount")]
        public async Task<IActionResult> SubmitTeleBotAccount(string SystemApp)
        {
            try
            {
                string BotType = HttpContext.Request.Form["botType"].ToString();
                string BotUserName = HttpContext.Request.Form["botUserName"].ToString();
                string BotToken = HttpContext.Request.Form["botToken"].ToString();

                if (BotUserName.IsNullOrEmpty()
                    || BotToken.IsNullOrEmpty()
                    || SystemApp.IsNullOrEmpty()
                    || BotType.IsNullOrEmpty())
                {
                    _res.IsSuccess = false;
                    _res.Message = "Vui lòng điền đầy đủ thông tin";
                    return Json(_res);
                }

                string BotID = HttpContext.Request.Form["botId"].ToString();
                
                TelegramAccountDTO _botShareTeleDTO = new TelegramAccountDTO();
                _botShareTeleDTO.UserName = BotUserName;
                _botShareTeleDTO.Token = BotToken;
                _botShareTeleDTO.System = SystemApp;
                _botShareTeleDTO.BotType = Int32.Parse(BotType);

                if (BotID.IsNullOrEmpty())
                {
                    ResponseDTO _postRes = await _telebotServices.AddNewTelegramBotAccount(_botShareTeleDTO);
                    if (_postRes == null && _postRes.IsSuccess == false)
                    {
                        _res.IsSuccess = false;
                        _res.Message = _postRes.Message;
                    }
                }
                else
                {
                    _botShareTeleDTO.ID = Guid.Parse(BotID);
                    ResponseDTO _putRes = await _telebotServices.UpdateTelegramBotAccount(_botShareTeleDTO);
                    Console.WriteLine(JsonConvert.SerializeObject(_putRes));
                    if (_putRes == null && _putRes.IsSuccess == false)
                    {
                        _res.IsSuccess = false;
                        _res.Message = _putRes.Message;
                    }
                }
                
            }
            catch (Exception ex)
            {
                _res.IsSuccess = false;
                _res.Message = "Lỗi hệ thống - "+ ex.Message;
            }
            return Json(_res);
        }


        [HttpPost]
        public async Task<IActionResult> GetTelegramResponseTableData(string BotID)
        {
            List<TelegramResponseDTO> _result = new List<TelegramResponseDTO>();
            var _myDraw = "";
            try
            {
                _myDraw = Request.Form["draw"].FirstOrDefault();
                var _sortColumn = Request.Form["columns[" + Request.Form["order[0][column]"].FirstOrDefault() + "][name]"].FirstOrDefault();
                var _sortColumnDirection = Request.Form["order[0][dir]"].FirstOrDefault();
                var _searchValue = Request.Form["search[value]"].FirstOrDefault();
                int pageSize = Convert.ToInt32(Request.Form["length"].FirstOrDefault() ?? "0");
                int skip = Convert.ToInt32(Request.Form["start"].FirstOrDefault() ?? "0");

                QueryParameterDTO iParameters = new QueryParameterDTO();
                iParameters.System = BotID;
                iParameters.PageSize = pageSize;
                iParameters.SearchText = _searchValue;
                iParameters.SortBy = _sortColumn;
                iParameters.SortDirection = _sortColumnDirection;

                _res = await _telebotServices.GetTelegramResponseAsync(iParameters);
                Console.WriteLine(JsonConvert.SerializeObject(_res));
                if (_res != null && _res.IsSuccess == true)
                {
                    _result = JsonConvert.DeserializeObject<List<TelegramResponseDTO>>(_res.Result.ToString());
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            Console.WriteLine(JsonConvert.SerializeObject(_result, Formatting.Indented));
            return Json(new
            {
                draw = _myDraw,
                recordsTotal = _res.TotalCount,
                recordsFiltered = _res.TotalCount,
                data = _result
            });
        }


        [HttpPost]
        [Route("/TeleBot/SubmitTeleBotAccountSetting")]
        public async Task<IActionResult> SubmitTeleBotAccountSetting(string SystemApp)
        {
            try
            {
                var form = HttpContext.Request.Form;
                string ID = form["ID"].ToString();
                string UserName = form["UserName"].ToString();
                string BotToken = form["Token"].ToString();
                bool IsIndividualWorking = form["IsIndividualWorking"].ToString() == "on";
                string ChatID = form["ChatID"].ToString();
                string URLHooking = form["URLHooking"].ToString();

                string KeyBoardData = form["KeyboardData"].ToString();
                int BotType = Int32.Parse(form["BotType"].ToString());
                if (form["URLHooking"].ToString() == "BOT OCR")
                {
                    BotType = 1;
                }

                TelegramAccountDTO _botShareTeleDTO = new TelegramAccountDTO
                {
                    ID = Guid.Parse(ID),
                    UserName = UserName,
                    CreateDate = DateTime.Now,
                    Token = BotToken,
                    IsIndividualWorking = IsIndividualWorking,
                    ChatID = ChatID,
                    URLHooking = URLHooking,
                    BotType = BotType,
                    Status = true,
                    System = SystemApp,
                    KeyboardData = KeyBoardData
                };


                Console.WriteLine(JsonConvert.SerializeObject("\n\n\n\n\n" + JsonConvert.SerializeObject(_botShareTeleDTO)));

                ResponseDTO _putRes = await _telebotServices.UpdateTelegramBotAccount(_botShareTeleDTO);
                Console.WriteLine(JsonConvert.SerializeObject(_putRes));
                if (_putRes == null && _putRes.IsSuccess == false)
                {
                    _res.IsSuccess = false;
                    _res.Message = _putRes.Message;
                }
            }
            catch (Exception ex)
            {
                _res.IsSuccess = false;
                _res.Message = "Lỗi hệ thống - " + ex.Message;
            }
            return Json(_res);
        }


        [HttpPost]
        [Route("/TeleBot/SetTelegramHookURL")]
        public async Task<IActionResult> SetTelegramHookURL(Guid BotID, string System)
        {
            try
            {
                ResponseDTO _setHookRes = await _telebotServices.SetTelegramHook(BotID, System);
                Console.WriteLine(JsonConvert.SerializeObject(_setHookRes));
                if (_setHookRes == null && _setHookRes.IsSuccess == false)
                {
                    _res.IsSuccess = false;
                    _res.Message = _setHookRes.Message;
                }
            }
            catch (Exception ex)
            {
                _res.IsSuccess = false;
                _res.Message = "Lỗi hệ thống - " + ex.Message;
            }
            return Json(_res);
        }
        
        [HttpPost]
        [Route("/TeleBot/SubmitTelegramResponseFunction")]
        public async Task<IActionResult> SubmitTelegramResponseFunction()
        {
            try
            {
                Console.WriteLine("SubmitTelegramResponseFunction");
                var form = HttpContext.Request.Form;

                string ID = form["TelegramResponse_ID"];
                string botID = form["TelegramResponse_BotID"];
                string requestCode = form["TelegramResponse_RequestCode"];
                string imageURL = form["TelegramResponse_ImageURL"];
                string content = form["TelegramResponse_Content"];
                string inlineKeyboard = form["TelegramResponse_InlineKeyboard"];

                if (string.IsNullOrWhiteSpace(botID) || string.IsNullOrWhiteSpace(requestCode))
                {
                    _res.IsSuccess = false;
                    _res.Message = "Invalid request. No data received.";
                    return Json(_res);
                }

                TelegramResponseDTO responseDTO = new TelegramResponseDTO
                {
                    BotID = Guid.Parse(botID),
                    RequestCode = requestCode,
                    Content = content,
                    URLImage = imageURL,
                    InlineKeyboard = inlineKeyboard
                };
                Console.WriteLine(JsonConvert.SerializeObject(responseDTO, Formatting.Indented));

                ResponseDTO _submitTelegramResponseRes = new ResponseDTO();
                
                if (ID.IsNullOrEmpty())
                {
                    Console.WriteLine("Here");
                    _submitTelegramResponseRes = await _telebotServices.AddNewTelegramResponse(responseDTO);
                }
                else
                {
                    Console.WriteLine("Here1");
                    responseDTO.ID = Guid.Parse(ID);
                    _submitTelegramResponseRes = await _telebotServices.UpdateTelegramResponse(responseDTO);
                }

                if (_submitTelegramResponseRes == null || !_submitTelegramResponseRes.IsSuccess)
                {
                    _res.IsSuccess = false;
                    _res.Message = _submitTelegramResponseRes.Message;
                }
            }
            catch (Exception ex)
            {
                _res.IsSuccess = false;
                _res.Message = "System error - " + ex.Message;
            }
            return Json(_res);
        }

        [HttpPost]
        [Route("/TeleBot/GetTeleResponseByID")]
        public async Task<IActionResult> GetTeleResponseByID(Guid ID)
        {
            try
            {
                // Call a service method to handle the data (if needed)
                ResponseDTO _getTelegramResposeByIDRes = await _telebotServices.GetTelegramResponseByIDAsync(ID);
                if (_getTelegramResposeByIDRes == null || !_getTelegramResposeByIDRes.IsSuccess)
                {
                    _res.IsSuccess = false;
                    _res.Message = _getTelegramResposeByIDRes.Message;
                }
                else
                {
                    _res.Result = JsonConvert.DeserializeObject<TelegramResponseDTO>(_getTelegramResposeByIDRes.Result.ToString());
                }
            }
            catch (Exception ex)
            {
                _res.IsSuccess = false;
                _res.Message = "System error - " + ex.Message;
            }
            return Json(_res);
        }
    }
}
