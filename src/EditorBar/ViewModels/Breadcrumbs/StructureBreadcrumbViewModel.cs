// ------------------------------------------------------------
// 
// Copyright (c) Jiří Polášek. All rights reserved.
// 
// ------------------------------------------------------------

#nullable enable

using JPSoftworks.EditorBar.Services.StructureProviders;
using Color = System.Drawing.Color;

namespace JPSoftworks.EditorBar.ViewModels;

public class StructureBreadcrumbViewModel : BreadcrumbModel<BaseStructureModel>
{
    public StructureBreadcrumbViewModel(
        BaseStructureModel model,
        string text,
        Color background,
        Color foreground) : base(
        model, text, background, foreground)
    {
    }
}