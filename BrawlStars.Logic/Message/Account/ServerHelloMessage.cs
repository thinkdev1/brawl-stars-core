namespace BrawlStars.Logic.Message.Account;

using BrawlStars.Logic.Enums;
using BrawlStars.Logic.Serialization;
using BrawlStars.Logic.Serialization.BasicTypes;

[Piranha(MessageType.ServerHello)]
public class ServerHelloMessage : PiranhaMessage
{
    public override MessageType MessageType => MessageType.ServerHello;
    public override ServiceNodeType ServiceNodeType => ServiceNodeType.Account;

    [Serialize] public ISerializedProperty<byte[]> ServerNonce { get; set; }

    public ServerHelloMessage()
    {
        ServerNonce = new ByteArrayProperty();
    }
}
