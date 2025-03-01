using ADMIN.Constant;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace ADMIN.Controllers
{
    public class AuthController(IOptions<IdentityConstant> options) : Controller
    {
        IdentityConstant _options = options.Value;
        public IActionResult Index()
        {
            return View();
        }
        [HttpGet]
        public async Task<IActionResult> Logout()
        {
            var idToken = await HttpContext.GetTokenAsync("id_token");
            await HttpContext.SignOutAsync("Cookies");

            var logoutUrl = $"{_options.IdentityUrl}/connect/endsession";

            //// If you have an id_token_hint, append it to the logout URL
            if (!string.IsNullOrEmpty(idToken))
            {
                logoutUrl = $"{logoutUrl}?id_token_hint={idToken}&post_logout_redirect_uri={_options.SignOutCallBack}";
            }
            else
            {
                // If no id_token is found, you can still log out, but it will be a more general session logout
                logoutUrl = $"{logoutUrl}?post_logout_redirect_uri={_options.SignOutCallBack}";
            }

            //// Redirect to IdentityServer to initiate the logout process
            return Redirect(logoutUrl);
        }
    }
}
