using ADMIN.Controllers;
using ADMIN.Provider;
using ADMIN.Constant;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using System.Diagnostics;
using ADMIN.Models.DTO;

namespace ADMIN.Controllers
{
    public class HomeController : BaseController
    {
        private readonly ITokenProvider _tokenProvider;
        private IdentityConstant _options;
        private ResponseDTO _responseDTO;

        public HomeController(ITokenProvider tokenProvider, IOptions<IdentityConstant> options) : base(tokenProvider)
        {
            _tokenProvider = tokenProvider;
            _options = options.Value;
            _responseDTO = new ResponseDTO();
        }

        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> ActiveMenuSelect(string MenuSelect)
        {
            try
            {
                HttpContext.Session.SetString(_options.SessionSiteActive, MenuSelect ?? "");
                TempData["success"] = $"{MenuSelect} đã được kích hoạt";
            }
            catch (Exception ex)
            {
                TempData["error"] = ex.Message;
            }
            
            return Ok();
        }
    }
}
