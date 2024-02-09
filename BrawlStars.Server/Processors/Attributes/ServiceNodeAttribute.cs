namespace BrawlStars.Server.Processors.Attributes;

using BrawlStars.Logic.Enums;

[AttributeUsage(AttributeTargets.Class)]
internal class ServiceNodeAttribute : Attribute
{
    public ServiceNodeType ServiceNodeType { get; }

    public ServiceNodeAttribute(ServiceNodeType serviceNodeType)
    {
        ServiceNodeType = serviceNodeType;
    }
}
