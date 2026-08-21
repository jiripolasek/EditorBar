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

    public bool ShowLocateInSolutionExplorerButton { get; set; }

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

    public MemberTreeSearchResultView MemberTreeSearchResultViewDefault { get; set; }

    public bool DebugMode { get; set; }

    public Color DarkSolutionBackground { get; set; }

    public Color DarkSolutionForeground { get; set; }

    public Color DarkNonSolutionRootBackground { get; set; }

    public Color DarkNonSolutionRootForeground { get; set; }

    public Color DarkProjectBackground { get; set; }

    public Color DarkProjectForeground { get; set; }

    public Color DarkSolutionFolderBackground { get; set; }

    public Color DarkSolutionFolderForeground { get; set; }

    public Color DarkParentFolderBackground { get; set; }

    public Color DarkParentFolderForeground { get; set; }

    public Color DarkProjectFoldersBackground { get; set; }

    public Color DarkProjectFoldersForeground { get; set; }

    public Color DarkFileBreadcrumbBackground { get; set; }

    public Color DarkFileBreadcrumbForeground { get; set; }

    public Color DarkStructureBreadcrumbBackground { get; set; }

    public Color DarkStructureBreadcrumbForeground { get; set; }

    public Color LightSolutionBackground { get; set; }

    public Color LightSolutionForeground { get; set; }

    public Color LightNonSolutionRootBackground { get; set; }

    public Color LightNonSolutionRootForeground { get; set; }

    public Color LightProjectBackground { get; set; }

    public Color LightProjectForeground { get; set; }

    public Color LightSolutionFolderBackground { get; set; }

    public Color LightSolutionFolderForeground { get; set; }

    public Color LightParentFolderBackground { get; set; }

    public Color LightParentFolderForeground { get; set; }

    public Color LightProjectFoldersBackground { get; set; }

    public Color LightProjectFoldersForeground { get; set; }

    public Color LightFileBreadcrumbBackground { get; set; }

    public Color LightFileBreadcrumbForeground { get; set; }

    public Color LightStructureBreadcrumbBackground { get; set; }

    public Color LightStructureBreadcrumbForeground { get; set; }

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
        this.ShowLocateInSolutionExplorerButton = model.ShowLocateInSolutionExplorerButton;
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
        this.MemberTreeSearchResultViewDefault = model.MemberTreeSearchResultViewDefault;
        this.DebugMode = model.DebugMode;
        this.DarkSolutionBackground = model.SolutionBackground;
        this.DarkSolutionForeground = model.SolutionForeground;
        this.DarkNonSolutionRootBackground = model.NonSolutionRootBackground;
        this.DarkNonSolutionRootForeground = model.NonSolutionRootForeground;
        this.DarkProjectBackground = model.ProjectBackground;
        this.DarkProjectForeground = model.ProjectForeground;
        this.DarkSolutionFolderBackground = model.SolutionFolderBackground;
        this.DarkSolutionFolderForeground = model.SolutionFolderForeground;
        this.DarkParentFolderBackground = model.ParentFolderBackground;
        this.DarkParentFolderForeground = model.ParentFolderForeground;
        this.DarkProjectFoldersBackground = model.ProjectFoldersBackground;
        this.DarkProjectFoldersForeground = model.ProjectFoldersForeground;
        this.DarkFileBreadcrumbBackground = model.FileBreadcrumbBackground;
        this.DarkFileBreadcrumbForeground = model.FileBreadcrumbForeground;
        this.DarkStructureBreadcrumbBackground = model.StructureBreadcrumbBackground;
        this.DarkStructureBreadcrumbForeground = model.StructureBreadcrumbForeground;
        this.LightSolutionBackground = model.LightSolutionBackground;
        this.LightSolutionForeground = model.LightSolutionForeground;
        this.LightNonSolutionRootBackground = model.LightNonSolutionRootBackground;
        this.LightNonSolutionRootForeground = model.LightNonSolutionRootForeground;
        this.LightProjectBackground = model.LightProjectBackground;
        this.LightProjectForeground = model.LightProjectForeground;
        this.LightSolutionFolderBackground = model.LightSolutionFolderBackground;
        this.LightSolutionFolderForeground = model.LightSolutionFolderForeground;
        this.LightParentFolderBackground = model.LightParentFolderBackground;
        this.LightParentFolderForeground = model.LightParentFolderForeground;
        this.LightProjectFoldersBackground = model.LightProjectFoldersBackground;
        this.LightProjectFoldersForeground = model.LightProjectFoldersForeground;
        this.LightFileBreadcrumbBackground = model.LightFileBreadcrumbBackground;
        this.LightFileBreadcrumbForeground = model.LightFileBreadcrumbForeground;
        this.LightStructureBreadcrumbBackground = model.LightStructureBreadcrumbBackground;
        this.LightStructureBreadcrumbForeground = model.LightStructureBreadcrumbForeground;
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
        model.ShowLocateInSolutionExplorerButton = this.ShowLocateInSolutionExplorerButton;
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
        model.MemberTreeSearchResultViewDefault = this.MemberTreeSearchResultViewDefault;
        model.DebugMode = this.DebugMode;
        model.SolutionBackground = this.DarkSolutionBackground;
        model.SolutionForeground = this.DarkSolutionForeground;
        model.NonSolutionRootBackground = this.DarkNonSolutionRootBackground;
        model.NonSolutionRootForeground = this.DarkNonSolutionRootForeground;
        model.ProjectBackground = this.DarkProjectBackground;
        model.ProjectForeground = this.DarkProjectForeground;
        model.SolutionFolderBackground = this.DarkSolutionFolderBackground;
        model.SolutionFolderForeground = this.DarkSolutionFolderForeground;
        model.ParentFolderBackground = this.DarkParentFolderBackground;
        model.ParentFolderForeground = this.DarkParentFolderForeground;
        model.ProjectFoldersBackground = this.DarkProjectFoldersBackground;
        model.ProjectFoldersForeground = this.DarkProjectFoldersForeground;
        model.FileBreadcrumbBackground = this.DarkFileBreadcrumbBackground;
        model.FileBreadcrumbForeground = this.DarkFileBreadcrumbForeground;
        model.StructureBreadcrumbBackground = this.DarkStructureBreadcrumbBackground;
        model.StructureBreadcrumbForeground = this.DarkStructureBreadcrumbForeground;
        model.LightSolutionBackground = this.LightSolutionBackground;
        model.LightSolutionForeground = this.LightSolutionForeground;
        model.LightNonSolutionRootBackground = this.LightNonSolutionRootBackground;
        model.LightNonSolutionRootForeground = this.LightNonSolutionRootForeground;
        model.LightProjectBackground = this.LightProjectBackground;
        model.LightProjectForeground = this.LightProjectForeground;
        model.LightSolutionFolderBackground = this.LightSolutionFolderBackground;
        model.LightSolutionFolderForeground = this.LightSolutionFolderForeground;
        model.LightParentFolderBackground = this.LightParentFolderBackground;
        model.LightParentFolderForeground = this.LightParentFolderForeground;
        model.LightProjectFoldersBackground = this.LightProjectFoldersBackground;
        model.LightProjectFoldersForeground = this.LightProjectFoldersForeground;
        model.LightFileBreadcrumbBackground = this.LightFileBreadcrumbBackground;
        model.LightFileBreadcrumbForeground = this.LightFileBreadcrumbForeground;
        model.LightStructureBreadcrumbBackground = this.LightStructureBreadcrumbBackground;
        model.LightStructureBreadcrumbForeground = this.LightStructureBreadcrumbForeground;
    }
}
