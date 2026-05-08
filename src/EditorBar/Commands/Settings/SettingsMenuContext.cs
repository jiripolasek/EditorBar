// ------------------------------------------------------------
// Copyright (c) Jiří Polášek. All rights reserved.
// ------------------------------------------------------------

#nullable enable

using JPSoftworks.EditorBar.Commands.Abstractions;
using Microsoft.VisualStudio.Text.Editor;

namespace JPSoftworks.EditorBar.Commands;

[MenuId(PackageGuids.EditorBarCmdSetString, PackageIds.EditorBarSettingsMenu)]
internal sealed record SettingsMenuContext(IWpfTextView? CurrentTextView) : MenuContext;
