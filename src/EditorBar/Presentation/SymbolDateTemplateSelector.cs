// ------------------------------------------------------------
// 
// Copyright (c) Jiří Polášek. All rights reserved.
// 
// ------------------------------------------------------------

#nullable enable

using System.Windows;
using System.Windows.Controls;
using JPSoftworks.EditorBar.Services;
using JPSoftworks.EditorBar.Services.StructureProviders.Roslyn;
using JPSoftworks.EditorBar.ViewModels;

namespace JPSoftworks.EditorBar.Presentation;

internal class SymbolDateTemplateSelector : DataTemplateSelector
{
    public DataTemplate? SymbolTemplate { get; set; }

    public DataTemplate? SeparatorTemplate { get; set; }

    public override DataTemplate? SelectTemplate(object? item, DependencyObject container)
    {
        return item switch
        {
            SymbolFileStructureElementModel => this.SymbolTemplate,
            SeparatorListItemViewModel => this.SeparatorTemplate,
            MemberListItemViewModel => this.SymbolTemplate,
            FileStructureElementModel => this.SymbolTemplate,
            _ => base.SelectTemplate(item, container)
        };
    }
}