namespace BrawlStars.Logic.Serialization.BasicTypes;

public struct ByteProperty : ISerializedProperty<byte>
{
    public byte Value { get; set; }

    public void Decode(ref ByteStream b)
    {
        Value = b.ReadByte();
    }

    public void Encode(ref ByteStream b)
    {
        b.WriteByte(Value);
    }
}
