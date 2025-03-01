using ADMIN.Models.ViewModel.AplicationVm;
using ADMIN.Models.ViewModel.RequestResponseVm;
using ADMIN.Provider;
using ADMIN.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace ADMIN.Controllers
{
    [Authorize]
    public class BaseController: Controller
    {
        private readonly ITokenProvider _tokenProvider;
        public BaseController(ITokenProvider tokenProvider)
        {
            _tokenProvider = tokenProvider;
        }

        public override void OnActionExecuted(ActionExecutedContext context)
        {
            Console.WriteLine("1");
            if (HttpContext.User.Identity.IsAuthenticated)
            {
                if (string.IsNullOrEmpty(_tokenProvider.RefreshTokenAsync().Result))
                {
                    context.Result = RedirectToAction("Logout", "Auth");
                }
            }

            base.OnActionExecuted(context);
        }
    }
}
