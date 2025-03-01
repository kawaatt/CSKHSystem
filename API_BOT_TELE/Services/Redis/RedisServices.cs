using StackExchange.Redis;

namespace TELEBOT_CSKH.Services.Redis
{
    public interface IRedisServices
    {
        Task RemoveValueAsync(string key);
        Task<string?> GetValueAsync(string key);
        Task SetValueAsync(string key, string value, TimeSpan? expiry = null);
    }

    public class RedisServices : IRedisServices
    {
        private readonly ConnectionMultiplexer _redis;
        private readonly IDatabase _database;

        public RedisServices(IConfiguration configuration)
        {
            _redis = ConnectionMultiplexer.Connect(configuration["RedisConnection"]);
            _database = _redis.GetDatabase();
        }

        public async Task SetValueAsync(string key, string value, TimeSpan? expiry = null)
        {
            await _database.StringSetAsync(key, value, expiry);
        }

        public async Task<string?> GetValueAsync(string key)
        {
            return await _database.StringGetAsync(key);
        }

        public async Task RemoveValueAsync(string key)
        {
            await _database.KeyDeleteAsync(key);
        }
    }
}
