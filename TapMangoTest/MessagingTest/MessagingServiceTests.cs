using Moq;
using NUnit.Framework;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Options;
using System.Text;
using System.Threading.Tasks;
using TapMangoTest.Services;
using TapMangoTest.Contracts.Models;

namespace TapMangoTest.Tests
{
    [TestFixture]
    public class MessagingServiceTests
    {
        private Mock<IDistributedCache> _mockCache;
        private Mock<IOptionsMonitor<AppSettings>> _mockOptions;
        private MessagingService _messagingService;
        private AppSettings _testSettings;
        private const string CanSendMessage = "Yes! Can Send Message";
        private const string LimitExceeded = "Allowed Limit has exceeded!";
        [SetUp]
        public void Setup()
        {
            _mockCache = new Mock<IDistributedCache>();
            _mockOptions = new Mock<IOptionsMonitor<AppSettings>>();

            // Setup test settings
            _testSettings = new AppSettings
            {
                AllowedAccountLimitPerSec = 5,
                AllowedPhoneLimitPerSec = 3
            };
            _mockOptions.Setup(o => o.CurrentValue).Returns(_testSettings);

            // Initialize MessagingService with mocks
            _messagingService = new MessagingService(_mockCache.Object, _mockOptions.Object);
        }

        [Test]
        public async Task CanSendMessageAsync_ShouldReturnYes_WhenLimitNotExceeded()
        {
            // Arrange
            string account = "user123";
            string phoneNumber = "1234567890";
            string timestamp = DateTime.UtcNow.ToString("yyyyMMddHHmmss");
            string accountKey = $"account:{account}:{timestamp}";
            string phoneKey = $"account:{account}:phone:{phoneNumber}:{timestamp}";

            _mockCache.Setup(c => c.GetAsync(accountKey, It.IsAny<CancellationToken>()))
           .ReturnsAsync((byte[])null); // Simulate empty cache

            _mockCache.Setup(c => c.GetAsync(phoneKey, It.IsAny<CancellationToken>()))
                      .ReturnsAsync((byte[])null); //
            // Act
            var result = await _messagingService.CanSendMessageAsync(account, phoneNumber);

            // Assert
            Assert.That(CanSendMessage, Is.EqualTo(result));
        }

        [Test]
        public async Task CanSendMessageAsync_ShouldReturnExceedsLimit_WhenAccountLimitExceeded()
        {
            // Arrange
            string account = "user123";
            string phoneNumber = "1234567890";
            string timestamp = DateTime.UtcNow.ToString("yyyyMMddHHmmss");
            string accountKey = $"account:{account}:{timestamp}";

            // Mock cache response (account already reached the limit)
            byte[] cachedData = Encoding.UTF8.GetBytes(_testSettings.AllowedAccountLimitPerSec.ToString());

            _mockCache.Setup(c => c.GetAsync(accountKey, It.IsAny<CancellationToken>()))
                      .ReturnsAsync(cachedData);

            // Act
            var result = await _messagingService.CanSendMessageAsync(account, phoneNumber);

            // Assert
            Assert.That(LimitExceeded, Is.EqualTo(result));
        }

        [Test]
        public async Task CanSendMessageAsync_ShouldReturnExceedsLimit_WhenPhoneLimitExceeded()
        {
            // Arrange
            string account = "user123";
            string phoneNumber = "1234567890";
            string timestamp = DateTime.UtcNow.ToString("yyyyMMddHHmmss");
            string phoneKey = $"account:{account}:phone:{phoneNumber}:{timestamp}";
            string accountKey = $"account:{account}:{timestamp}";
            // Mock cache response (phone number already reached the limit)
            byte[] cachedData = Encoding.UTF8.GetBytes(_testSettings.AllowedPhoneLimitPerSec.ToString());
            byte[] cachedAccountData = Encoding.UTF8.GetBytes("1");

            _mockCache.Setup(c => c.GetAsync(accountKey, It.IsAny<CancellationToken>()))
                      .ReturnsAsync(cachedAccountData);

            _mockCache.Setup(c => c.GetAsync(phoneKey, It.IsAny<CancellationToken>()))
                      .ReturnsAsync(cachedData);

            // Act
            var result = await _messagingService.CanSendMessageAsync(account, phoneNumber);

            // Assert
            Assert.That(LimitExceeded, Is.EqualTo(result));
        }

        
       
    }
}
