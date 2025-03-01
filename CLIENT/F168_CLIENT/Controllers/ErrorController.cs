using Microsoft.AspNetCore.Mvc;

namespace SHBET_CLIENT.Controllers
{
    public class ErrorController : Controller
    {
        public IActionResult Unauthorized()
        {
            return View(); // Tương ứng với View: Views/Error/Unauthorized.cshtml
        }

        public IActionResult NotFound()
        {
            return View(); // Tương ứng với View: Views/Error/NotFound.cshtml
        }

        public IActionResult InternalServerError()
        {
            Console.WriteLine("HERE");
            return View("InternalServerError");
        }
    }
}
