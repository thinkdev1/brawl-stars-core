namespace BrawlStars.Logic.Serialization;

using System.Reflection;

public class AutoEncoded : IEncodeable
{
    public int EncodingLength { get; private set; }

    public void Decode(ref ByteStream b)
    {
        foreach (var property in GetSerializedProperties())
        {
            if (property.GetValue(this) is IEncodeable encodeable)
            {
                encodeable.Decode(ref b);
            }
        }

        EncodingLength = b.Offset;
    }

    public void Encode(ref ByteStream b)
    {
        foreach (var property in GetSerializedProperties())
        {
            if (property.GetValue(this) is IEncodeable encodeable)
            {
                encodeable.Encode(ref b);
            }
        }

        EncodingLength = b.Offset;
    }

    private IOrderedEnumerable<PropertyInfo> GetSerializedProperties()
    {
        return from property in GetType().GetProperties()
               where Attribute.IsDefined(property, typeof(SerializeAttribute))
               orderby ((SerializeAttribute)property
                         .GetCustomAttributes(typeof(SerializeAttribute), false)
                         .Single()).Order
               select property;
    }
}
