// ------------------------------------------------------------
// 
// Copyright (c) Jiří Polášek. All rights reserved.
// 
// ------------------------------------------------------------

#nullable enable

using System.Windows;
using System.Windows.Media;

namespace JPSoftworks.EditorBar.Controls;

/// <summary>
/// Interaction logic for ColorSelector.xaml
/// </summary>
public partial class ColorSelector
{
    public static readonly DependencyProperty IsCheckedProperty = DependencyProperty.Register(
        nameof(IsChecked), typeof(bool), typeof(ColorSelector),
        new FrameworkPropertyMetadata(default(bool),
            FrameworkPropertyMetadataOptions.AffectsRender | FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));

    public static readonly DependencyProperty ForegroundColorProperty = DependencyProperty.Register(
        nameof(ForegroundColor), typeof(Color), typeof(ColorSelector),
        new FrameworkPropertyMetadata(default(Color),
            FrameworkPropertyMetadataOptions.AffectsRender | FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));

    public static readonly DependencyProperty BackgroundColorProperty = DependencyProperty.Register(
        nameof(BackgroundColor), typeof(Color), typeof(ColorSelector),
        new FrameworkPropertyMetadata(default(Color),
            FrameworkPropertyMetadataOptions.AffectsRender | FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));

    public static readonly DependencyProperty IsCheckableProperty = DependencyProperty.Register(nameof(IsCheckable),
        typeof(bool), typeof(ColorSelector), new PropertyMetadata(true));

    public static readonly DependencyProperty LabelContentProperty = DependencyProperty.Register(nameof(LabelContent),
        typeof(object), typeof(ColorSelector), new PropertyMetadata(default(object)!));

    public bool IsChecked
    {
        get => (bool)this.GetValue(IsCheckedProperty);
        set => this.SetValue(IsCheckedProperty, value);
    }

    public Color ForegroundColor
    {
        get => (Color)this.GetValue(ForegroundColorProperty);
        set => this.SetValue(ForegroundColorProperty, value);
    }

    public Color BackgroundColor
    {
        get => (Color)this.GetValue(BackgroundColorProperty);
        set => this.SetValue(BackgroundColorProperty, value);
    }

    public bool IsCheckable
    {
        get => (bool)this.GetValue(IsCheckableProperty);
        set => this.SetValue(IsCheckableProperty, value);
    }

    public object? LabelContent
    {
        get => (object?)this.GetValue(LabelContentProperty);
        set => this.SetValue(LabelContentProperty, value!);
    }

    public ColorSelector()
    {
        this.InitializeComponent();
    }
}