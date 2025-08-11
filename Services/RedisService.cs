
using StackExchange.Redis;

namespace Api.Services

{
    public class RedisService
    {
        private readonly IDatabase _db;

        public RedisService(IConnectionMultiplexer redis)
        {
            _db = redis.GetDatabase();
        }

        public async Task SetAsync(string key, string value)
        {
            await _db.StringSetAsync(key, value, TimeSpan.FromMinutes(10));
        }

        public async Task<string?> GetAsync(string key)
        {
            return await _db.StringGetAsync(key);
        }
    }

}