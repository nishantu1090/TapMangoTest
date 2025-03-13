using TapMangoTest.Contracts.Interfaces;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Options;
using TapMangoTest.Contracts.Models;
namespace TapMangoTest.Services
{
    public class MessagingService : IMessagingService
    {
        private readonly IDistributedCache _cache;
        private readonly AppSettings _settings;
        private const string CanSendMessage = "Yes! Can Send Message";
        private const string LimitExceeded = "Allowed Limit has exceeded!";

        public MessagingService(IDistributedCache cache, IOptionsMonitor<AppSettings> options)
        {
            this._cache = cache;
            _settings = options.CurrentValue;
        }
        public async Task<string> CanSendMessageAsync(string account, string phoneNumber)
        {
            string timestamp = DateTime.UtcNow.ToString("yyyyMMddHHmmss");
            string accountKey = $"account:{account}:{timestamp}";
            string phoneKey = $"account:{account}:phone:{phoneNumber}:{timestamp}";
            var exceedsLimit = await ExceedsLimit(accountKey, phoneKey);
            if (!exceedsLimit)
            {
                return CanSendMessage;
            }
            else
            {
                return LimitExceeded;
            }               
        }

        public async Task SetDataAsync(string key, string value)
        {
            await _cache.SetStringAsync(key, value, new DistributedCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = TimeSpan.FromSeconds(1)
            });
        }

        public async Task<string?> GetDataAsync(string key)
        {
            return await _cache.GetStringAsync(key);            
        }

        private async Task<bool> ExceedsLimit(string accountKey, string phoneKey)
        {
            var countPerAccount = await GetDataAsync(accountKey);            
            if(countPerAccount == null)
            {
                await SetDataAsync(accountKey, "1");
            }
            int noOfMessagesPerAccount = countPerAccount == null ? 1: int.Parse(countPerAccount) + 1;
            if (noOfMessagesPerAccount > this._settings.AllowedAccountLimitPerSec)
                return true;

            //update cache            
            await SetDataAsync(accountKey, noOfMessagesPerAccount.ToString());

            var countPerPhone = await GetDataAsync(phoneKey);
            if (countPerPhone == null)
            {
                await SetDataAsync(phoneKey, "1");
                return false;
            }
            int noOfMessagesPerPhone = countPerPhone == null ? 1 : int.Parse(countPerPhone) + 1;
            if (noOfMessagesPerPhone > this._settings.AllowedPhoneLimitPerSec)
                return true;

            //update cache
            await SetDataAsync(phoneKey, noOfMessagesPerPhone.ToString());

            return false;
        }
    }
}
