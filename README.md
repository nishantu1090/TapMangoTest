  TapMangoTest <br />
  I have used redis as the cache to store info. Please run redis in docker with the following command before running the .NET application.
  1. docker pull redis
  2. docker run --name redis-container -d -p 6379:6379 redis

  I have also added a TestProject in the solution, it contains tests to test the service.

  Approach used to solve the given problem:
  I have created a web api which can be called with the account name and phone number in the request to see if this phone number and account is allowed to send message. The web api will return a message which is "Yes! Can send message" if the upper limit has not reached yet
  else it will return "Limit Exceeded!" message. 
  To check if the limit of the account has exceeded I have used the key "account:acountNo:timestamp" in redis to keep track of how many requests have been made with this account in one second. The cache is set to expire in a second, so within a second if the same account 
  is used to make requests which exceeds the upper limit, then a "Limit Exceeded!" message is returned by the api. As the cache with that particular key is set to expire in a second, in the next second the same account will be allowed to send messages.
  Same approach has been used to keep track of the particular phone number requests.

  The limits are configurable in appsettings.json file!
