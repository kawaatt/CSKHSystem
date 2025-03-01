using API.Constant;
using API.Data;
using API.Models.OidcModel;
using AutoMapper;
using Azure.Core;
using Microsoft.AspNetCore.Authentication;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System.IdentityModel.Tokens.Jwt;

namespace API.Services
{
    public interface IUserContextService
    {
        Task<UserDTO> GetUserInfoByTokenAsync();
    }

    public class UserContextService(IHttpContextAccessor httpContextAccessor, AppDbContext DBContext, IMapper mapper) : IUserContextService
    {
        private readonly IHttpContextAccessor _httpContextAccessor = httpContextAccessor;
        private readonly AppDbContext _dbContext = DBContext;
        private IMapper _mapper = mapper;

        public async Task<UserDTO> GetUserInfoByTokenAsync()
        {
            UserDTO myUserResult = new UserDTO();

            if (!_httpContextAccessor.HttpContext.User.Identity.IsAuthenticated) return myUserResult;
            //var userContext = _httpContextAccessor.HttpContext.User;
            //var accessToken = _httpContextAccessor.HttpContext.GetTokenAsync("access_token").Result;
            var accessToken = _httpContextAccessor.HttpContext.GetTokenAsync("access_token").Result;
            if (string.IsNullOrEmpty(accessToken))
                return myUserResult;

            var handler = new JwtSecurityTokenHandler();
            var token = handler.ReadJwtToken(accessToken);
            // Truy xuất các claims
            var claims = token.Claims.Select(c => new { c.Type, c.Value }).ToList();

            //var claimsPrincipal = _httpContextAccessor.HttpContext?.User;
            myUserResult.TenantId = claims.FirstOrDefault(claim => claim.Type == ClaimConstant.TenantId)?.Value;
            myUserResult.UserName = claims.FirstOrDefault(claim => claim.Type == ClaimConstant.UserName)?.Value;
            myUserResult.Role = claims.FirstOrDefault(claim => claim.Type == ClaimConstant.Role)?.Value;
            var userFullInfor = claims.FirstOrDefault(claim => claim.Type == ClaimConstant.UserFullInfor)?.Value;
            myUserResult.UserInfor = JsonConvert.DeserializeObject<UserInforVm>(userFullInfor);
            myUserResult.Permissions = claims.Where(c => c.Type == ClaimConstant.Permissions).Select(c => c.Value).ToList();

            //get user allocation
            var claimUserAllocation = claims.FirstOrDefault(claim => claim.Type == ClaimConstant.UserAllocationData)?.Value;

            myUserResult.UserAllocationInfors = JsonConvert.DeserializeObject<UserAllocationClaimVm>(claimUserAllocation);
            if (myUserResult.UserAllocationInfors != null)
            {
                myUserResult.ProjectCodeAllowCollection = myUserResult.UserAllocationInfors.AllocationSites.Select(x => x.Name.ToUpper()).ToList();
            }
            myUserResult.ProjectCode = myUserResult.Site;
            myUserResult.AccessToken = accessToken;
            return myUserResult;
        }

    }

}
