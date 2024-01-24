// ------------------------------------------------------------
//
// Copyright (c) Jiří Polášek. All rights reserved.
//
// ------------------------------------------------------------

using System.Windows;
using System.Windows.Media;

namespace JPSoftworks.EditorBar.Controls;
/// <summary>
/// Interaction logic for ColorSelector.xaml
/// </summary>
public partial class ColorSelector
{
    public static readonly DependencyProperty IsCheckedProperty = DependencyProperty.Register(
        nameof(IsChecked), typeof(bool), typeof(ColorSelector), new FrameworkPropertyMetadata(default(bool), FrameworkPropertyMetadataOptions.AffectsRender | FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));

    public bool IsChecked
    {
        get { return (bool)this.GetValue(IsCheckedProperty); }
        set { this.SetValue(IsCheckedProperty, value); }
    }

    public static readonly DependencyProperty ForegroundColorProperty = DependencyProperty.Register(
        nameof(ForegroundColor), typeof(Color), typeof(ColorSelector), new FrameworkPropertyMetadata(default(Color), FrameworkPropertyMetadataOptions.AffectsRender | FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));

    public Color ForegroundColor
    {
        get { return (Color)this.GetValue(ForegroundColorProperty); }
        set { this.SetValue(ForegroundColorProperty, value); }
    }

    public static readonly DependencyProperty BackgroundColorProperty = DependencyProperty.Register(
        nameof(BackgroundColor), typeof(Color), typeof(ColorSelector), new FrameworkPropertyMetadata(default(Color), FrameworkPropertyMetadataOptions.AffectsRender | FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));

    public Color BackgroundColor
    {
        get { return (Color)this.GetValue(BackgroundColorProperty); }
        set { this.SetValue(BackgroundColorProperty, value); }
    }

    public static readonly DependencyProperty IsCheckableProperty = DependencyProperty.Register(nameof(IsCheckable), typeof(bool), typeof(ColorSelector), new PropertyMetadata(true));

    public bool IsCheckable
    {
        get { return (bool)this.GetValue(IsCheckableProperty); }
        set { this.SetValue(IsCheckableProperty, value); }
    }

    public static readonly DependencyProperty LabelContentProperty = DependencyProperty.Register(nameof(LabelContent), typeof(object), typeof(ColorSelector), new PropertyMetadata(default(object)!));

    public object? LabelContent
    {
        get { return (object?)GetValue(LabelContentProperty); }
        set { SetValue(LabelContentProperty, value!); }
    }

    public ColorSelector()
    {
        this.InitializeComponent();
    }
}
