namespace BrawlStars.Server.Network.Tcp;

using BrawlStars.Server.Network.Base;
using BrawlStars.Server.Processors.DependencyInjection;
using Microsoft.Extensions.Logging;

internal class TcpSession : NetSession
{
    private const int MaxPacketSize = 16384;
    private const int ReadTimeout = 30;

    private readonly byte[] _recvBuffer;

    public TcpSession(ILogger<TcpSession> logger, ProcessorManager processorManager, NetSessionManager sessionManager) : base(logger, processorManager, sessionManager)
    {
        _recvBuffer = GC.AllocateUninitializedArray<byte>(MaxPacketSize);
    }

    public override async Task RunAsync()
    {
        if (NetworkUnit == null)
            throw new InvalidOperationException("The NetworkUnit is null, can't run session!");

        var memory = _recvBuffer.AsMemory();
        var memoryOffset = 0;

        while (true)
        {
            var nRead = await ReadWithTimeoutAsync(memory[memoryOffset..], ReadTimeout);
            if (nRead <= 0) break;

            var consumedBytes = await ConsumePacketsAsync(_recvBuffer, memoryOffset += nRead);
            if (consumedBytes == -1)
                break;

            if (consumedBytes > 0)
                Buffer.BlockCopy(_recvBuffer, consumedBytes, _recvBuffer, 0, memoryOffset -= consumedBytes);
        }
    }

    private async ValueTask<int> ReadWithTimeoutAsync(Memory<byte> memory, int timeoutSeconds)
    {
        var cancellationTokenSource = new CancellationTokenSource(timeoutSeconds * 1000);
        return await NetworkUnit!.ReceiveAsync(memory, cancellationTokenSource.Token);
    }
}
