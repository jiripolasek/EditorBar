// ------------------------------------------------------------
// 
// Copyright (c) Jiří Polášek. All rights reserved.
// 
// ------------------------------------------------------------

#nullable enable

using JPSoftworks.EditorBar.Helpers;
using Microsoft.VisualStudio.Text.Editor;

namespace JPSoftworks.EditorBar.Services;

public record FileStructureElementModel
{
    public string PrimaryName { get; init; }

    public string? SecondaryName { get; init; }

    public string SearchText { get; init; }

    public StackedImageMoniker? ImageMoniker { get; init; }

    public required Action<IWpfTextView> NavigationAction { get; init; }
}