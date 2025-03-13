  TapMangoTest <br />
  I have used redis as the cache to store info. Please run redis in docker with the following command before running the .NET application.
  1. docker pull redis
  2. docker run --name redis-container -d -p 6379:6379 redis

  I have also added a TestProject in the solution, it contains tests to test the service.
