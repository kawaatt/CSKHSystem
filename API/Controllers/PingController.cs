using API.Models.CSKHAuto;
using API.Models.DTO;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [ApiController]
    [Route("api/ping")]
    //[Authorize]
    public class PingController : Controller
    {
        [HttpGet]
        public async Task<ResponseDTO> Index()
        {
            return new ResponseDTO();
        }
    }
}
