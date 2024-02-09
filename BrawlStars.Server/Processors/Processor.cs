namespace BrawlStars.Server.Processors;

using System.Reflection;
using BrawlStars.Logic.Message;
using BrawlStars.Server.Network;
using BrawlStars.Server.Processors.Attributes;
using BrawlStars.Server.Processors.Result;
using Microsoft.Extensions.Logging;

internal class Processor
{
    public ISession Session { get; }

    private readonly ILogger _logger;

    public Processor(ILogger<Processor> logger, ISession session)
    {
        Session = session;
        _logger = logger;
    }

    protected IHandlingResult Ok(PiranhaMessage? message = null)
    {
        return new SingleResponseHandlingResult(message);
    }

    protected IHandlingResult Ok(params PiranhaMessage[] messages)
    {
        var result = new MultipleResponseHandlingResult();

        foreach (var message in messages)
            result.Enqueue(message);

        return result;
    }

    public async ValueTask<IHandlingResult?> ProcessMessage(PiranhaMessage message, IServiceProvider serviceProvider)
    {
        var type = GetType();
        foreach (var method in type.GetMethods())
        {
            var messageAttribute = method.GetCustomAttribute<MessageAttribute>();
            if (messageAttribute == null)
                continue;

            if (messageAttribute.MessageType != message.MessageType)
                continue;

            var parameters = method.GetParameters();
            var callParameters = new object?[parameters.Length];

            for (int i = 0; i < parameters.Length; i++)
            {
                var parameter = parameters[i];
                if (parameter.GetCustomAttribute<FromPayloadAttribute>() != null)
                {
                    callParameters[i] = message;
                }
                else
                {
                    callParameters[i] = serviceProvider.GetService(parameter.ParameterType);
                }
            }

            if (method.ReturnType == typeof(ValueTask<IHandlingResult>))
            {
                return await (ValueTask<IHandlingResult>)method.Invoke(this, callParameters)!;
            }
            else if (method.ReturnType == typeof(IHandlingResult))
            {
                return method.Invoke(this, callParameters) as IHandlingResult;
            }
            else
            {
                throw new InvalidOperationException($"Wrong return type in method {method.Name}, processor: {method.DeclaringType!.Name}");
            }
        }

        _logger.LogWarning("No handler found for message of type {messageType}", message.MessageType);
        return null;
    }
}
