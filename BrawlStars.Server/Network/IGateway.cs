namespace BrawlStars.Server.Network;

internal interface IGateway
{
    void Start(CancellationToken cancellationToken);
    void Stop();
}
