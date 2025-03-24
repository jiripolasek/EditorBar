// ------------------------------------------------------------
// 
// Copyright (c) Jiří Polášek. All rights reserved.
// 
// ------------------------------------------------------------

using System.Windows.Media;
using Color = System.Drawing.Color;

namespace JPSoftworks.EditorBar.ViewModels;

public class PhysicalDirectoryBreadcrumbModel : BreadcrumbModel<PhysicalDirectoryModel>
{
    public PhysicalDirectoryBreadcrumbModel(
        PhysicalDirectoryModel model,
        string text,
        Brush background,
        Brush foreground) : base(model, text, background, foreground)
    {
    }

    public PhysicalDirectoryBreadcrumbModel(
        PhysicalDirectoryModel model,
        string text,
        Color background,
        Color foreground) : base(model, text, background, foreground)
    {
    }
}