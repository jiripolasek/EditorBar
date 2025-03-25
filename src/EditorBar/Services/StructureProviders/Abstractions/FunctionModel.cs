// ------------------------------------------------------------
// 
// Copyright (c) Jiří Polášek. All rights reserved.
// 
// ------------------------------------------------------------

#nullable enable

using JPSoftworks.EditorBar.Helpers;

namespace JPSoftworks.EditorBar.Services.StructureProviders;

public class FunctionModel : TypeMemberModel
{
    public FunctionModel(string displayName, StackedImageMoniker imageMoniker, AnchorPoint anchorPoint)
        : base(displayName, imageMoniker, anchorPoint)
    {
    }
}