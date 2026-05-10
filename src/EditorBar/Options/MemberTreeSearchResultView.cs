// ------------------------------------------------------------
// Copyright (c) Jiří Polášek. All rights reserved.
// ------------------------------------------------------------

#nullable enable

using System.ComponentModel;

namespace JPSoftworks.EditorBar.Options;

/// <summary>
/// Defines the preferred default presentation for filtered member-tree results.
/// </summary>
public enum MemberTreeSearchResultView
{
    [Description("Tree")]
    Tree,

    [Description("List")]
    List,

    [Description("Remember last used")]
    RememberLastUsed
}
