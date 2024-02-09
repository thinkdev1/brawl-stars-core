namespace BrawlStars.Logic.Message.Account;

using BrawlStars.Logic.Enums;
using BrawlStars.Logic.Serialization;
using BrawlStars.Logic.Serialization.BasicTypes;

[Piranha(MessageType.ClientHello)]
public class ClientHelloMessage : PiranhaMessage
{
    public override MessageType MessageType => MessageType.ClientHello;
    public override ServiceNodeType ServiceNodeType => ServiceNodeType.Account;

    [Serialize] public ISerializedProperty<int> Protocol { get; set; }
    [Serialize] public ISerializedProperty<int> KeyVersion { get; set; }
    [Serialize] public ISerializedProperty<int> MajorVersion { get; set; }
    [Serialize] public ISerializedProperty<int> MinorVersion { get; set; }
    [Serialize] public ISerializedProperty<int> BuildVersion { get; set; }
    [Serialize] public ISerializedProperty<string> ContentHash { get; set; }
    [Serialize] public ISerializedProperty<int> DeviceType { get; set; }
    [Serialize] public ISerializedProperty<int> AppStore { get; set; }

    public ClientHelloMessage()
    {
        Protocol = new IntProperty();
        KeyVersion = new IntProperty();
        MajorVersion = new IntProperty();
        MinorVersion = new IntProperty();
        BuildVersion = new IntProperty();
        ContentHash = new StringProperty();
        DeviceType = new IntProperty();
        AppStore = new IntProperty();
    }
}
