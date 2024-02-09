namespace BrawlStars.Logic.Serialization.BasicTypes;

public struct BoolProperty : ISerializedProperty<bool>
{
    public bool Value { get; set; }

    public void Decode(ref ByteStream b)
    {
        Value = b.ReadBoolean();
    }

    public void Encode(ref ByteStream b)
    {
        b.WriteBoolean(Value);
    }
}
