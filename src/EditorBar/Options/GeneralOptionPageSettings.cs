// ------------------------------------------------------------
// Copyright (c) Jiří Polášek. All rights reserved.
// ------------------------------------------------------------

#nullable enable

using System.Drawing;
using System.Runtime.InteropServices;

namespace JPSoftworks.EditorBar.Options;

/// <summary>
/// Defines the Visual Studio automation settings surface for the Editor Bar options page.
/// </summary>
/// <remarks>
/// Only portable settings belong here. Properties exposed by this automation object participate in
/// Visual Studio roaming and Import/Export, so machine-specific values such as external editor and
/// terminal commands must stay out of this class and remain in the local extension settings store.
/// </remarks>
[ComVisible(true)]
public class GeneralOptionPageSettings
{
    public BarPosition BarPosition { get; set; }

    public bool ShowPathRelativeToSolutionRoot { get; set; }

    public FileLabel FileLabelStyle { get; set; }

    public bool ShowSolutionFolders { get; set; }

    public bool ShowProjectFolders { get; set; }

    public bool ShowParentFolder { get; set; }

    public bool ShowSolutionRoot { get; set; }

    public bool ShowProject { get; set; }

    public DisplayStyle DisplayStyle { get; set; }

    public VisualStyle VisualStyle { get; set; }

    public bool ShowCodeStructureBreadcrumbs { get; set; }

    public bool ShowFileNameBreadcrumb { get; set; }

    public bool Enabled { get; set; }

    public bool ShowOpenDefaultEditorButton { get; set; }

    public bool ShowOpenExternalEditorButton { get; set; }

    public bool ShowOpenContainingFolderButton { get; set; }

    public bool ShowOpenTerminalButton { get; set; }

    public bool DisplayInDiffViews { get; set; }

    public bool DisplayInAuxiliaryDocuments { get; set; }

    public bool DisplayInNonEditableDocuments { get; set; }

    public bool DisplayInBlame { get; set; }

    public bool DisplayInTempFiles { get; set; }

    public FileAction FileAction { get; set; }

    public FileAction AlternateFileAction { get; set; }

    public bool ShowMemberListFilterBoxWhenEmpty { get; set; }

    public bool DebugMode { get; set; }

    public Color SolutionBackground { get; set; }

    public Color SolutionForeground { get; set; }

    public Color NonSolutionRootBackground { get; set; }

    public Color NonSolutionRootForeground { get; set; }

    public Color ProjectBackground { get; set; }

    public Color ProjectForeground { get; set; }

    public Color SolutionFolderBackground { get; set; }

    public Color SolutionFolderForeground { get; set; }

    public Color ParentFolderBackground { get; set; }

    public Color ParentFolderForeground { get; set; }

    public Color ProjectFoldersBackground { get; set; }

    public Color ProjectFoldersForeground { get; set; }

    public Color FileBreadcrumbBackground { get; set; }

    public Color FileBreadcrumbForeground { get; set; }

    public Color StructureBreadcrumbBackground { get; set; }

    public Color StructureBreadcrumbForeground { get; set; }

    public void CopyFromModel(GeneralOptionsModel model)
    {
        this.BarPosition = model.BarPosition;
        this.ShowPathRelativeToSolutionRoot = model.ShowPathRelativeToSolutionRoot;
        this.FileLabelStyle = model.FileLabelStyle;
        this.ShowSolutionFolders = model.ShowSolutionFolders;
        this.ShowProjectFolders = model.ShowProjectFolders;
        this.ShowParentFolder = model.ShowParentFolder;
        this.ShowSolutionRoot = model.ShowSolutionRoot;
        this.ShowProject = model.ShowProject;
        this.DisplayStyle = model.DisplayStyle;
        this.VisualStyle = model.VisualStyle;
        this.ShowCodeStructureBreadcrumbs = model.ShowCodeStructureBreadcrumbs;
        this.ShowFileNameBreadcrumb = model.ShowFileNameBreadcrumb;
        this.Enabled = model.Enabled;
        this.ShowOpenDefaultEditorButton = model.ShowOpenDefaultEditorButton;
        this.ShowOpenExternalEditorButton = model.ShowOpenExternalEditorButton;
        this.ShowOpenContainingFolderButton = model.ShowOpenContainingFolderButton;
        this.ShowOpenTerminalButton = model.ShowOpenTerminalButton;
        this.DisplayInDiffViews = model.DisplayInDiffViews;
        this.DisplayInAuxiliaryDocuments = model.DisplayInAuxiliaryDocuments;
        this.DisplayInNonEditableDocuments = model.DisplayInNonEditableDocuments;
        this.DisplayInBlame = model.DisplayInBlame;
        this.DisplayInTempFiles = model.DisplayInTempFiles;
        this.FileAction = model.FileAction;
        this.AlternateFileAction = model.AlternateFileAction;
        this.ShowMemberListFilterBoxWhenEmpty = model.ShowMemberListFilterBoxWhenEmpty;
        this.DebugMode = model.DebugMode;
        this.SolutionBackground = model.SolutionBackground;
        this.SolutionForeground = model.SolutionForeground;
        this.NonSolutionRootBackground = model.NonSolutionRootBackground;
        this.NonSolutionRootForeground = model.NonSolutionRootForeground;
        this.ProjectBackground = model.ProjectBackground;
        this.ProjectForeground = model.ProjectForeground;
        this.SolutionFolderBackground = model.SolutionFolderBackground;
        this.SolutionFolderForeground = model.SolutionFolderForeground;
        this.ParentFolderBackground = model.ParentFolderBackground;
        this.ParentFolderForeground = model.ParentFolderForeground;
        this.ProjectFoldersBackground = model.ProjectFoldersBackground;
        this.ProjectFoldersForeground = model.ProjectFoldersForeground;
        this.FileBreadcrumbBackground = model.FileBreadcrumbBackground;
        this.FileBreadcrumbForeground = model.FileBreadcrumbForeground;
        this.StructureBreadcrumbBackground = model.StructureBreadcrumbBackground;
        this.StructureBreadcrumbForeground = model.StructureBreadcrumbForeground;
    }

