// ------------------------------------------------------------
// 
// Copyright (c) Jiří Polášek. All rights reserved.
// 
// ------------------------------------------------------------

using System.Collections.ObjectModel;
using System.Windows.Input;
using JPSoftworks.EditorBar.Fx;
using JPSoftworks.EditorBar.Helpers;
using JPSoftworks.EditorBar.Options;
using Microsoft.VisualStudio.PlatformUI;
using Microsoft.Win32;

namespace JPSoftworks.EditorBar.ViewModels;

public class OptionsPageViewModel : ViewModel
{
    private bool _isEnabled;
    private BarPosition _barPosition;
    private DisplayStyle _displayStyle;
    private FileAction _doubleClickAction;
    private FileAction _doubleClickCtrlAction;
    private string? _externalEditorPath;
    private string? _externalEditorArguments;
    private bool _showRelativePath;
    private bool _displayInAuxiliaryDocuments;
    private bool _displayInNonEditableDocuments;
    private bool _displayInDiffViews;
    private bool _isDebugModeEnabled;
    private bool _displayInBlame;
    private bool _displayInTemp;
    private VisualStyle _visualStyle;

    public ObservableCollection<EnumViewModel<BarPosition>> BarPositions { get; } =
    [
        new EnumViewModel<BarPosition>(BarPosition.Top, "Top"),
        new EnumViewModel<BarPosition>(BarPosition.Bottom, "Bottom")
    ];

    public ObservableCollection<EnumViewModel<DisplayStyle>> DisplayStyles { get; } =
    [
        new EnumViewModel<DisplayStyle>(DisplayStyle.Normal, "Normal"),
        new EnumViewModel<DisplayStyle>(DisplayStyle.Compact, "Compact")
    ];

    public ObservableCollection<EnumViewModel<VisualStyle>> VisualStyles { get; } =
    [
        new EnumViewModel<VisualStyle>(VisualStyle.FullRowCommandBar, "Command Bar"),
        new EnumViewModel<VisualStyle>(VisualStyle.FullRowTransparent, "Transparent"),
        new EnumViewModel<VisualStyle>(VisualStyle.FullRowToolWindow, "Tool Window")
    ];

    public ObservableCollection<EnumViewModel<bool>> PathStyles { get; } =
    [
        new EnumViewModel<bool>(true, "Relative Path"),
        new EnumViewModel<bool>(false, "Full Path")
    ];

    public ObservableCollection<EnumViewModel<FileAction>> DoubleClickActions { get; } =
    [
        new EnumViewModel<FileAction>(FileAction.None, "Do nothing"),
        new EnumViewModel<FileAction>(FileAction.OpenContainingFolder, "Open Containing Folder"),
        new EnumViewModel<FileAction>(FileAction.OpenInExternalEditor, "Open in External Editor"),
        new EnumViewModel<FileAction>(FileAction.OpenInDefaultEditor, "Open in Default Editor"),
        new EnumViewModel<FileAction>(FileAction.CopyRelativePath,"Copy Relative path"),
        new EnumViewModel<FileAction>(FileAction.CopyAbsolutePath, "Copy Full path")
    ];

    public ICommand BrowseForExternalEditorCommand { get; }

    public OptionsPageViewModel()
    {
        this.BrowseForExternalEditorCommand = new DispatchedDelegateCommand(this.ExecuteBrowseForExternalEditorCommand);
    }

    private void ExecuteBrowseForExternalEditorCommand(object parameter)
    {
        var dlg = new OpenFileDialog { Filter = "Executables (*.exe)|*.exe|All Files|*.*" };
        if (dlg.ShowDialog() == true)
        {
            this.ExternalEditorPath = dlg.FileName;
        }
    }

    public bool IsEnabled
    {
        get => this._isEnabled;
        set => this.SetField(ref this._isEnabled, value);
    }

    public BarPosition BarPosition
    {
        get => this._barPosition;
        set => this.SetField(ref this._barPosition, value);
    }

    public DisplayStyle DisplayStyle
    {
        get => this._displayStyle;
        set => this.SetField(ref this._displayStyle, value);
    }

    public VisualStyle VisualStyle
    {
        get => this._visualStyle;
        set => this.SetField(ref this._visualStyle, value);
    }

    public bool DisplayInAuxiliaryDocuments
    {
        get => this._displayInAuxiliaryDocuments;
        set => this.SetField(ref this._displayInAuxiliaryDocuments, value);
    }

    public bool DisplayInNonEditableDocuments
    {
        get => this._displayInNonEditableDocuments;
        set => this.SetField(ref this._displayInNonEditableDocuments, value);
    }

    public bool DisplayInDiffViews
    {
        get => this._displayInDiffViews;
        set => this.SetField(ref this._displayInDiffViews, value);
    }

    public bool DisplayInBlame
    {
        get => this._displayInBlame;
        set => this.SetField(ref this._displayInBlame, value);
    }

    public bool DisplayInTemp
    {
        get => this._displayInTemp;
        set => this.SetField(ref this._displayInTemp, value);
    }

    public FileAction DoubleClickAction
    {
        get => this._doubleClickAction;
        set => this.SetField(ref this._doubleClickAction, value);
    }

    public FileAction DoubleClickCtrlAction
    {
        get => this._doubleClickCtrlAction;
        set => this.SetField(ref this._doubleClickCtrlAction, value);
    }

