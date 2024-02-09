namespace BrawlStars.Server.Network.Base;

using System.Buffers.Binary;
using System.Net;
using BrawlStars.Logic.Enums;
using BrawlStars.Logic.Message;
using BrawlStars.Logic.Serialization;
using BrawlStars.Server.Processors.DependencyInjection;
using Microsoft.Extensions.Logging;

internal abstract class NetSession : ISession
{
    private const int HeaderSize = 7;
    private const int SendBufferSize = 16384;

    public EndPoint RemoteEndPoint => NetworkUnit!.RemoteEndPoint;
    public ulong Id { get; private set; }

    protected INetworkUnit? NetworkUnit { get; private set; }
    private readonly ILogger _logger;
    private readonly ProcessorManager _processorManager;
    private readonly NetSessionManager _sessionManager;

    private readonly byte[] _sendBuffer;

    public NetSession(ILogger<NetSession> logger, ProcessorManager processorManager, NetSessionManager sessionManager)
    {
        _logger = logger;
        _processorManager = processorManager;
        _sessionManager = sessionManager;

        _sendBuffer = GC.AllocateUninitializedArray<byte>(SendBufferSize);
    }

    public void Establish(ulong sessionId, INetworkUnit networkUnit)
    {
        Id = sessionId;
        NetworkUnit = networkUnit;

        if (!_sessionManager.TryAdd(this))
            throw new ApplicationException("Couldn't start session.");
    }

    protected async ValueTask<int> ConsumePacketsAsync(byte[] data, int size)
    {
        if (size < HeaderSize)
            return 0;

        var memory = data.AsMemory();
        var consumedBytes = 0;
        do
        {
            var buffer = memory[consumedBytes..];

            var messageType = BinaryPrimitives.ReadUInt16BigEndian(buffer[0..2].Span);
            var messageLength = BinaryPrimitives.ReadUInt16BigEndian(buffer[3..5].Span);
            var messageVersion = BinaryPrimitives.ReadUInt16BigEndian(buffer[5..7].Span);

            if (size - consumedBytes - HeaderSize < messageLength)
                break;

            _logger.LogInformation("Received message! Type: {type}, length: {length}, version: {version}", messageType, messageLength, messageVersion);

            var message = DecodeMessage(messageType, messageVersion, buffer.Span.Slice(HeaderSize, messageLength));
            if (message != null)
            {
                var result = await _processorManager.HandleMessage(message);

                if (result != null)
                {
                    while (result.NextMessage(out var sendMessage))
                        await SendAsync(sendMessage);
                }
            }
            else
            {
                _logger.LogWarning("Ignoring message of unknown type {messageType}", (MessageType)messageType);
            }

            consumedBytes += HeaderSize + messageLength;
        } while (size - consumedBytes >= HeaderSize);

        return consumedBytes;
    }

    private async Task SendAsync(PiranhaMessage message)
    {
        EncodeMessage(message, _sendBuffer.AsSpan()[HeaderSize..]);

        var memory = _sendBuffer.AsMemory();
        BinaryPrimitives.WriteUInt16BigEndian(memory[0..2].Span, (ushort)message.MessageType);
        BinaryPrimitives.WriteUInt16BigEndian(memory[3..5].Span, (ushort)message.EncodingLength);
        BinaryPrimitives.WriteUInt16BigEndian(memory[5..7].Span, message.MessageVersion);

        await NetworkUnit!.SendAsync(memory[0..(message.EncodingLength + HeaderSize)]);
        _logger.LogInformation("Message {type} sent!", message.MessageType);
    }

    private static void EncodeMessage(PiranhaMessage message, Span<byte> destination)
    {
        var b = new ByteStream(destination);
        message.Encode(ref b);
    }

    private static PiranhaMessage? DecodeMessage(ushort messageType, ushort messageVersion, Span<byte> payload)
    {
        var message = MessageFactory.CreateMessageByType((MessageType)messageType);
        if (message != null)
        {
            message.MessageVersion = messageVersion;

            var b = new ByteStream(payload);
            message.Decode(ref b);
        }

        return message;
    }

    public abstract Task RunAsync();

    public virtual void Dispose()
    {
        _sessionManager.TryRemove(this);
        NetworkUnit?.Dispose();
    }
}
