namespace BrawlStars.Logic.Message;

using BrawlStars.Logic.Enums;

[AttributeUsage(AttributeTargets.Class)]
internal class PiranhaAttribute : Attribute
{
    public MessageType MessageType { get; set; }

    public PiranhaAttribute(MessageType messageType)
    {
        MessageType = messageType;
    }
}
