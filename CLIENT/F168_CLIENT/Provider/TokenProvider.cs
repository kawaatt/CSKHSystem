using Microsoft.IdentityModel.Tokens;
using SHBET_CLIENT.Constant;
using SHBET_CLIENT.Models.DTO;
using System.IdentityModel.Tokens.Jwt;
using System.Text;

namespace SHBET_CLIENT.Provider
{
    public interface ITokenProvider
    {
        Task<string?> GetToken();
        void SetToken(string token);
        void ClearToken();


        UserDTO ReadTokenClearInformation();
    }

    public class TokenProvider : ITokenProvider
    {
        private readonly IHttpContextAccessor _contextAccessor;
        public TokenProvider(IHttpContextAccessor contextAccessor)
        {
            _contextAccessor = contextAccessor;
        }

        public async Task<string?> GetToken()
        {
            string? Token = null;
            try
            {
                _contextAccessor.HttpContext?.Request.Cookies.TryGetValue(SD.TokenCookie, out Token);
            }
            catch (Exception ex)
            {
                Console.WriteLine("TOKEN NULL");
            }
            return Token;
        }

        public void SetToken(string token)
        {
            _contextAccessor.HttpContext.Response.Cookies.Append(SD.TokenCookie, token, new CookieOptions
            {
                Expires = DateTimeOffset.UtcNow.AddDays(7), // Expires in 7 days
                HttpOnly = true,  // Prevents client-side JavaScript from accessing it
                Secure = true,    // Ensures it's sent over HTTPS
                SameSite = SameSiteMode.Strict // Prevents CSRF attacks
            });
        }

        public void ClearToken()
        {
            _contextAccessor.HttpContext.Response.Cookies.Delete(SD.TokenCookie);
        }

        public UserDTO ReadTokenClearInformation()
        {
            UserDTO myUserResult = new UserDTO();
            string? Token = null;
            bool? hasProjectCode = _contextAccessor.HttpContext?.Request.Cookies.TryGetValue(SD.TokenCookie, out Token);
            if (hasProjectCode == true)
            {
                var handler = new JwtSecurityTokenHandler();
                var jsonToken = handler.ReadToken(Token);
                var tokenS = jsonToken as JwtSecurityToken;
                myUserResult.ProjectCode = tokenS.Claims.First(claim => claim.Type == JwtRegisteredClaimNames.Sub).Value;
                myUserResult.UserName = tokenS.Claims.First(claim => claim.Type == JwtRegisteredClaimNames.Name).Value;
                myUserResult.Role = tokenS.Claims.First(claim => claim.Type == "role").Value;
            }
            return myUserResult;
        }


    }
}
