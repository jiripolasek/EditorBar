// ------------------------------------------------------------
//
// Copyright (c) Jiří Polášek. All rights reserved.
//
// ------------------------------------------------------------

using System.Windows;
using System.Windows.Controls;

namespace JPSoftworks.EditorBar.Controls;

public class ChevronButton : Button
{
    public static readonly DependencyProperty IsMiddleProperty =
        DependencyProperty.Register(nameof(IsMiddle), typeof(bool), typeof(ChevronButton), new PropertyMetadata(true));

    public bool IsMiddle
    {
        get => (bool)this.GetValue(IsMiddleProperty);
        set => this.SetValue(IsMiddleProperty, value);
    }

    static ChevronButton()
    {
        DefaultStyleKeyProperty!.OverrideMetadata(typeof(ChevronButton),
            new FrameworkPropertyMetadata(typeof(ChevronButton)));
    }
}