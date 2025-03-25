// ------------------------------------------------------------
// 
// Copyright (c) Jiří Polášek. All rights reserved.
// 
// ------------------------------------------------------------

#nullable enable

using System.Windows;
using System.Windows.Input;
using Microsoft.VisualStudio.PlatformUI;

namespace JPSoftworks.EditorBar.Controls;

/// <summary>
/// Interaction logic for EditorBarBreadcrumbs.xaml
/// </summary>
public partial class EditorBarBreadcrumbs
{
    public EditorBarBreadcrumbs()
    {
        this.InitializeComponent();
    }

    public void FocusFirstBreadcrumbOfType<TBreadcrumb>(Func<TBreadcrumb, bool>? condition = null)
    {
        var firstProjectModel =
            this.BreadcrumbsItemsControl!.ItemContainerGenerator.Items?.FirstOrDefault(t =>
                t is TBreadcrumb b && (condition == null || condition(b)));
        this.FocusBreadcrumb(firstProjectModel);
    }

    public void FocusLastBreadcrumbOfType<TBreadcrumb>(Func<TBreadcrumb, bool>? condition = null)
    {
        var firstProjectModel =
            this.BreadcrumbsItemsControl!.ItemContainerGenerator.Items?.LastOrDefault(t =>
                t is TBreadcrumb b && (condition == null || condition(b)));
        this.FocusBreadcrumb(firstProjectModel);
    }

    private void FocusBreadcrumb(object? firstProjectModel)
    {
        if (firstProjectModel == null ||
            this.BreadcrumbsItemsControl!.ItemContainerGenerator.ContainerFromItem(firstProjectModel) is not UIElement
                uiElement)
        {
            return;
        }

        uiElement.AcquireWin32Focus(out _);
        uiElement.MoveFocus(new TraversalRequest(FocusNavigationDirection.Next));
    }
}