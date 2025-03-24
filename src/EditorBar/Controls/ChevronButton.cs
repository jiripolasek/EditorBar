// ------------------------------------------------------------
// 
// Copyright (c) Jiří Polášek. All rights reserved.
// 
// ------------------------------------------------------------

#nullable enable

using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace JPSoftworks.EditorBar.Controls;

public class ChevronButton : ContentControl
{
    public static readonly DependencyProperty ContextCommandProperty = DependencyProperty.Register(
        nameof(ContextCommand), typeof(ICommand), typeof(ChevronButton), new PropertyMetadata(null!));

    public static readonly DependencyProperty ShowRightBorderProperty = DependencyProperty.Register(
        nameof(ShowRightBorder), typeof(bool), typeof(ChevronButton), new PropertyMetadata(false));

    public static readonly DependencyProperty RightBorderBrushProperty = DependencyProperty.Register(
        nameof(RightBorderBrush), typeof(Brush), typeof(ChevronButton), new PropertyMetadata(null!));

    public bool ShowRightBorder
    {
        get => (bool)this.GetValue(ShowRightBorderProperty);
        set => this.SetValue(ShowRightBorderProperty, value);
    }

    public Brush RightBorderBrush
    {
        get => (Brush)this.GetValue(RightBorderBrushProperty)!;
        set => this.SetValue(RightBorderBrushProperty, value);
    }

    public ICommand? ContextCommand
    {
        get => (ICommand)this.GetValue(ContextCommandProperty)!;
        set => this.SetValue(ContextCommandProperty, value!);
    }

    static ChevronButton()
    {
        DefaultStyleKeyProperty!.OverrideMetadata(typeof(ChevronButton),
            new FrameworkPropertyMetadata(typeof(ChevronButton)));
    }
}