namespace BrawlStars.Logic.Serialization.BasicTypes;

public struct VIntProperty : ISerializedProperty<int>
{
    public int Value { get; set; }

    public void Decode(ref ByteStream b)
    {
        Value = b.ReadVInt();
    }

    public void Encode(ref ByteStream b)
    {
        b.WriteVInt(Value);
    }
}
