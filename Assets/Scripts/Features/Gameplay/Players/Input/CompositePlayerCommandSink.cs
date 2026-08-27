using System;
using System.Collections.Generic;
using System.Linq;

public sealed class CompositePlayerCommandSink : IPlayerCommandSink
{
    private readonly IReadOnlyList<IPlayerCommandSink> _sinks;

    public CompositePlayerCommandSink(params IPlayerCommandSink[] sinks)
    {
        if (sinks == null || sinks.Length == 0)
            throw new ArgumentException(nameof(sinks));
        
        if (sinks.Any(sink => sink == null))
            throw new ArgumentException(nameof(sinks));

        _sinks = sinks;
    }

    public void Submit(in PlayerCommand playerCommand)
    {
        foreach (IPlayerCommandSink sink in _sinks)
            sink.Submit(playerCommand);
    }
}
