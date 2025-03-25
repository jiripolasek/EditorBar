// ------------------------------------------------------------
// 
// Copyright (c) Jiří Polášek. All rights reserved.
// 
// ------------------------------------------------------------

#nullable enable

using System.Collections.ObjectModel;
using System.Windows.Input;
using JPSoftworks.EditorBar.Helpers.Presentation;
using JPSoftworks.EditorBar.Options;
using Microsoft.VisualStudio.PlatformUI;
using Microsoft.Win32;

namespace JPSoftworks.EditorBar.ViewModels;

/// <summary>
/// OptionsPageViewModel manages various settings for an options page, including display styles, editor paths, and user
/// preferences.
/// </summary>
public class OptionsPageViewModel : ObservableObject
{
    private BarPosition _barPosition;
    private bool _displayInAuxiliaryDocuments;
    private bool _displayInBlame;
    private bool _displayInDiffViews;
    private bool _displayInNonEditableDocuments;
    private bool _displayInTemp;
    private DisplayStyle _displayStyle;
    private FileAction _doubleClickActionOnFileLabel;
    private FileAction _doubleClickCtrlActionOnFileLabel;
    private string? _externalEditorArguments;
    private string? _externalEditorPath;
    private bool _isDebugModeEnabled;
    private bool _isEnabled;
    private FileLabel _pathStyle;
    private VisualStyle _visualStyle;

    /// <summary>
    /// Contains available options for the bar position.
    /// </summary>
    public ObservableCollection<EnumViewModel<BarPosition>> BarPositions { get; } =
    [
        new(BarPosition.Top, "Top"),
        new(BarPosition.Bottom, "Bottom")
    ];

    /// <summary>
    /// Contains available options for the display style.
    /// </summary>
    public ObservableCollection<EnumViewModel<DisplayStyle>> DisplayStyles { get; } =
    [
        new(DisplayStyle.Normal, "Normal"),
        new(DisplayStyle.Compact, "Compact")
    ];

    /// <summary>
    /// Contains available options for the visual style.
    /// </summary>
    public ObservableCollection<EnumViewModel<VisualStyle>> VisualStyles { get; } =
    [
        new(VisualStyle.FullRowTransparent, "Transparent"),
        new(VisualStyle.FullRowCommandBar, "Command Bar"),
        new(VisualStyle.FullRowToolWindow, "Tool Window")
    ];

    /// <summary>
    /// Contains available options for the path style.
    /// </summary>
    public ObservableCollection<EnumViewModel<FileLabel>> PathStyles { get; } =
    [
        new(FileLabel.Hidden, "None (hidden)"),
        new(FileLabel.RelativePathInProject, "Relative Path (in project)"),
        new(FileLabel.RelativePathInSolution, "Relative Path (in solution)"),
        new(FileLabel.AbsolutePath, "Absolute Path"),
        new(FileLabel.FileName, "File Name Only")
    ];

    /// <summary>
    /// Contains available options for the double-click action.
    /// </summary>
    public ObservableCollection<EnumViewModel<FileAction>> DoubleClickActions { get; } =
    [
        new(FileAction.None, "Do nothing"),
        new(FileAction.OpenContainingFolder, "Open Containing Folder"),
        new(FileAction.OpenInExternalEditor, "Open in External Editor"),
        new(FileAction.OpenInDefaultEditor, "Open in Default Editor"),
        new(FileAction.CopyRelativePath, "Copy Relative path"),
        new(FileAction.CopyAbsolutePath, "Copy Full path")
    ];

    /// <summary>
    /// Represents a command to browse for an external editor. It allows users to select an external editing
    /// application.
    /// </summary>
    public ICommand BrowseForExternalEditorCommand { get; }

    /// <summary>
    /// Indicates whether a feature is enabled or not. It provides a getter and a setter to access and modify the
    /// enabled state.
    /// </summary>
    public bool IsEnabled
    {
        get => this._isEnabled;
        set => this.SetProperty(ref this._isEnabled, value);
    }

    /// <summary>
    /// Gets or sets a value indicating where the bar should be displayed.
    /// </summary>
    public BarPosition BarPosition
    {
        get => this._barPosition;
        set => this.SetProperty(ref this._barPosition, value);
    }

