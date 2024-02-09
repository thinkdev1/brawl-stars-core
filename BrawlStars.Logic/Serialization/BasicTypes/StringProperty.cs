namespace BrawlStars.Logic.Serialization.BasicTypes;

public struct StringProperty : ISerializedProperty<string>
{
    public string Value { get; set; }

    public void Decode(ref ByteStream b)
    {
        Value = b.ReadString();
    }

    public void Encode(ref ByteStream b)
    {
        b.WriteString(Value);
    }
}
