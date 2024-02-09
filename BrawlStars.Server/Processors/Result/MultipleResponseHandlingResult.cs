namespace BrawlStars.Server.Processors.Result;

using System.Diagnostics.CodeAnalysis;
using BrawlStars.Logic.Message;

internal class MultipleResponseHandlingResult : IHandlingResult
{
    private readonly Queue<PiranhaMessage> _toSend;

    public MultipleResponseHandlingResult()
    {
        _toSend = new Queue<PiranhaMessage>();
    }

    public void Enqueue(PiranhaMessage message)
    {
        _toSend.Enqueue(message);
    }

    public bool NextMessage([MaybeNullWhen(false)] out PiranhaMessage message)
    {
        return _toSend.TryDequeue(out message);
    }
}
