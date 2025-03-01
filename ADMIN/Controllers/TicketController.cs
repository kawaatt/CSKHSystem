using ADMIN.Controllers;
using ADMIN.Provider;
using ADMIN.Constant;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using System.Diagnostics;
using ADMIN.Models.DTO;
using ADMIN.Services;
using Newtonsoft.Json;
using ADMIN.Models.CSKHAuto;
using Microsoft.AspNetCore.Http.HttpResults;

namespace ADMIN.Controllers
{
    public class TicketController : BaseController
    {
        private readonly ITokenProvider _tokenProvider;
        private ResponseDTO _res;
        private readonly ITicketServices _ticketServices;
        private readonly IUserContextService _userContextServices;

        public TicketController(ITokenProvider tokenProvider, ITicketServices iTicketServices, IUserContextService userContextServices) : base(tokenProvider)
        {
            _tokenProvider = tokenProvider;
            _res = new ResponseDTO();
            _ticketServices = iTicketServices;
            _userContextServices = userContextServices;
        }

        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> GetTicketTableData(string SystemApp, int Status)
        {
            List<TicketRequestDTO> _result = new List<TicketRequestDTO>();
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
                Console.WriteLine(_sortColumn + "|" + _sortColumnDirection);

                _res = await _ticketServices.GetTicketData(iParameters);
                Console.WriteLine(JsonConvert.SerializeObject(_res));
                if (_res != null && _res.IsSuccess == true)
                {
                    _result = JsonConvert.DeserializeObject<List<TicketRequestDTO>>(_res.Result.ToString());
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
        public async Task<IActionResult> GetTicketByID(string TicketID,string System)
        {
            _res = await _ticketServices.GetTicketByID(TicketID, System);
            Console.WriteLine(JsonConvert.SerializeObject(_res));
            if (_res == null || _res.IsSuccess == false)
            {
                _res.IsSuccess = false;
                _res.Message = "NO DATA FOUND";
            }

            TicketRequestDTO ticketRequestDTO = JsonConvert.DeserializeObject<TicketRequestDTO>(_res.Result.ToString());
            return PartialView("_ProcessModal", ticketRequestDTO);
        }

        [HttpPost]
        public async Task<IActionResult> AcceptTicketProcess(string TicketID, string System)
        {
            var currentUser = await _userContextServices.GetUserInfoAsync();
            _res = await _ticketServices.AcceptTicketProcess(TicketID, System, currentUser.UserName);
            Console.WriteLine(JsonConvert.SerializeObject(_res));
            if (_res == null || _res.IsSuccess == false)
            {
                _res.IsSuccess = false;
                _res.Message = "QUERY ERROR";
                return PartialView("_ProcessModal", null);
            }

            TicketRequestDTO ticketRequestDTO = JsonConvert.DeserializeObject<TicketRequestDTO>(_res.Result.ToString());
            return PartialView("_ProcessModal", ticketRequestDTO);
        }

        [HttpPost]
        public async Task<IActionResult> UpdateTicketProcess(string TicketID, string System, string CheckedResult)
        {
            var currentUser = await _userContextServices.GetUserInfoAsync();
            _res = await _ticketServices.UpdateTicketProcess(TicketID, currentUser.UserName, CheckedResult, System);
            Console.WriteLine(JsonConvert.SerializeObject(_res));
            if (_res == null || _res.IsSuccess == false)
            {
                _res.IsSuccess = false;
                _res.Message = "QUERY ERROR";
                return PartialView("_ProcessModal", null);
            }

            TicketRequestDTO ticketRequestDTO = JsonConvert.DeserializeObject<TicketRequestDTO>(_res.Result.ToString());
            return PartialView("_ProcessModal", ticketRequestDTO);
        }

        [HttpPost]
        public async Task<IActionResult> FinishTicketProcess(string TicketID, string System, string CheckedResult)
        {
            var currentUser = await _userContextServices.GetUserInfoAsync();
            _res = await _ticketServices.FinishTicketProcess(TicketID, currentUser.UserName, CheckedResult, System);
            Console.WriteLine(JsonConvert.SerializeObject(_res));
            if (_res == null || _res.IsSuccess == false)
            {
                _res.IsSuccess = false;
                _res.Message = "QUERY ERROR";
                return PartialView("_ProcessModal", null);
            }

            TicketRequestDTO ticketRequestDTO = JsonConvert.DeserializeObject<TicketRequestDTO>(_res.Result.ToString());
            return PartialView("_ProcessModal", ticketRequestDTO);
        }
        
    }
}
