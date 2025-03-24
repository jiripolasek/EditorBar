// ------------------------------------------------------------
// 
// Copyright (c) Jiří Polášek. All rights reserved.
// 
// ------------------------------------------------------------

#nullable enable

using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using JPSoftworks.EditorBar.Helpers;
using JPSoftworks.EditorBar.Helpers.Presentation;
using DrawingColor = System.Drawing.Color;

namespace JPSoftworks.EditorBar.ViewModels;

public class BreadcrumbModel
{
    public bool IsFirst { get; set; }

    public bool IsLast { get; set; }

    public bool IsMiddle => !this.IsFirst && !this.IsLast;

    public BreadcrumbModel? PreviousBreadcrumb { get; set; }

    public BreadcrumbModel? NextBreadcrumb { get; set; }

    public Brush Background { get; set; }

    public Brush Foreground { get; }

    public string Text { get; }

    public ICommand? Command { get; }

    public ICommand? ContextCommand { get; init; }

    public string? AssociatedFile { get; init; }

    public string? AssociatedDirectory { get; init; }

    public UIElement? Icon { get; set; }

    public StackedImageMoniker ImageMoniker { get; set; }

    public Func<Task<IList<MemberListItemViewModel>>>? ItemsProvider { get; init; }

    public bool CanHaveChildren => this.ItemsProvider != null;

    public BreadcrumbModel(string text, Brush background, Brush foreground)
    {
        this.Text = text;
        this.Background = background;
        this.Foreground = foreground;
        this.Command = null;
    }

    public BreadcrumbModel(string text, DrawingColor background, DrawingColor foreground)
    {
        this.Text = text;
        this.Background = new SolidColorBrush(background.ToMediaColor());
        this.Background.Freeze();

        this.Foreground = new SolidColorBrush(foreground.ToMediaColor());
        this.Foreground.Freeze();
        this.Command = null;
    }
}