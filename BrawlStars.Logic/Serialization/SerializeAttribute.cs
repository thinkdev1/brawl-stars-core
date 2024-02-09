namespace BrawlStars.Logic.Serialization;

using System.Runtime.CompilerServices;

[AttributeUsage(AttributeTargets.Property)]
public class SerializeAttribute : Attribute
{
    public int Order { get; }

    public SerializeAttribute([CallerLineNumber] int order = 0)
    {
        Order = order;
    }
}
