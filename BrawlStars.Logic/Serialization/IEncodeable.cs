namespace BrawlStars.Logic.Serialization;

public interface IEncodeable
{
    void Encode(ref ByteStream b);
    void Decode(ref ByteStream b);
}
