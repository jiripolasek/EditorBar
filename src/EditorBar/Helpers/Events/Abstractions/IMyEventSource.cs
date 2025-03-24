// ------------------------------------------------------------
// 
// Copyright (c) Jiří Polášek. All rights reserved.
// 
// ------------------------------------------------------------

#nullable enable

namespace JPSoftworks.EditorBar.Helpers.Events.Abstractions;

internal interface IEventAggregator
{
    public event EventHandler? Changed;
}