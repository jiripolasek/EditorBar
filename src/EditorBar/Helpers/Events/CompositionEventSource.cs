// ------------------------------------------------------------
// 
// Copyright (c) Jiří Polášek. All rights reserved.
// 
// ------------------------------------------------------------

#nullable enable

using JPSoftworks.EditorBar.Helpers.Events.Abstractions;

namespace JPSoftworks.EditorBar.Helpers.Events;

internal class CompositionEventAggregator : IEventAggregator
{
    public event EventHandler? Changed
    {
        add
        {
            foreach (var source in this._sources)
            {
                source.Changed += value;
            }
        }
        remove
        {
            foreach (var source in this._sources)
            {
                source.Changed -= value;
            }
        }
    }

    private readonly IEnumerable<IEventAggregator> _sources;

    public CompositionEventAggregator(params IEnumerable<IEventAggregator> sources)
    {
        this._sources = sources;
    }
}