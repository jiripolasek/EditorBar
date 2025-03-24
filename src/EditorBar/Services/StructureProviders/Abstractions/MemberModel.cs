// ------------------------------------------------------------
// 
// Copyright (c) Jiří Polášek. All rights reserved.
// 
// ------------------------------------------------------------

#nullable enable

using JPSoftworks.EditorBar.Helpers;

namespace JPSoftworks.EditorBar.Services.StructureProviders;

public class TypeMemberModel : BaseStructureModel
{
    public TypeMemberModel(string displayName, StackedImageMoniker imageMoniker, AnchorPoint anchorPoint)
        : base(displayName, imageMoniker, anchorPoint)
    {
    }
}