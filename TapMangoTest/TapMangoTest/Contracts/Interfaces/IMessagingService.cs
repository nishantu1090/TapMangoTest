namespace TapMangoTest.Contracts.Interfaces
{
    public interface IMessagingService
    {
        public Task<String> CanSendMessageAsync(String account, String phoneNumber);
    }
}
