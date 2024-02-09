namespace BrawlStars.Logic.Message.Account;

using BrawlStars.Logic.Enums;

[Piranha(MessageType.KeepAliveServer)]
public class KeepAliveServerMessage : PiranhaMessage
{
    public override MessageType MessageType => MessageType.KeepAliveServer;
    public override ServiceNodeType ServiceNodeType => ServiceNodeType.Account;

    public KeepAliveServerMessage()
    {
    }
}
