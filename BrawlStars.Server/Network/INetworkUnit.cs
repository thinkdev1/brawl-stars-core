namespace BrawlStars.Server.Network;
using System.Net;

internal interface INetworkUnit : IDisposable
{
    EndPoint RemoteEndPoint { get; }

    ValueTask<int> ReceiveAsync(Memory<byte> destination, CancellationToken cancellationToken);
    ValueTask<int> SendAsync(Memory<byte> data);
}
