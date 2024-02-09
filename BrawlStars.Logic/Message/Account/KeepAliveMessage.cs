namespace BrawlStars.Logic.Message.Account;

using BrawlStars.Logic.Enums;

[Piranha(MessageType.KeepAlive)]
public class KeepAliveMessage : PiranhaMessage
{
    public override MessageType MessageType => MessageType.KeepAlive;
    public override ServiceNodeType ServiceNodeType => ServiceNodeType.Account;

    public KeepAliveMessage()
    {
    }
}
