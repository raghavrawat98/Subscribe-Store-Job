using Microsoft.Extensions.Hosting;
using StackExchange.Redis;
using System.Text.Json;

namespace Subscribe_Store_Job
{
    public class RedisSubscriberService : BackgroundService
    {
        private readonly IConnectionMultiplexer _redis;
        private const string ChannelName = "MyRedisChannel";

        public RedisSubscriberService(IConnectionMultiplexer redis)
        {
            _redis = redis;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            var subscriber = _redis.GetSubscriber();

            await subscriber.SubscribeAsync(ChannelName, async (channel, message) =>
            {
                // Parse the JSON message
                var jsonString = message.ToString();
                var payload = JsonSerializer.Deserialize<RedisMessage>(jsonString);

                if (payload != null)
                {
                    var db = _redis.GetDatabase();
                    string key = payload.Id.ToString();
                    string value = payload.Value;

                    // Store the message in Redis with key=id and value=message
                    await db.StringSetAsync(key, value);
                }
            });
        }
    }

    public class RedisMessage
    {
        public int Id { get; set; }
        public string Value { get; set; }
    }
}