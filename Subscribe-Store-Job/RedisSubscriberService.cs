using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using StackExchange.Redis;
using Subscribe_Store_Job.OrderServices.Abstractions;
using System.Text.Json;

namespace Subscribe_Store_Job
{
    public class RedisSubscriberService : BackgroundService
    {
        private readonly IConnectionMultiplexer _redis;
        private IOrderProcessor _orderProcessor;
        private readonly ILogger<RedisSubscriberService> _logger;
        private const string ChannelName = "OrderTopic";

        public RedisSubscriberService(
            IOrderProcessor orderProcessor,
            ILogger<RedisSubscriberService> logger,
            IConnectionMultiplexer connectionMultiplexer)
        {
            _orderProcessor = orderProcessor;
            _logger = logger;
            _redis = connectionMultiplexer;
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
                    Models.Order order = JsonSerializer.Deserialize<Models.Order>(message.ToString());

                    if (order != null)
                    {
                        await _orderProcessor.Process(order);
                    }
                    else
                    {
                        _logger.LogWarning($"Received an invalid message format: {message.ToString()}");
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