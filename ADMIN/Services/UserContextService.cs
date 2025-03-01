using ADMIN.Constant;
using ADMIN.Models.DTO;
using ADMIN.Models.OidcModel;
using ADMIN.Provider;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace ADMIN.Services
{
    public interface IUserContextService
    {
        Task<UserDTO> GetUserInfoAsync();
    }
    public class UserContextService : IUserContextService
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ITokenProvider _tokenProvider;
        IdentityConstant _options;

        public UserContextService(IHttpContextAccessor httpContextAccessor, ITokenProvider tokenProvider, IOptions<IdentityConstant> options)
        {
            _httpContextAccessor = httpContextAccessor;
            _tokenProvider = tokenProvider;
            _options = options.Value;
        }

        public async Task<UserDTO> GetUserInfoAsync()
        {
            UserDTO myUserResult = new UserDTO();

            if (!_httpContextAccessor.HttpContext.User.Identity.IsAuthenticated) return myUserResult;

            var accessToken = await _tokenProvider.RefreshTokenAsync();
            if (string.IsNullOrEmpty(accessToken))
                return myUserResult;

            var claimsPrincipal = _httpContextAccessor.HttpContext?.User;
            myUserResult.TenantId = claimsPrincipal.FindFirst(claim => claim.Type == ClaimConstant.TenantId).Value;
            myUserResult.UserName = claimsPrincipal.FindFirst(claim => claim.Type == ClaimConstant.UserName).Value;
            myUserResult.Role = claimsPrincipal.FindFirst(claim => claim.Type == ClaimConstant.Role).Value;
            var userFullInfor = claimsPrincipal.FindFirst(claim => claim.Type == ClaimConstant.UserFullInfor).Value;
            myUserResult.UserInfor = JsonSerializer.Deserialize<UserInforVm>(userFullInfor);
            myUserResult.Permissions = claimsPrincipal.Claims.Where(c => c.Type == ClaimConstant.Permissions).Select(c => c.Value).ToList();

            //get user allocation
            var claimUserAllocation = claimsPrincipal.FindFirst(claim => claim.Type == ClaimConstant.UserAllocationData).Value;
            var options = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true,  // Không phân biệt chữ hoa thường
                DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull // Bỏ qua null
            };

            myUserResult.UserAllocationInfors = JsonSerializer.Deserialize<UserAllocationClaimVm>(claimUserAllocation ?? "", options);
            if (myUserResult.UserAllocationInfors != null)
            {
                myUserResult.SitesCollection = myUserResult.UserAllocationInfors.AllocationSites.Select(x => x.Name.ToUpper()).ToList();

                //Console.WriteLine(Newtonsoft.Json.JsonConvert.SerializeObject(_options));
                myUserResult.Site = _httpContextAccessor.HttpContext.Session.GetString(_options.SessionSiteActive);
                if (myUserResult.Site.IsNullOrEmpty())
                {
                    myUserResult.Site = myUserResult.UserAllocationInfors.AllocationSites.Select(x => x.Name).FirstOrDefault();
                }
                _httpContextAccessor.HttpContext.Session.SetString(_options.SessionSiteActive, myUserResult.Site ?? "");
            }
            //myUserResult.ProjectCode = myUserResult.Site;
            myUserResult.AccessToken = accessToken;
            return myUserResult;
        }
    }

}
