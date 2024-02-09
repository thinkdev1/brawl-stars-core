namespace BrawlStars.Server.Network.Tcp;

using System.Net.Sockets;
using System.Threading;
using BrawlStars.Server.Settings;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

internal class TcpGateway : IGateway
{
    private const int SocketListenBacklog = 100;

    private readonly IOptions<GatewayOptions> _options;
    private readonly ILogger _logger;
    private readonly Socket _socket;
    private readonly NetSessionManager _sessionManager;

    public TcpGateway(ILogger<TcpGateway> logger, IOptions<GatewayOptions> options, NetSessionManager sessionManager)
    {
        _logger = logger;
        _options = options;
        _sessionManager = sessionManager;

        _socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
    }

    public void Start(CancellationToken cancellationToken)
    {
        var bindPoint = _options.Value.BindPoint;

        _socket.Bind(bindPoint);
        _socket.Listen(SocketListenBacklog);
        _ = RunAsync(cancellationToken);

        _logger.LogInformation("TcpGateway is listening at {endPoint}", bindPoint);
    }

    private async Task RunAsync(CancellationToken cancellationToken)
    {
        while (!cancellationToken.IsCancellationRequested)
        {
            var socket = await _socket.AcceptAsync(cancellationToken);
            _ = _sessionManager.RunSessionAsync(new TcpNetworkUnit(socket));
        }
    }

    public void Stop()
    {
        _socket.Close();
    }
}
