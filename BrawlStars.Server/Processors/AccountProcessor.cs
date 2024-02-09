namespace BrawlStars.Server.Processors;

using BrawlStars.Logic.Enums;
using BrawlStars.Logic.Message.Account;
using BrawlStars.Logic.Serialization.BasicTypes;
using BrawlStars.Server.Network;
using BrawlStars.Server.Processors.Attributes;
using BrawlStars.Server.Processors.Result;
using Microsoft.Extensions.Logging;

[ServiceNode(ServiceNodeType.Account)]
internal class AccountProcessor : Processor
{
    private readonly ILogger _logger;

    public AccountProcessor(ILogger<AccountProcessor> logger, ISession session) : base(logger, session)
    {
        _logger = logger;
    }

    [Message(MessageType.ClientHello)]
    public IHandlingResult OnClientHello([FromPayload] ClientHelloMessage message)
    {
        _logger.LogInformation("ClientHello: protocol: {protocol}, key: {keyVersion}, game version: {major}.{build}, content hash: {hash}",
                           message.Protocol.Value, message.KeyVersion.Value, message.MajorVersion.Value, message.BuildVersion.Value, message.ContentHash.Value);
        
        return Ok(new ServerHelloMessage
        {
            ServerNonce = new ByteArrayProperty
            {
                Value = new byte[24]
            }
        });
    }

    [Message(MessageType.KeepAlive)]
    public IHandlingResult OnKeepAlive([FromPayload] KeepAliveMessage message)
    {
        return Ok(new KeepAliveServerMessage());
    }
}
