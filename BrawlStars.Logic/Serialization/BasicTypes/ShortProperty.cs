namespace BrawlStars.Logic.Serialization.BasicTypes;

public struct ShortProperty : ISerializedProperty<short>
{
    public short Value { get; set; }

    public void Decode(ref ByteStream b)
    {
        Value = b.ReadShort();
    }

    public void Encode(ref ByteStream b)
    {
        b.WriteShort(Value);
    }
}
