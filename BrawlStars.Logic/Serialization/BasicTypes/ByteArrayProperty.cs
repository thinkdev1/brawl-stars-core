namespace BrawlStars.Logic.Serialization.BasicTypes;

public struct ByteArrayProperty : ISerializedProperty<byte[]>
{
    public byte[] Value { get; set; }

    public void Decode(ref ByteStream b)
    {
        Value = b.ReadBytes(b.ReadInt());
    }

    public void Encode(ref ByteStream b)
    {
        b.WriteInt(Value.Length);
        b.WriteBytes(this.Value);
    }
}
