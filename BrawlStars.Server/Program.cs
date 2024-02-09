namespace BrawlStars.Server;

using BrawlStars.Server.Network;
using BrawlStars.Server.Network.Tcp;
using BrawlStars.Server.Processors.DependencyInjection;
using BrawlStars.Server.Settings;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

internal class Program
{
    private static async Task Main(string[] args)
    {
        Console.Title = "BrawlStars.Server";

        var builder = Host.CreateApplicationBuilder(args);

        builder.Services.AddSingleton<IGateway, TcpGateway>();
        builder.Services.AddScoped<ISession, TcpSession>();
        builder.Services.AddSingleton<NetSessionManager>();
        builder.Services.AddSingleton<ProcessorManager>();
        builder.Services.AddLogging(logging => logging.AddSimpleConsole());

        builder.Services.Configure<GatewayOptions>(builder.Configuration.GetRequiredSection(GatewayOptions.ConfigSection));
        builder.Services.AddHostedService<GameServer>();

        await builder.Build().RunAsync();
    }
}