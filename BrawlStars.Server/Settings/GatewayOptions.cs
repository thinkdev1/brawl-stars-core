namespace BrawlStars.Server.Settings;
using System.Net;

internal class GatewayOptions
{
    public const string ConfigSection = "Gateway";

    public string? Host { get; set; }
    public int Port { get; set; }

    public IPEndPoint BindPoint
    {
        get
        {
            return new IPEndPoint(IPAddress.Parse(Host!), Port);
        }
    }
}
