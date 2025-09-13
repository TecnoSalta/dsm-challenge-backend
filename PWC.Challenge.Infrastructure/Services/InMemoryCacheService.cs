using Microsoft.Extensions.Logging;
using PWC.Challenge.Application.Services;
using System.Text;
using System.Text.Json;

namespace PWC.Challenge.Infrastructure.Services
{
    public class InMemoryCacheService : ICacheService
    {
        private readonly ILogger<InMemoryCacheService> _logger;
        private readonly Dictionary<string, (byte[] data, DateTime expiration)> _cache = new();

        public InMemoryCacheService(ILogger<InMemoryCacheService> logger)
        {
            _logger = logger;
            _logger.LogInformation("Using In-Memory cache (Redis not available)");
        }

        public Task<T?> GetAsync<T>(string key, CancellationToken cancellationToken = default)
        {
            if (_cache.TryGetValue(key, out var value) && value.expiration > DateTime.UtcNow)
            {
                var json = Encoding.UTF8.GetString(value.data);
                return Task.FromResult(JsonSerializer.Deserialize<T>(json));
            }
            return Task.FromResult(default(T));
        }

        public Task SetAsync<T>(string key, T value, TimeSpan? expiration = null, CancellationToken cancellationToken = default)
        {
            var json = JsonSerializer.Serialize(value);
            var bytes = Encoding.UTF8.GetBytes(json);
            var exp = DateTime.UtcNow.Add(expiration ?? TimeSpan.FromSeconds(30));

            _cache[key] = (bytes, exp);
            return Task.CompletedTask;
        }

        public Task RemoveAsync(string key, CancellationToken cancellationToken = default)
        {
            _cache.Remove(key);
            return Task.CompletedTask;
        }

        public Task<bool> ExistsAsync(string key, CancellationToken cancellationToken = default)
        {
            return Task.FromResult(_cache.ContainsKey(key) && _cache[key].expiration > DateTime.UtcNow);
        }

        public Task RemoveByPatternAsync(string pattern, CancellationToken cancellationToken = default)
        {
            var keysToRemove = _cache.Keys.Where(k => k.Contains(pattern)).ToList();
            foreach (var key in keysToRemove)
            {
                _cache.Remove(key);
            }
            return Task.CompletedTask;
        }
        public async Task<T> GetOrCreateAsync<T>(string key, Func<Task<T>> factory, TimeSpan? expiration = null, CancellationToken cancellationToken = default)
        {
            var cachedValue = await GetAsync<T>(key, cancellationToken);
            if (cachedValue != null)
            {
                return cachedValue;
            }

            var value = await factory();
            await SetAsync(key, value, expiration, cancellationToken);

            return value;
        }
    }
}