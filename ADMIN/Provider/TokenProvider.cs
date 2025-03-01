using ADMIN.Models.OidcModel;
using ADMIN.Constant;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Options;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text.Json;

namespace ADMIN.Provider
{
    public interface ITokenProvider
    {
        Task<string> RefreshTokenAsync();
    }
    public class TokenProvider(IOptions<IdentityConstant> options, IHttpContextAccessor contextAccessor) : ITokenProvider
    {
        IdentityConstant _options = options.Value;
        private readonly IHttpContextAccessor _httpContextAccessor = contextAccessor;

        public async Task<string> RefreshTokenAsync()
        {
            if (_httpContextAccessor.HttpContext.User.Identity.IsAuthenticated)
            {
                var authenticateResult = await _httpContextAccessor.HttpContext.AuthenticateAsync();
                var token = GetOrRefreshAccessTokenAsync().Result;
                if (string.IsNullOrEmpty(token))
                     _httpContextAccessor.HttpContext.SignOutAsync("Cookies");

                return token;
            }
            return "";
        }

        private async Task<string> GetOrRefreshAccessTokenAsync()
        {
            var httpContext = _httpContextAccessor.HttpContext;
            var authenticateResult = await httpContext.AuthenticateAsync();

            if (authenticateResult?.Succeeded != true)
                throw new UnauthorizedAccessException("User is not authenticated.");


            // Lấy AccessToken hiện tại
            var refreshTokentest = authenticateResult.Properties.GetTokenValue("refresh_token");
            var accessToken = authenticateResult.Properties.GetTokenValue("access_token");
            var expiresAt = GetTokenExpiration(accessToken);

            if (expiresAt.HasValue && expiresAt.Value > DateTimeOffset.UtcNow)
                return accessToken; // Token còn hợp lệ, trả về


            // Token đã hết hạn, làm mới
            var refreshToken = authenticateResult.Properties.GetTokenValue("refresh_token");
            if (string.IsNullOrEmpty(refreshToken))
                throw new UnauthorizedAccessException("Refresh token is missing.");

            using var httpClient = new HttpClient();
            var tokenResponse = await RefreshAccessTokenMethodAsync(refreshToken);
            if (tokenResponse == null)
            {
                return null;
            }
            // Cập nhật AccessToken và RefreshToken mới vào session
            authenticateResult.Properties.UpdateTokenValue("access_token", tokenResponse.AccessToken);
            authenticateResult.Properties.UpdateTokenValue("refresh_token", tokenResponse.RefreshToken);
            authenticateResult.Properties.UpdateTokenValue("id_token", tokenResponse.IdToken);
            httpContext.Session.SetString("access_token", tokenResponse.AccessToken);
            httpContext.Session.SetString("refresh_token", tokenResponse.RefreshToken);
            httpContext.Session.SetString("id_token", tokenResponse.IdToken);
            // Cập nhật ClaimsPrincipal mới
            var newClaims = GetClaimsFromAccessTokenAsync(tokenResponse.AccessToken); // Đọc claims từ access token mới

            // Tạo ClaimsIdentity mới
            var claimsIdentity = new ClaimsIdentity(newClaims, "Bearer");
            var claimsPrincipal = new ClaimsPrincipal(claimsIdentity);

            // Cập nhật HttpContext.User
            //httpContext.User = claimsPrincipal;
            //return tokenResponse.AccessToken;


            httpContext.User = claimsPrincipal;
            //// Cập nhật lại thông tin xác thực trong HttpContext
            await httpContext.SignInAsync(claimsPrincipal, authenticateResult.Properties);
            return tokenResponse.AccessToken;
        }

        private DateTimeOffset? GetTokenExpiration(string? accessToken)
        {
            if (string.IsNullOrEmpty(accessToken))
            {
                return null; // Không có token, coi như hết hạn
            }

            // Giải mã JWT và lấy thời gian hết hạn
            var handler = new JwtSecurityTokenHandler();
            var jsonToken = handler.ReadToken(accessToken) as JwtSecurityToken;

            if (jsonToken == null)
            {
                return null; // Token không hợp lệ
            }

            // Lấy claim "exp" và kiểm tra
            var expClaim = jsonToken?.Claims?.FirstOrDefault(c => c.Type == "exp")?.Value;
            if (expClaim != null)
            {
                var expirationTime = DateTimeOffset.FromUnixTimeSeconds(long.Parse(expClaim));
                return expirationTime;
            }
            return null;
        }

        private async Task<TokenResponse> RefreshAccessTokenMethodAsync(string refreshToken)
        {
            var httpContext = _httpContextAccessor.HttpContext;
            //var idToken = await httpContext.GetTokenAsync("id_token");

            using var client = new HttpClient();
            var tokenEndpoint = $"{_options.IdentityUrl}/connect/token";
            var requestContent = new Dictionary<string, string>
            {
                { "grant_type", "refresh_token" },
                { "scope", "openid profile offline_access" },
                { "refresh_token", refreshToken },
                { "client_id", _options.ClientId},
                { "client_secret", _options.ClientSecret}
            };
            var request = new HttpRequestMessage(HttpMethod.Post, tokenEndpoint)
            {
                Content = new FormUrlEncodedContent(requestContent)
            };

            var response = await client.SendAsync(request);
            if (!response.IsSuccessStatusCode)
            {

                //throw new Exception("Refresh token failed.");
                return null;
            }

            var content = await response.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<TokenResponse>(content);
        }

        private IEnumerable<Claim> GetClaimsFromAccessTokenAsync(string accessToken)
        {
            var handler = new JwtSecurityTokenHandler();
            var jsonToken = handler.ReadToken(accessToken) as JwtSecurityToken;

            // Lấy tất cả các claims từ JWT token
            var claims = jsonToken?.Claims ?? Enumerable.Empty<Claim>();

            return claims;
        }
    }
}