    /// <summary>
    /// Gets or sets the display style.
    /// </summary>
    public DisplayStyle DisplayStyle
    {
        get => this._displayStyle;
        set => this.SetProperty(ref this._displayStyle, value);
    }

    /// <summary>
    /// Gets or sets the visual style.
    /// </summary>
    public VisualStyle VisualStyle
    {
        get => this._visualStyle;
        set => this.SetProperty(ref this._visualStyle, value);
    }

    /// <summary>
    /// Gets or sets the path style.
    /// </summary>
    public FileLabel PathStyle
    {
        get => this._pathStyle;
        set => this.SetProperty(ref this._pathStyle, value);
    }

    /// <summary>
    /// Gets or sets a value indicating whether to display in auxiliary documents.
    /// </summary>
    public bool DisplayInAuxiliaryDocuments
    {
        get => this._displayInAuxiliaryDocuments;
        set => this.SetProperty(ref this._displayInAuxiliaryDocuments, value);
    }

    /// <summary>
    /// Gets or sets a value indicating whether to display in non-editable documents.
    /// </summary>
    public bool DisplayInNonEditableDocuments
    {
        get => this._displayInNonEditableDocuments;
        set => this.SetProperty(ref this._displayInNonEditableDocuments, value);
    }

    /// <summary>
    /// Gets or sets a value indicating whether to display in diff views.
    /// </summary>
    public bool DisplayInDiffViews
    {
        get => this._displayInDiffViews;
        set => this.SetProperty(ref this._displayInDiffViews, value);
    }

    /// <summary>
    /// Gets or sets a value indicating whether to display in blame views.
    /// </summary>
    public bool DisplayInBlame
    {
        get => this._displayInBlame;
        set => this.SetProperty(ref this._displayInBlame, value);
    }

    /// <summary>
    /// Gets or sets a value indicating whether to display in temporary files.
    /// </summary>
    public bool DisplayInTemp
    {
        get => this._displayInTemp;
        set => this.SetProperty(ref this._displayInTemp, value);
    }

    /// <summary>
    /// Gets or sets the double-click action.
    /// </summary>
    public FileAction DoubleClickActionOnFileLabel
    {
        get => this._doubleClickActionOnFileLabel;
        set => this.SetProperty(ref this._doubleClickActionOnFileLabel, value);
    }

    /// <summary>
    /// Gets or sets the double-click action with Ctrl key.
    /// </summary>
    public FileAction DoubleClickCtrlActionOnFileLabel
    {
        get => this._doubleClickCtrlActionOnFileLabel;
        set => this.SetProperty(ref this._doubleClickCtrlActionOnFileLabel, value);
    }

    /// <summary>
    /// Gets the solution root segment options.
    /// </summary>
    public EditorSegmentOptionsViewModel SolutionRootSegment { get; } = new();

    /// <summary>
    /// Gets the non-solution root segment options.
    /// </summary>
    public EditorSegmentOptionsViewModel NonSolutionRootSegment { get; } = new();

    /// <summary>
    /// Gets the solution folder segment options.
    /// </summary>
    public EditorSegmentOptionsViewModel SolutionFolderSegment { get; } = new();

    /// <summary>
    /// Gets the project name segment options.
    /// </summary>
    public EditorSegmentOptionsViewModel ProjectNameSegment { get; } = new();

    /// <summary>
    /// Gets the project folder segments options.
    /// </summary>
    public EditorSegmentOptionsViewModel ProjectFolderSegments { get; } = new();

    /// <summary>
    /// Gets the parent folder segment options.
    /// </summary>
    public EditorSegmentOptionsViewModel ParentFolderSegment { get; } = new();

    /// <summary>
    /// Gets the file segment options.
    /// </summary>
    public EditorSegmentOptionsViewModel FileSegment { get; } = new();

    /// <summary>
    /// Gets the code structure segment options.
    /// </summary>
    public EditorSegmentOptionsViewModel CodeStructureSegment { get; } = new();

    /// <summary>
    /// Gets or sets the external editor path.
    /// </summary>
    public string? ExternalEditorPath
    {
        get => this._externalEditorPath;
        set => this.SetProperty(ref this._externalEditorPath, value);
    }

