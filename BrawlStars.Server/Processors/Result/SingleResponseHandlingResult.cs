namespace BrawlStars.Server.Processors.Result;

using System.Diagnostics.CodeAnalysis;
using BrawlStars.Logic.Message;

internal class SingleResponseHandlingResult : IHandlingResult
{
    private PiranhaMessage? _message;

    public SingleResponseHandlingResult(PiranhaMessage? message)
    {
        _message = message;
    }

    public bool NextMessage([MaybeNullWhen(false)] out PiranhaMessage message)
    {
        message = _message;
        _message = null;

        return message != null;
    }
}
