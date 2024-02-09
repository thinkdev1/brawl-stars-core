namespace BrawlStars.Server;

using BrawlStars.Server.Network;
using Microsoft.Extensions.Hosting;

internal class GameServer : IHostedService
{
    private readonly IGateway _gateway;

    public GameServer(IGateway gateway)
    {
        _gateway = gateway;
    }

    public Task StartAsync(CancellationToken cancellationToken)
    {
        _gateway.Start(cancellationToken);

        return Task.CompletedTask;
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        _gateway.Stop();

        return Task.CompletedTask;
    }
}
