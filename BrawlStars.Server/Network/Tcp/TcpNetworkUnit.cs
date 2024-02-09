namespace BrawlStars.Server.Network.Tcp;
using System.Net;
using System.Net.Sockets;

internal class TcpNetworkUnit : INetworkUnit
{
    private readonly Socket _socket;
    public EndPoint RemoteEndPoint => _socket.RemoteEndPoint!;

    public TcpNetworkUnit(Socket socket)
    {
        _socket = socket;
    }

    public async ValueTask<int> ReceiveAsync(Memory<byte> destination, CancellationToken cancellationToken)
    {
        return await _socket.ReceiveAsync(destination, cancellationToken);
    }

    public async ValueTask<int> SendAsync(Memory<byte> data)
    {
        return await _socket.SendAsync(data);
    }

    public void Dispose()
    {
        _socket.Close();
    }
}
