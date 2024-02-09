namespace BrawlStars.Server.Processors.Attributes;

using BrawlStars.Logic.Enums;

[AttributeUsage(AttributeTargets.Method)]
internal class MessageAttribute : Attribute
{
    public MessageType MessageType { get; }

    public MessageAttribute(MessageType type)
    {
        MessageType = type;
    }
}
