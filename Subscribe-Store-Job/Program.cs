using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using StackExchange.Redis;

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
                            ("redis-17547.c83.us-east-1-2.ec2.redns.redis-cloud.com:17547,password=oQqZa02Qq0jkekRQTGZPoRQfRNNjbwU2")); // Replace with your Redis connection string

                        // Register the background service
                        services.AddHostedService<RedisSubscriberService>();
                    }).ConfigureLogging(logging =>
                    {
                        logging.ClearProviders(); // Remove default providers
                        logging.AddConsole();     // Add console logging
                        logging.SetMinimumLevel(LogLevel.Information); // Set the minimum log level
                    })
                            .Build();

            await host.RunAsync();
        }
    }
}
