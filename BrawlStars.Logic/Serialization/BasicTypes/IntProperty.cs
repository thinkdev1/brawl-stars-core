namespace BrawlStars.Logic.Serialization.BasicTypes;

public struct IntProperty : ISerializedProperty<int>
{
    public int Value { get; set; }

    public void Decode(ref ByteStream b)
    {
        Value = b.ReadInt();
    }

    public void Encode(ref ByteStream b)
    {
        b.WriteInt(Value);
    }
}
