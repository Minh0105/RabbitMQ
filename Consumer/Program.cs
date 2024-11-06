using Common.Interfaces;
using Consumer.Services;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

IHost host = Host.CreateDefaultBuilder(args)
    .ConfigureServices(services =>
    {
        services.AddSingleton<IRabbitMQService, ConsumerRabbitMQService>();
        services.AddHostedService<OrderProcessingService>();
    })
    .Build();

await host.RunAsync();