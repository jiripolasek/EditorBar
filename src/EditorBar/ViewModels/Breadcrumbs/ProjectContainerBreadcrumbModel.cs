// ------------------------------------------------------------
// 
// Copyright (c) Jiří Polášek. All rights reserved.
// 
// ------------------------------------------------------------

#nullable enable

using System.Windows.Media;
using JPSoftworks.EditorBar.Services.LocationProviders;
using Color = System.Drawing.Color;

namespace JPSoftworks.EditorBar.ViewModels;

public class ProjectContainerBreadcrumbModel : BreadcrumbModel<IProjectInfo>
{
    public ProjectContainerBreadcrumbModel(IProjectInfo model, string text, Brush background, Brush foreground) : base(
        model, text, background, foreground)
    {
    }

    public ProjectContainerBreadcrumbModel(IProjectInfo model, string text, Color background, Color foreground) : base(
        model, text, background, foreground)
    {
    }
}