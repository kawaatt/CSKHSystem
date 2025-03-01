using ADMIN.Models.OidcModel;
using ADMIN.Services;
using ADMIN.Models.OidcModel;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using ADMIN.Models.DTO;

namespace ADMIN.Component
{
    public class TopMenuViewComponent : ViewComponent
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IUserContextService _userContextService;

        public TopMenuViewComponent(IHttpContextAccessor httpContextAccessor, IUserContextService userContextService)
        {
            _httpContextAccessor = httpContextAccessor;
            _userContextService = userContextService;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            var userDto = new UserDTO();

            if (!_httpContextAccessor.HttpContext.User.Identity.IsAuthenticated)
                return View(userDto);

            var currentUser = await _userContextService.GetUserInfoAsync();

            userDto = currentUser;
            return View(userDto);
        }
    }
}
