namespace BrawlStars.Server.Network;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System.Collections.Concurrent;
using System.Diagnostics.CodeAnalysis;
using System.Net.Sockets;

internal class NetSessionManager
{
    private readonly ILogger _logger;
    private readonly IServiceScopeFactory _serviceScopeFactory;
    private readonly ConcurrentDictionary<ulong, ISession> _sessions;

    private ulong _sessionIdSeed;

    public NetSessionManager(ILogger<NetSessionManager> logger, IServiceScopeFactory serviceScopeFactory)
    {
        _logger = logger;
        _serviceScopeFactory = serviceScopeFactory;

        _sessions = new ConcurrentDictionary<ulong, ISession>();
    }

    public async Task RunSessionAsync(INetworkUnit networkUnit)
    {
        await using var scope = _serviceScopeFactory.CreateAsyncScope();
        var session = scope.ServiceProvider.GetRequiredService<ISession>();

        try
        {
            session.Establish(NextId(), networkUnit);
            await session.RunAsync();
        }
        catch (OperationCanceledException)
        {
            _logger.LogDebug("Operation canceled, disconnecting client");
        }
        catch (SocketException)
        {
            _logger.LogDebug("Socket exception occurred, disconnecting client");
        }
        catch (Exception exception)
        {
            _logger.LogError("Exception occurred during a session, trace: {exception}", exception);
        }
    }

    /// <summary>
    /// Stores session in this instance.
    /// </summary>
    public bool TryAdd(ISession session)
    {
        if (_sessions.TryAdd(session.Id, session))
        {
            _logger.LogInformation("New connection from {endPoint}", session.RemoteEndPoint);
            return true;
        }

        return false;
    }

    /// <summary>
    /// Gets <see cref="ISession"/> by session id.
    /// </summary>
    public bool TryGet(ulong id, [MaybeNullWhen(false)] out ISession session)
    {
        return _sessions.TryGetValue(id, out session);
    }

    /// <summary>
    /// Removes <see cref="ISession"/> with specified session id.
    /// </summary>
    public bool TryRemove(ISession session)
    {
        if (_sessions.TryRemove(session.Id, out _))
        {
            _logger.LogInformation("Client from {endPoint} disconnected", session.RemoteEndPoint);
            return true;
        }

        return false;
    }

    private ulong NextId()
    {
        return Interlocked.Increment(ref _sessionIdSeed);
    }
}
