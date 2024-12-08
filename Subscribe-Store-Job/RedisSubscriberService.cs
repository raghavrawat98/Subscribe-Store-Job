using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using StackExchange.Redis;
using System.Text.Json;

namespace Subscribe_Store_Job
{
    public class RedisSubscriberService : BackgroundService
    {
        private readonly IConnectionMultiplexer _redis;
        private readonly ILogger<RedisSubscriberService> _logger;
        private const string ChannelName = "MyRedisChannel";

        public RedisSubscriberService(IConnectionMultiplexer redis,
            ILogger<RedisSubscriberService> logger)
        {
            _redis = redis;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            var subscriber = _redis.GetSubscriber();

            await subscriber.SubscribeAsync(ChannelName, async (channel, message) =>
            {
                _logger.LogInformation("Message received on channel {Channel}: {Message}", channel, message);

                try
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
                        _logger.LogInformation($"Message stored in Redis. Key: {key}, Value: {value}");
                    }
                    else
                    {
                        _logger.LogWarning($"Received an invalid message format: {jsonString}");
                    }
                }
                catch (JsonException ex)
                {
                    _logger.LogError(ex, $"Error parsing message: {message}");
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, $"Error processing message: {message}");
                }
            });

            _logger.LogInformation($"Subscribed to Redis channel: {ChannelName}");
        }
    }

    public class RedisMessage
    {
        public int Id { get; set; }
        public string Value { get; set; }
    }
}