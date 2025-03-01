using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Caching.Memory;

namespace ADMIN.Provider
{
    public class MemoryCacheTicketStore : ITicketStore
    {
        private readonly IMemoryCache _memoryCache;
        private const string KeyPrefix = "AuthSession_";

        public MemoryCacheTicketStore(IMemoryCache memoryCache)
        {
            _memoryCache = memoryCache;
        }

        public Task<string> StoreAsync(AuthenticationTicket ticket)
        {
            var key = KeyPrefix + Guid.NewGuid();
            _memoryCache.Set(key, ticket, DateTimeOffset.UtcNow.AddHours(1)); // Thời gian lưu session
            return Task.FromResult(key);
        }

        public Task<AuthenticationTicket?> RetrieveAsync(string key)
        {
            _memoryCache.TryGetValue(key, out AuthenticationTicket? ticket);
            return Task.FromResult(ticket);
        }

        public Task RenewAsync(string key, AuthenticationTicket ticket)
        {
            _memoryCache.Set(key, ticket, DateTimeOffset.UtcNow.AddHours(1)); // Cập nhật session
            return Task.CompletedTask;
        }

        public Task RemoveAsync(string key)
        {
            _memoryCache.Remove(key);
            return Task.CompletedTask;
        }

        // Thêm phương thức làm mới token
        public async Task<bool> RefreshTokenAsync(string key, Func<string, Task<(string AccessToken, string RefreshToken)>> refreshTokenHandler)
        {
            var ticket = await RetrieveAsync(key);
            if (ticket == null) return false;

            // Lấy RefreshToken hiện tại
            var currentRefreshToken = ticket.Properties.GetTokenValue("refresh_token");
            if (string.IsNullOrEmpty(currentRefreshToken)) return false;

            // Gọi handler để làm mới token
            var (newAccessToken, newRefreshToken) = await refreshTokenHandler(currentRefreshToken);

            // Cập nhật token trong ticket
            ticket.Properties.UpdateTokenValue("access_token", newAccessToken);
            ticket.Properties.UpdateTokenValue("refresh_token", newRefreshToken);

            // Lưu lại session
            await RenewAsync(key, ticket);
            return true;
        }
    }
}
