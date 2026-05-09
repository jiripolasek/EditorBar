// ------------------------------------------------------------
// Copyright (c) Jiří Polášek. All rights reserved.
// ------------------------------------------------------------

#nullable enable

using JPSoftworks.EditorBar.Commands.Abstractions;
using Microsoft.VisualStudio.Text;

namespace JPSoftworks.EditorBar.Commands;

[MenuId(PackageGuids.EditorBarCmdSetString, PackageIds.EditorBarFileActionMenu)]
internal sealed record FileActionMenuContext(ITextDocument? CurrentDocument, string? FilePath) : MenuContext
{
    public FileActionMenuContext(ITextDocument? currentDocument)
        : this(currentDocument, currentDocument?.FilePath)
    {
    }

    public FileActionMenuContext(string? filePath)
        : this(null, filePath)
    {
    }

    public string? TargetFilePath => this.CurrentDocument?.FilePath ?? this.FilePath;

    public override bool Validate()
    {
        return !string.IsNullOrWhiteSpace(this.TargetFilePath);
    }
}