    /// <summary>
    /// Gets or sets the external editor arguments.
    /// </summary>
    public string? ExternalEditorArguments
    {
        get => this._externalEditorArguments;
        set => this.SetProperty(ref this._externalEditorArguments, value);
    }

    /// <summary>
    /// Gets or sets a value indicating whether debug mode is enabled.
    /// </summary>
    public bool IsDebugModeEnabled
    {
        get => this._isDebugModeEnabled;
        set => this.SetProperty(ref this._isDebugModeEnabled, value);
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="OptionsPageViewModel" /> class.
    /// </summary>
    public OptionsPageViewModel()
    {
        this.BrowseForExternalEditorCommand = new DispatchedDelegateCommand(this.ExecuteBrowseForExternalEditorCommand);
    }

    /// <summary>
    /// Executes the command to browse for an external editor.
    /// </summary>
    /// <param name="parameter">The command parameter.</param>
    private void ExecuteBrowseForExternalEditorCommand(object parameter)
    {
        var dlg = new OpenFileDialog { Filter = "Executables (*.exe)|*.exe|All Files|*.*" };
        if (dlg.ShowDialog() == true)
        {
            this.ExternalEditorPath = dlg.FileName;
        }
    }

    /// <summary>
    /// Loads the settings from the specified model.
    /// </summary>
    /// <param name="model">The model containing the settings.</param>
    public void Load(GeneralOptionsModel model)
    {
        try
        {
            this.IsEnabled = model.Enabled;
            this.BarPosition = model.BarPosition;
            this.DisplayStyle = model.DisplayStyle;
            this.VisualStyle = model.VisualStyle;
            this.PathStyle = model.FileLabelStyle;

            this.DisplayInAuxiliaryDocuments = model.DisplayInAuxiliaryDocuments;
            this.DisplayInNonEditableDocuments = model.DisplayInNonEditableDocuments;
            this.DisplayInDiffViews = model.DisplayInDiffViews;
            this.DisplayInBlame = model.DisplayInBlame;
            this.DisplayInTemp = model.DisplayInTempFiles;

            this.DoubleClickActionOnFileLabel = model.FileAction;
            this.DoubleClickCtrlActionOnFileLabel = model.AlternateFileAction;

            this.SolutionRootSegment.IsVisible = model.ShowSolutionRoot;
            this.SolutionRootSegment.ForegroundColor = model.SolutionForeground.ToMediaColor();
            this.SolutionRootSegment.BackgroundColor = model.SolutionBackground.ToMediaColor();
            this.NonSolutionRootSegment.ForegroundColor = model.NonSolutionRootForeground.ToMediaColor();
            this.NonSolutionRootSegment.BackgroundColor = model.NonSolutionRootBackground.ToMediaColor();

            this.SolutionFolderSegment.IsVisible = model.ShowSolutionFolders;
            this.SolutionFolderSegment.ForegroundColor = model.SolutionFolderForeground.ToMediaColor();
            this.SolutionFolderSegment.BackgroundColor = model.SolutionFolderBackground.ToMediaColor();

            this.ProjectNameSegment.IsVisible = model.ShowProject;
            this.ProjectNameSegment.ForegroundColor = model.ProjectForeground.ToMediaColor();
            this.ProjectNameSegment.BackgroundColor = model.ProjectBackground.ToMediaColor();

            this.ProjectFolderSegments.IsVisible = model.ShowProjectFolders;
            this.ProjectFolderSegments.ForegroundColor = model.ProjectFoldersForeground.ToMediaColor();
            this.ProjectFolderSegments.BackgroundColor = model.ProjectFoldersBackground.ToMediaColor();

            this.ParentFolderSegment.IsVisible = model.ShowParentFolder;
            this.ParentFolderSegment.ForegroundColor = model.ParentFolderForeground.ToMediaColor();
            this.ParentFolderSegment.BackgroundColor = model.ParentFolderBackground.ToMediaColor();

            this.FileSegment.IsVisible = model.ShowFileNameBreadcrumb;
            this.FileSegment.ForegroundColor = model.FileBreadcrumbForeground.ToMediaColor();
            this.FileSegment.BackgroundColor = model.FileBreadcrumbBackground.ToMediaColor();

            this.CodeStructureSegment.IsVisible = model.ShowCodeStructureBreadcrumbs;
            this.CodeStructureSegment.ForegroundColor = model.StructureBreadcrumbForeground.ToMediaColor();
            this.CodeStructureSegment.BackgroundColor = model.StructureBreadcrumbBackground.ToMediaColor();

            this.ExternalEditorPath = model.ExternalEditorCommand ?? "";
            this.ExternalEditorArguments = model.ExternalEditorCommandArguments ?? "";

            this.IsDebugModeEnabled = model.DebugMode;
        }
        catch (Exception ex)
        {
            ex.Log();
        }
    }

    /// <summary>
    /// Saves the settings to the specified model.
    /// </summary>
    /// <param name="model">The model to save the settings to.</param>
    public void Save(GeneralOptionsModel model)
    {
        try
        {
            model.Enabled = this.IsEnabled;
            model.BarPosition = this.BarPosition;
            model.DisplayStyle = this.DisplayStyle;
            model.VisualStyle = this.VisualStyle;
            model.FileLabelStyle = this.PathStyle;

            model.DisplayInAuxiliaryDocuments = this.DisplayInAuxiliaryDocuments;
            model.DisplayInNonEditableDocuments = this.DisplayInNonEditableDocuments;
            model.DisplayInDiffViews = this.DisplayInDiffViews;
            model.DisplayInBlame = this.DisplayInBlame;
            model.DisplayInTempFiles = this.DisplayInTemp;

            model.FileAction = this.DoubleClickActionOnFileLabel;
            model.AlternateFileAction = this.DoubleClickCtrlActionOnFileLabel;

            model.ShowSolutionRoot = this.SolutionRootSegment.IsVisible;
            model.SolutionForeground = this.SolutionRootSegment.ForegroundColor.ToDrawingColor();
            model.SolutionBackground = this.SolutionRootSegment.BackgroundColor.ToDrawingColor();
            model.NonSolutionRootForeground = this.NonSolutionRootSegment.ForegroundColor.ToDrawingColor();
            model.NonSolutionRootBackground = this.NonSolutionRootSegment.BackgroundColor.ToDrawingColor();

            model.ShowSolutionFolders = this.SolutionFolderSegment.IsVisible;
            model.SolutionFolderForeground = this.SolutionFolderSegment.ForegroundColor.ToDrawingColor();
            model.SolutionFolderBackground = this.SolutionFolderSegment.BackgroundColor.ToDrawingColor();

            model.ShowProject = this.ProjectNameSegment.IsVisible;
            model.ProjectForeground = this.ProjectNameSegment.ForegroundColor.ToDrawingColor();
            model.ProjectBackground = this.ProjectNameSegment.BackgroundColor.ToDrawingColor();

            model.ShowProjectFolders = this.ProjectFolderSegments.IsVisible;
            model.ProjectFoldersForeground = this.ProjectFolderSegments.ForegroundColor.ToDrawingColor();
            model.ProjectFoldersBackground = this.ProjectFolderSegments.BackgroundColor.ToDrawingColor();

            model.ShowParentFolder = this.ParentFolderSegment.IsVisible;
            model.ParentFolderForeground = this.ParentFolderSegment.ForegroundColor.ToDrawingColor();
            model.ParentFolderBackground = this.ParentFolderSegment.BackgroundColor.ToDrawingColor();

            model.ShowFileNameBreadcrumb = this.FileSegment.IsVisible;
            model.FileBreadcrumbForeground = this.FileSegment.ForegroundColor.ToDrawingColor();
            model.FileBreadcrumbBackground = this.FileSegment.BackgroundColor.ToDrawingColor();

            model.ShowCodeStructureBreadcrumbs = this.CodeStructureSegment.IsVisible;
            model.StructureBreadcrumbForeground = this.CodeStructureSegment.ForegroundColor.ToDrawingColor();
            model.StructureBreadcrumbBackground = this.CodeStructureSegment.BackgroundColor.ToDrawingColor();

            model.ExternalEditorCommand = (this.ExternalEditorPath ?? "").Trim();
            model.ExternalEditorCommandArguments = (this.ExternalEditorArguments ?? "").Trim();

            model.DebugMode = this.IsDebugModeEnabled;

            model.Save();
        }
        catch (Exception ex)
        {
            ex.Log();
        }
    }
}