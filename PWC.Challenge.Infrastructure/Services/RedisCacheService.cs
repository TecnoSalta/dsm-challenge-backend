using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Logging;
using StackExchange.Redis;
using System.Text.Json;
using System.Text;
using PWC.Challenge.Infrastructure.Configurations;

namespace PWC.Challenge.Infrastructure.Services
{
    public class RedisCacheService : ICacheService
    {
        private readonly IDistributedCache _cache;
        private readonly RedisSettings _redisSettings;
        private readonly IConnectionMultiplexer _redisConnection;
        private readonly ILogger<RedisCacheService> _logger;

        public RedisCacheService(
            IDistributedCache cache,
            IOptions<RedisSettings> redisSettings,
            IConnectionMultiplexer redisConnection,
            ILogger<RedisCacheService> logger)
        {
            _cache = cache;
            _redisSettings = redisSettings.Value;
            _redisConnection = redisConnection;
            _logger = logger;
        }

        public async Task<T?> GetAsync<T>(string key, CancellationToken cancellationToken = default)
        {
            try
            {
                _logger.LogDebug("Getting cache for key: {Key}", key);
                var bytes = await _cache.GetAsync(key, cancellationToken);
                if (bytes == null)
                {
                    _logger.LogDebug("Cache miss for key: {Key}", key);
                    return default;
                }

                var json = Encoding.UTF8.GetString(bytes);
                _logger.LogDebug("Cache hit for key: {Key}", key);
                return JsonSerializer.Deserialize<T>(json);
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Error getting cache for key: {Key}", key);
                return default;
            }
        }

        public async Task SetAsync<T>(string key, T value, TimeSpan? expiration = null, CancellationToken cancellationToken = default)
        {
            try
            {
                _logger.LogDebug("Setting cache for key: {Key}", key);
                var options = new DistributedCacheEntryOptions
                {
                    AbsoluteExpirationRelativeToNow = expiration ?? TimeSpan.FromSeconds(_redisSettings.AbsoluteExpirationInSeconds),
                    SlidingExpiration = TimeSpan.FromSeconds(_redisSettings.SlidingExpirationInSeconds)
                };

                var json = JsonSerializer.Serialize(value);
                var bytes = Encoding.UTF8.GetBytes(json);

                await _cache.SetAsync(key, bytes, options, cancellationToken);
                _logger.LogDebug("Cache set successfully for key: {Key}", key);
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Error setting cache for key: {Key}", key);
            }
        }

        public async Task RemoveAsync(string key, CancellationToken cancellationToken = default)
        {
            try
            {
                _logger.LogDebug("Removing cache for key: {Key}", key);
                await _cache.RemoveAsync(key, cancellationToken);
                _logger.LogDebug("Cache removed for key: {Key}", key);
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Error removing cache for key: {Key}", key);
            }
        }

        public async Task<bool> ExistsAsync(string key, CancellationToken cancellationToken = default)
        {
            try
            {
                var value = await _cache.GetAsync(key, cancellationToken);
                return value != null;
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Error checking cache existence for key: {Key}", key);
                return false;
            }
        }

        public async Task RemoveByPatternAsync(string pattern, CancellationToken cancellationToken = default)
        {
            try
            {
                _logger.LogDebug("Removing cache by pattern: {Pattern}", pattern);
                var server = _redisConnection.GetServer(_redisConnection.GetEndPoints().First());
                var keys = server.Keys(pattern: $"*{pattern}*").ToArray();

                if (keys.Length > 0)
                {
                    var database = _redisConnection.GetDatabase();
                    await database.KeyDeleteAsync(keys);
                    _logger.LogDebug("Removed {Count} keys matching pattern: {Pattern}", keys.Length, pattern);
                }
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Error removing cache by pattern: {Pattern}", pattern);
            }
        }

        public async Task<T> GetOrCreateAsync<T>(string key, Func<Task<T>> factory, TimeSpan? expiration = null, CancellationToken cancellationToken = default)
        {
            try
            {
                // Intentar obtener del cache primero
                var cachedValue = await GetAsync<T>(key, cancellationToken);
                if (cachedValue != null)
                {
                    _logger.LogDebug("Cache hit for GetOrCreate: {Key}", key);
                    return cachedValue;
                }

                _logger.LogDebug("Cache miss for GetOrCreate: {Key}", key);

                // Si no está en cache, ejecutar la factory function
                var value = await factory();

                // Guardar en cache
                await SetAsync(key, value, expiration, cancellationToken);

                return value;
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Error in GetOrCreateAsync for key: {Key}", key);

                // Si hay error, ejecutar la factory function como fallback
                return await factory();
            }
        }
    }
}