    public void ApplyToModel(GeneralOptionsModel model)
    {
        model.BarPosition = this.BarPosition;
        model.ShowPathRelativeToSolutionRoot = this.ShowPathRelativeToSolutionRoot;
        model.FileLabelStyle = this.FileLabelStyle;
        model.ShowSolutionFolders = this.ShowSolutionFolders;
        model.ShowProjectFolders = this.ShowProjectFolders;
        model.ShowParentFolder = this.ShowParentFolder;
        model.ShowSolutionRoot = this.ShowSolutionRoot;
        model.ShowProject = this.ShowProject;
        model.DisplayStyle = this.DisplayStyle;
        model.VisualStyle = this.VisualStyle;
        model.ShowCodeStructureBreadcrumbs = this.ShowCodeStructureBreadcrumbs;
        model.ShowFileNameBreadcrumb = this.ShowFileNameBreadcrumb;
        model.Enabled = this.Enabled;
        model.ShowOpenDefaultEditorButton = this.ShowOpenDefaultEditorButton;
        model.ShowOpenExternalEditorButton = this.ShowOpenExternalEditorButton;
        model.ShowOpenContainingFolderButton = this.ShowOpenContainingFolderButton;
        model.ShowOpenTerminalButton = this.ShowOpenTerminalButton;
        model.DisplayInDiffViews = this.DisplayInDiffViews;
        model.DisplayInAuxiliaryDocuments = this.DisplayInAuxiliaryDocuments;
        model.DisplayInNonEditableDocuments = this.DisplayInNonEditableDocuments;
        model.DisplayInBlame = this.DisplayInBlame;
        model.DisplayInTempFiles = this.DisplayInTempFiles;
        model.FileAction = this.FileAction;
        model.AlternateFileAction = this.AlternateFileAction;
        model.ShowMemberListFilterBoxWhenEmpty = this.ShowMemberListFilterBoxWhenEmpty;
        model.DebugMode = this.DebugMode;
        model.SolutionBackground = this.SolutionBackground;
        model.SolutionForeground = this.SolutionForeground;
        model.NonSolutionRootBackground = this.NonSolutionRootBackground;
        model.NonSolutionRootForeground = this.NonSolutionRootForeground;
        model.ProjectBackground = this.ProjectBackground;
        model.ProjectForeground = this.ProjectForeground;
        model.SolutionFolderBackground = this.SolutionFolderBackground;
        model.SolutionFolderForeground = this.SolutionFolderForeground;
        model.ParentFolderBackground = this.ParentFolderBackground;
        model.ParentFolderForeground = this.ParentFolderForeground;
        model.ProjectFoldersBackground = this.ProjectFoldersBackground;
        model.ProjectFoldersForeground = this.ProjectFoldersForeground;
        model.FileBreadcrumbBackground = this.FileBreadcrumbBackground;
        model.FileBreadcrumbForeground = this.FileBreadcrumbForeground;
        model.StructureBreadcrumbBackground = this.StructureBreadcrumbBackground;
        model.StructureBreadcrumbForeground = this.StructureBreadcrumbForeground;
    }
}
