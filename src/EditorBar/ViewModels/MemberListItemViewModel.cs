// ------------------------------------------------------------
// 
// Copyright (c) Jiří Polášek. All rights reserved.
// 
// ------------------------------------------------------------

#nullable enable

using System.Windows;
using System.Windows.Input;
using JPSoftworks.EditorBar.Helpers;
using JPSoftworks.EditorBar.Services;

namespace JPSoftworks.EditorBar.ViewModels;

public record MemberListItemViewModel
{
    public StackedImageMoniker? ImageMoniker { get; init; }

    public string PrimaryName { get; init; }

    public string? SecondaryName { get; init; }

    public string SearchText { get; init; }

    public ICommand? Command { get; set; }

    public object? CommandParameter { get; set; }

    [Obsolete] public UIElement? Icon { get; set; }

    public static MemberListItemViewModel FromModel(FileStructureElementModel model)
    {
        return new MemberListItemViewModel
        {
            ImageMoniker = model.ImageMoniker,
            PrimaryName = model.PrimaryName,
            SecondaryName = model.SecondaryName,
            SearchText = model.SearchText
        };
    }
}