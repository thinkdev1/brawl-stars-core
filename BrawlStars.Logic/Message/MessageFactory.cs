namespace BrawlStars.Logic.Message;

using System.Collections.Immutable;
using System.Reflection;
using BrawlStars.Logic.Enums;

public static class MessageFactory
{
    private static readonly ImmutableDictionary<MessageType, Type> s_messageTypes;

    static MessageFactory()
    {
        var messageTypes = ImmutableDictionary.CreateBuilder<MessageType, Type>();
        var types = Assembly.GetExecutingAssembly().GetTypes();
        
        foreach (var type in types)
        {
            var piranhaAttribute = type.GetCustomAttribute<PiranhaAttribute>();
            if (piranhaAttribute == null)
                continue;

            if (!messageTypes.TryGetKey(piranhaAttribute.MessageType, out _))
                messageTypes.Add(piranhaAttribute.MessageType, type);
        }

        s_messageTypes = messageTypes.ToImmutable();
    }

    public static PiranhaMessage? CreateMessageByType(MessageType messageType)
    {
        if (s_messageTypes.TryGetValue(messageType, out Type? type))
            return Activator.CreateInstance(type!) as PiranhaMessage;

        return null;
    }
}
