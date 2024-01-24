// ------------------------------------------------------------
//
// Copyright (c) Jiří Polášek. All rights reserved.
//
// ------------------------------------------------------------

using System.Windows;
using System.Windows.Forms;
using System.Windows.Media;
using JPSoftworks.EditorBar.Helpers;

namespace JPSoftworks.EditorBar.Controls;
/// <summary>
/// Interaction logic for ColorButton.xaml
/// </summary>
public partial class ColorButton
{
    public static readonly DependencyProperty SelectedColorProperty = DependencyProperty.Register(nameof(SelectedColor), typeof(Color), typeof(ColorButton), new FrameworkPropertyMetadata(default(Color), FrameworkPropertyMetadataOptions.AffectsRender | FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));

    public Color SelectedColor
    {
        get { return (Color)this.GetValue(SelectedColorProperty); }
        set { this.SetValue(SelectedColorProperty, value); }
    }

    public static readonly DependencyProperty LabelProperty = DependencyProperty.Register(
        nameof(Label), typeof(string), typeof(ColorButton), new PropertyMetadata(default(string)!));

    public string? Label
    {
        get { return (string?)this.GetValue(LabelProperty); }
        set { this.SetValue(LabelProperty, value!); }
    }

    public ColorButton()
    {
        this.InitializeComponent();
    }

    private void ButtonBase_OnClick(object sender, RoutedEventArgs e)
    {
        var colorDialog = new ColorDialog
        {
            Color = this.SelectedColor.ToDrawingColor(),
            FullOpen = true,
            AllowFullOpen = true
        };
        if (colorDialog.ShowDialog() == DialogResult.OK)
        {
            this.SelectedColor = colorDialog.Color.ToMediaColor();
        }
    }
}