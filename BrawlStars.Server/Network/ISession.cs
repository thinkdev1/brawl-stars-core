namespace BrawlStars.Server.Network;

using System.Net;

internal interface ISession : IDisposable
{
    EndPoint RemoteEndPoint { get; }
    ulong Id { get; }

    void Establish(ulong sessionId, INetworkUnit networkUnit);
    Task RunAsync();
}
