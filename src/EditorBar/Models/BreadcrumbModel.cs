// ------------------------------------------------------------
//
// Copyright (c) Jiří Polášek. All rights reserved.
//
// ------------------------------------------------------------

using System.Windows.Input;
using System.Windows.Media;
using JPSoftworks.EditorBar.Helpers;

namespace JPSoftworks.EditorBar.Models;

public class BreadcrumbModel
{
    public Brush Background { get; }

    public Brush Foreground { get; }

    public string Text { get; }

    public bool IsMiddle { get; set; }

    public ICommand? Command { get; }

    public string? AssociatedFile { get; set; }

    public string? AssociatedDirectory { get; set; }

    public BreadcrumbModel(string text, Brush background, Brush foreground, bool isMiddle = true)
    {
        this.Text = text;
        this.Background = background;
        this.Foreground = foreground;
        this.IsMiddle = isMiddle;
        this.Command = null;
    }

    public BreadcrumbModel(string text, System.Drawing.Color background, System.Drawing.Color foreground)
    {
        this.Text = text;
        this.Background = new SolidColorBrush(background.ToMediaColor());
        this.Foreground = new SolidColorBrush(foreground.ToMediaColor());
        this.IsMiddle = true;
        this.Command = null;
    }
}