namespace BrawlStars.Server.Processors.DependencyInjection;

using System.Collections.Immutable;
using System.Reflection;
using BrawlStars.Logic.Enums;
using BrawlStars.Logic.Message;
using BrawlStars.Server.Processors.Attributes;
using BrawlStars.Server.Processors.Result;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

internal class ProcessorManager
{
    private static readonly ImmutableDictionary<ServiceNodeType, Type> s_processorTypes;
    private readonly IServiceProvider _serviceProvider;
    private readonly ILogger _logger;

    static ProcessorManager()
    {
        var processorTypes = ImmutableDictionary.CreateBuilder<ServiceNodeType, Type>();

        var types = Assembly.GetExecutingAssembly().GetTypes();
        foreach (var type in types)
        {
            var serviceNodeAttribute = type.GetCustomAttribute<ServiceNodeAttribute>();
            if (serviceNodeAttribute == null)
                continue;

            if (!processorTypes.TryGetKey(serviceNodeAttribute.ServiceNodeType, out _))
                processorTypes.Add(serviceNodeAttribute.ServiceNodeType, type);
        }

        s_processorTypes = processorTypes.ToImmutable();
    }

    public ProcessorManager(IServiceProvider serviceProvider, ILogger<ProcessorManager> logger)
    {
        _serviceProvider = serviceProvider;
        _logger = logger;
    }

    public async ValueTask<IHandlingResult?> HandleMessage(PiranhaMessage message)
    {
        if (s_processorTypes.TryGetValue(message.ServiceNodeType, out Type? processorType))
        {
            var processor = ActivatorUtilities.CreateInstance(_serviceProvider, processorType) as Processor;
            return await processor!.ProcessMessage(message, _serviceProvider);
        }
        else
        {
            _logger.LogWarning("No processor implemented for service node type {serviceNodeType}!", message.ServiceNodeType);
        }

        return null;
    }
}
