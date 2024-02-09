namespace BrawlStars.Server.Processors.Result;

using System.Diagnostics.CodeAnalysis;
using BrawlStars.Logic.Message;

internal interface IHandlingResult
{
    bool NextMessage([MaybeNullWhen(false)] out PiranhaMessage message);
}
