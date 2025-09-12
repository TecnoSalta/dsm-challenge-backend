using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
using StackExchange.Redis;
using PWC.Challenge.Infrastructure.Configurations;
using PWC.Challenge.Infrastructure.Services;
using System.Text;
using System.Text.Json;

namespace PWC.Challenge.UnitTests.Infrastructure.Services
{
    public class RedisCacheServiceTests
    {
        private readonly Mock<IDistributedCache> _distributedCacheMock;
        private readonly Mock<IConnectionMultiplexer> _redisConnectionMock;
        private readonly Mock<ILogger<RedisCacheService>> _loggerMock;
        private readonly RedisSettings _redisSettings;
        private readonly RedisCacheService _cacheService;

        public RedisCacheServiceTests()
        {
            _distributedCacheMock = new Mock<IDistributedCache>();
            _redisConnectionMock = new Mock<IConnectionMultiplexer>();
            _loggerMock = new Mock<ILogger<RedisCacheService>>();
            _redisSettings = new RedisSettings
            {
                InstanceName = "test:",
                AbsoluteExpirationInSeconds = 30,
                SlidingExpirationInSeconds = 15
            };

            var optionsMock = new Mock<IOptions<RedisSettings>>();
            optionsMock.Setup(o => o.Value).Returns(_redisSettings);

            _cacheService = new RedisCacheService(
                _distributedCacheMock.Object,
                optionsMock.Object,
                _redisConnectionMock.Object,
                _loggerMock.Object);
        }

        [Fact]
        public async Task GetAsync_ShouldReturnDeserializedObject_WhenCacheHit()
        {
            // Arrange
            var testObject = new { Id = 1, Name = "Test" };
            var serialized = JsonSerializer.Serialize(testObject);
            var bytes = Encoding.UTF8.GetBytes(serialized);

            _distributedCacheMock
                .Setup(c => c.GetAsync("test_key", It.IsAny<CancellationToken>()))
                .ReturnsAsync(bytes);

            // Act
            var result = await _cacheService.GetAsync<object>("test_key");

            // Assert
            Assert.NotNull(result);
            _distributedCacheMock.Verify(c => c.GetAsync("test_key", It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task GetAsync_ShouldReturnDefault_WhenCacheMiss()
        {
            // Arrange
            _distributedCacheMock
                .Setup(c => c.GetAsync("test_key", It.IsAny<CancellationToken>()))
                .ReturnsAsync((byte[])null);

            // Act
            var result = await _cacheService.GetAsync<object>("test_key");

            // Assert
            Assert.Null(result);
        }

        [Fact]
        public async Task SetAsync_ShouldSerializeAndStoreObject()
        {
            // Arrange
            var testObject = new { Id = 1, Name = "Test" };
            byte[] capturedBytes = null;

            _distributedCacheMock
                .Setup(c => c.SetAsync("test_key", It.IsAny<byte[]>(), It.IsAny<DistributedCacheEntryOptions>(), It.IsAny<CancellationToken>()))
                .Callback<string, byte[], DistributedCacheEntryOptions, CancellationToken>((key, bytes, options, ct) => capturedBytes = bytes)
                .Returns(Task.CompletedTask);

            // Act
            await _cacheService.SetAsync("test_key", testObject);

            // Assert
            Assert.NotNull(capturedBytes);
            var deserialized = JsonSerializer.Deserialize<object>(Encoding.UTF8.GetString(capturedBytes));
            Assert.NotNull(deserialized);
        }

        [Fact]
        public async Task RemoveAsync_ShouldCallDistributedCache()
        {
            // Arrange
            _distributedCacheMock
                .Setup(c => c.RemoveAsync("test_key", It.IsAny<CancellationToken>()))
                .Returns(Task.CompletedTask);

            // Act
            await _cacheService.RemoveAsync("test_key");

            // Assert
            _distributedCacheMock.Verify(c => c.RemoveAsync("test_key", It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task ExistsAsync_ShouldReturnTrue_WhenKeyExists()
        {
            // Arrange
            _distributedCacheMock
                .Setup(c => c.GetAsync("test_key", It.IsAny<CancellationToken>()))
                .ReturnsAsync(new byte[1]);

            // Act
            var exists = await _cacheService.ExistsAsync("test_key");

            // Assert
            Assert.True(exists);
        }

        [Fact]
        public async Task GetOrCreateAsync_ShouldReturnCachedValue_WhenExists()
        {
            // Arrange
            var testObject = new { Id = 1, Name = "Cached" };
            var serialized = JsonSerializer.Serialize(testObject);
            var bytes = Encoding.UTF8.GetBytes(serialized);

            _distributedCacheMock
                .Setup(c => c.GetAsync("test_key", It.IsAny<CancellationToken>()))
                .ReturnsAsync(bytes);

            // Act
            var result = await _cacheService.GetOrCreateAsync("test_key",
                () => Task.FromResult(new { Id = 2, Name = "New" }));

            // Assert
            Assert.NotNull(result);
            Assert.Equal("Cached", result.Name);
        }

        [Fact]
        public async Task GetOrCreateAsync_ShouldCreateAndCache_WhenNotExists()
        {
            // Arrange
            _distributedCacheMock
                .Setup(c => c.GetAsync("test_key", It.IsAny<CancellationToken>()))
                .ReturnsAsync((byte[])null);

            _distributedCacheMock
                .Setup(c => c.SetAsync("test_key", It.IsAny<byte[]>(), It.IsAny<DistributedCacheEntryOptions>(), It.IsAny<CancellationToken>()))
                .Returns(Task.CompletedTask);

            // Act
            var result = await _cacheService.GetOrCreateAsync("test_key",
                () => Task.FromResult(new { Id = 2, Name = "New" }));

            // Assert
            Assert.NotNull(result);
            Assert.Equal("New", result.Name);
            _distributedCacheMock.Verify(c => c.SetAsync(
                "test_key",
                It.IsAny<byte[]>(),
                It.IsAny<DistributedCacheEntryOptions>(),
                It.IsAny<CancellationToken>()),
                Times.Once);
        }
    }
}