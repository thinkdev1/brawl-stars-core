namespace BrawlStars.Logic.Message;

using BrawlStars.Logic.Enums;
using BrawlStars.Logic.Serialization;

public abstract class PiranhaMessage : AutoEncoded, IEncodeable
{
    public abstract MessageType MessageType { get; }
    public abstract ServiceNodeType ServiceNodeType { get; }

    public ushort MessageVersion { get; set; }
}
