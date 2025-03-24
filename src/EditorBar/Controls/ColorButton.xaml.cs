// ------------------------------------------------------------
// 
// Copyright (c) Jiří Polášek. All rights reserved.
// 
// ------------------------------------------------------------

#nullable enable

using System.Windows;
using System.Windows.Forms;
using System.Windows.Media;
using JPSoftworks.EditorBar.Helpers.Presentation;
using Button = System.Windows.Controls.Button;

namespace JPSoftworks.EditorBar.Controls;

/// <summary>
/// Interaction logic for ColorButton.xaml
/// </summary>
public partial class ColorButton
{
    public static readonly DependencyProperty SelectedColorProperty = DependencyProperty.Register(nameof(SelectedColor),
        typeof(Color), typeof(ColorButton),
        new FrameworkPropertyMetadata(default(Color),
            FrameworkPropertyMetadataOptions.AffectsRender | FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));

    public static readonly DependencyProperty LabelProperty = DependencyProperty.Register(
        nameof(Label), typeof(string), typeof(ColorButton), new PropertyMetadata(default(string)!));

    public Color SelectedColor
    {
        get => (Color)this.GetValue(SelectedColorProperty);
        set => this.SetValue(SelectedColorProperty, value);
    }

    public string? Label
    {
        get => (string?)this.GetValue(LabelProperty);
        set => this.SetValue(LabelProperty, value!);
    }

    public ColorButton()
    {
        this.InitializeComponent();
    }

    private void ButtonBase_OnClick(object sender, RoutedEventArgs e)
    {
        var colorDialog = new ColorDialog
        {
            Color = this.SelectedColor.ToDrawingColor(), FullOpen = true, AllowFullOpen = true
        };
        if (colorDialog.ShowDialog() == DialogResult.OK)
        {
            this.SelectedColor = colorDialog.Color.ToMediaColor();
        }
    }

    private void ToggleButton_OnClick(object sender, RoutedEventArgs e)
    {
        this.Popup!.IsOpen = true;
        this.Popup!.Focus();
    }

    private void SetColor_Click(object sender, RoutedEventArgs e)
    {
        if (sender is Button { Tag: Color color })
        {
            this.SelectedColor = color;
            this.Popup!.IsOpen = false;
        }
    }
}