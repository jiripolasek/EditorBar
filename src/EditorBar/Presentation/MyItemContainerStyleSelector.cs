// ------------------------------------------------------------
// 
// Copyright (c) Jiří Polášek. All rights reserved.
// 
// ------------------------------------------------------------

#nullable enable

using System.Windows;
using System.Windows.Controls;
using JPSoftworks.EditorBar.ViewModels;

namespace JPSoftworks.EditorBar.Presentation;

internal sealed class MyItemContainerStyleSelector : StyleSelector
{
    public Style? NormalItemStyle { get; set; }
    public Style? SeparatorItemStyle { get; set; }

    public override Style SelectStyle(object item, DependencyObject container)
    {
        return item switch
        {
            SeparatorListItemViewModel => this.SeparatorItemStyle!,
            _ => this.NormalItemStyle!
        };
    }
}