namespace BrawlStars.Logic.Serialization.BasicTypes;

public struct EncodeableProperty<T> : ISerializedProperty<T> where T : IEncodeable
{
    public T Value { get; set; }

    public void Decode(ref ByteStream b)
    {
        Value.Decode(ref b);
    }

    public void Encode(ref ByteStream b)
    {
        Value.Encode(ref b);
    }
}
