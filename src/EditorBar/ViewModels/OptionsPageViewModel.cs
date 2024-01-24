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

    public void Load(GeneralOptionsModel model)
    {
        try
        {
            this.IsEnabled = model.Enabled;
            this.BarPosition = model.BarPosition;
            this.DisplayStyle = model.DisplayStyle;
            this.ShowRelativePath = model.ShowPathRelativeToSolutionRoot;

            this.DoubleClickAction = model.FileAction;
            this.DoubleClickCtrlAction = model.AlternateFileAction;

            this.SolutionRootSegment.IsVisible = model.ShowSolutionRoot;
            this.SolutionRootSegment.ForegroundColor = model.SolutionForeground.ToMediaColor();
            this.SolutionRootSegment.BackgroundColor = model.SolutionBackground.ToMediaColor();

            this.SolutionFolderSegment.IsVisible = model.ShowSolutionFolders;
            this.SolutionFolderSegment.ForegroundColor = model.SolutionFolderForeground.ToMediaColor();
            this.SolutionFolderSegment.BackgroundColor = model.SolutionFolderBackground.ToMediaColor();

            this.ProjectNameSegment.IsVisible = true;
            this.ProjectNameSegment.ForegroundColor = model.ProjectForeground.ToMediaColor();
            this.ProjectNameSegment.BackgroundColor = model.ProjectBackground.ToMediaColor();

            this.ExternalEditorPath = model.ExternalEditorCommand ?? "";
            this.ExternalEditorArguments = model.ExternalEditorCommandArguments ?? "";
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
            model.ShowPathRelativeToSolutionRoot = this.ShowRelativePath;

            model.FileAction = this.DoubleClickAction;
            model.AlternateFileAction = this.DoubleClickCtrlAction;

            model.ShowSolutionRoot = this.SolutionRootSegment.IsVisible;
            model.SolutionForeground = this.SolutionRootSegment.ForegroundColor.ToDrawingColor();
            model.SolutionBackground = this.SolutionRootSegment.BackgroundColor.ToDrawingColor();

            model.ShowSolutionFolders = this.SolutionFolderSegment.IsVisible;
            model.SolutionFolderForeground = this.SolutionFolderSegment.ForegroundColor.ToDrawingColor();
            model.SolutionFolderBackground = this.SolutionFolderSegment.BackgroundColor.ToDrawingColor();

            model.ProjectForeground = this.ProjectNameSegment.ForegroundColor.ToDrawingColor();
            model.ProjectBackground = this.ProjectNameSegment.BackgroundColor.ToDrawingColor();

            model.ExternalEditorCommand = (this.ExternalEditorPath ?? "").Trim();
            model.ExternalEditorCommandArguments = (this.ExternalEditorArguments ?? "").Trim();

            model.Save();
        }
        catch (Exception ex)
        {
            ex.Log();
        }
    }
}