using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using StackExchange.Redis;
using Subscribe_Store_Job.OrderServices.Abstractions;
using Subscribe_Store_Job.Processors.Implementations;
using Subscribe_Store_Job.Repositories.Abstractions;
using Subscribe_Store_Job.Repositories.Implementations;

namespace Subscribe_Store_Job
{
    internal class Program
    {
        static async Task Main(string[] args)
        {
            var host = Host.CreateDefaultBuilder(args)
                            .ConfigureServices((context, services) =>
                    {
                        // Register Redis connection multiplexer
                        services.AddSingleton<IConnectionMultiplexer>(sp =>
                            ConnectionMultiplexer.Connect
                            ("redis-17345.c100.us-east-1-4.ec2.redns.redis-cloud.com:17345,password=pBhDmDU07PubH9fXH5o58jDsevWU6YfO")); // Replace with your Redis connection string

                        // Register the background service
                        services.AddHostedService<RedisSubscriberService>();
                        services.AddScoped<IOrderProcessor, OrderProcessor>();
                        services.AddScoped<IOrderRepository, OrderRepostiory>();
                        services.AddScoped<IInventoryRepository, InventoryRepository>();
                        services.AddScoped<IMatchRepository, MatchRepository>();
                        services.AddScoped<IRedisRepository, RedisRepository>();
                    }).ConfigureLogging(logging =>
                    {
                        logging.ClearProviders(); // Remove default providers
                        logging.AddConsole();     // Add console logging
                        logging.SetMinimumLevel(LogLevel.Debug); // Set the minimum log level
                    })
                            .Build();

            await host.RunAsync();
        }
    }
}
