using ADMIN.Models.DTO;
using ADMIN.Models.ViewModel.HtmlViewModel;
using ADMIN.Services;
using Microsoft.AspNetCore.Mvc;

namespace ADMIN.Component
{
    public class SideBarViewComponent : ViewComponent
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IUserContextService _userContextService;

        public SideBarViewComponent(IHttpContextAccessor httpContextAccessor, IUserContextService userContextService)
        {
            _httpContextAccessor = httpContextAccessor;
            _userContextService = userContextService;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            List<MenuItem> menuList = new List<MenuItem>();
            
            if (!_httpContextAccessor.HttpContext.User.Identity.IsAuthenticated)
                return View(menuList);

            //Sync UserDTO
            var currentUser = await _userContextService.GetUserInfoAsync();
            
            //Console.WriteLine(JsonConvert.SerializeObject(currentUser));

            //Prepare data for Sidebar Menu
            if (CheckPermission(currentUser, $"{currentUser.Site}_TICKET.VIEW"))
                menuList.Add(new MenuItem { Name = "Ticket Management", Action = "Index", Controller = "Ticket", ProjectCode = $"{currentUser.Site}" });
            if (CheckPermission(currentUser, $"{currentUser.Site}_TELE_BOT.VIEW"))
                menuList.Add(new MenuItem { Name = "Telegram Bot", Action = "Index", Controller = "TeleBot", ProjectCode = $"{currentUser.Site}" });

            ViewBag.SiteActive = currentUser.Site;
            ViewBag.ProjectList = currentUser.SitesCollection;

            return View(menuList);
        }

        private bool CheckPermission(UserDTO currentUser, string permissionCheck)
        {
            var permissions = currentUser.Permissions;
            if (permissions.Contains(permissionCheck)) 
                return true;
            return false;
        }
    }
}
