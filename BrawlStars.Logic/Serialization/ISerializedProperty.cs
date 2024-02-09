namespace BrawlStars.Logic.Serialization;

public interface ISerializedProperty<T> : IEncodeable
{
    T Value { get; set; }
}