    public bool ShowRelativePath
    {
        get => this._showRelativePath;
        set => this.SetField(ref this._showRelativePath, value);
    }

    public EditorSegmentOptionsViewModel SolutionRootSegment { get; } = new();

    public EditorSegmentOptionsViewModel SolutionFolderSegment { get; } = new();

    public EditorSegmentOptionsViewModel ProjectNameSegment { get; } = new();

    public EditorSegmentOptionsViewModel ParentFolderSegment { get; set; } = new();

    public string? ExternalEditorPath
    {
        get => this._externalEditorPath;
        set => this.SetField(ref this._externalEditorPath, value);
    }

    public string? ExternalEditorArguments
    {
        get => this._externalEditorArguments;
        set => this.SetField(ref this._externalEditorArguments, value);
    }

    public bool IsDebugModeEnabled
    {
        get => this._isDebugModeEnabled;
        set => this.SetField(ref this._isDebugModeEnabled, value);
    }

    public void Load(GeneralOptionsModel model)
    {
        try
        {
            this.IsEnabled = model.Enabled;
            this.BarPosition = model.BarPosition;
            this.DisplayStyle = model.DisplayStyle;
            this.VisualStyle = model.VisualStyle;
            this.ShowRelativePath = model.ShowPathRelativeToSolutionRoot;

            this.DisplayInAuxiliaryDocuments = model.DisplayInAuxiliaryDocuments;
            this.DisplayInNonEditableDocuments = model.DisplayInNonEditableDocuments;
            this.DisplayInDiffViews = model.DisplayInDiffViews;
            this.DisplayInBlame = model.DisplayInBlame;
            this.DisplayInTemp = model.DisplayInTempFiles;

            this.DoubleClickAction = model.FileAction;
            this.DoubleClickCtrlAction = model.AlternateFileAction;

            this.SolutionRootSegment.IsVisible = model.ShowSolutionRoot;
            this.SolutionRootSegment.ForegroundColor = model.SolutionForeground.ToMediaColor();
            this.SolutionRootSegment.BackgroundColor = model.SolutionBackground.ToMediaColor();

            this.SolutionFolderSegment.IsVisible = model.ShowSolutionFolders;
            this.SolutionFolderSegment.ForegroundColor = model.SolutionFolderForeground.ToMediaColor();
            this.SolutionFolderSegment.BackgroundColor = model.SolutionFolderBackground.ToMediaColor();

            this.ProjectNameSegment.IsVisible = model.ShowProject;
            this.ProjectNameSegment.ForegroundColor = model.ProjectForeground.ToMediaColor();
            this.ProjectNameSegment.BackgroundColor = model.ProjectBackground.ToMediaColor();

            this.ParentFolderSegment.IsVisible = model.ShowParentFolder;
            this.ParentFolderSegment.ForegroundColor = model.ParentFolderForeground.ToMediaColor();
            this.ParentFolderSegment.BackgroundColor = model.ParentFolderBackground.ToMediaColor();

            this.ExternalEditorPath = model.ExternalEditorCommand ?? "";
            this.ExternalEditorArguments = model.ExternalEditorCommandArguments ?? "";

            this.IsDebugModeEnabled = model.DebugMode;
        }
        catch (Exception ex)
        {
            ex.Log();
        }
    }

    public void Save(GeneralOptionsModel model)
    {
        try
        {
            model.Enabled = this.IsEnabled;
            model.BarPosition = this.BarPosition;
            model.DisplayStyle = this.DisplayStyle;
            model.VisualStyle = this.VisualStyle;
            model.ShowPathRelativeToSolutionRoot = this.ShowRelativePath;

            model.DisplayInAuxiliaryDocuments = this.DisplayInAuxiliaryDocuments;
            model.DisplayInNonEditableDocuments = this.DisplayInNonEditableDocuments;
            model.DisplayInDiffViews = this.DisplayInDiffViews;
            model.DisplayInBlame = this.DisplayInBlame;
            model.DisplayInTempFiles = this.DisplayInTemp;

            model.FileAction = this.DoubleClickAction;
            model.AlternateFileAction = this.DoubleClickCtrlAction;

            model.ShowSolutionRoot = this.SolutionRootSegment.IsVisible;
            model.SolutionForeground = this.SolutionRootSegment.ForegroundColor.ToDrawingColor();
            model.SolutionBackground = this.SolutionRootSegment.BackgroundColor.ToDrawingColor();

            model.ShowSolutionFolders = this.SolutionFolderSegment.IsVisible;
            model.SolutionFolderForeground = this.SolutionFolderSegment.ForegroundColor.ToDrawingColor();
            model.SolutionFolderBackground = this.SolutionFolderSegment.BackgroundColor.ToDrawingColor();

            model.ShowProject = this.ProjectNameSegment.IsVisible;
            model.ProjectForeground = this.ProjectNameSegment.ForegroundColor.ToDrawingColor();
            model.ProjectBackground = this.ProjectNameSegment.BackgroundColor.ToDrawingColor();

            model.ShowParentFolder = this.ParentFolderSegment.IsVisible;
            model.ParentFolderForeground = this.ParentFolderSegment.ForegroundColor.ToDrawingColor();
            model.ParentFolderBackground = this.ParentFolderSegment.BackgroundColor.ToDrawingColor();

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