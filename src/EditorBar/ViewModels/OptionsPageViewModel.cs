// ------------------------------------------------------------
// Copyright (c) Jiří Polášek. All rights reserved.
// ------------------------------------------------------------

#nullable enable

using System.Collections.ObjectModel;
using System.Windows.Input;
using System.Windows.Media;
using JPSoftworks.EditorBar.Helpers;
using JPSoftworks.EditorBar.Helpers.Presentation;
using JPSoftworks.EditorBar.Helpers.VisualStudio;
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
    private bool _displayLocateInSolutionExplorerButton;
    private bool _displayMemberListFilterBoxWhenEmpty;
    private MemberTreeSearchResultView _memberTreeSearchResultViewDefault;
    private bool _displayOpenContainingFolderButton;
    private bool _displayOpenDefaultEditorButton;
    private bool _displayOpenExternalEditorButton;
    private bool _displayOpenTerminalButton;
    private DisplayStyle _displayStyle;
    private FileAction _doubleClickActionOnFileLabel;
    private FileAction _doubleClickCtrlActionOnFileLabel;
    private Color _bulkBreadcrumbBackgroundColor;
    private Color _bulkBreadcrumbForegroundColor;
    private string? _externalEditorArguments;
    private string? _externalEditorPath;
    private bool _isDebugModeEnabled;
    private bool _isEnabled;
    private bool _isCustomTerminalSelected;
    private bool _isTerminalPresetLocked;
    private bool _isEditingDarkColors;
    private bool _isEditingLightColors;
    private FileLabel _pathStyle;
    private string _copyFromOtherColorModeButtonText = string.Empty;
    private string _currentEditorAppearanceText = string.Empty;
    private EditorColorMode _selectedColorMode;
    private TerminalProfile _selectedTerminalProfile;
    private string? _terminalArguments;
    private string _terminalArgumentsDisplay = string.Empty;
    private string _terminalPresetHint = string.Empty;
    private string? _terminalPath;
    private string _terminalPathDisplay = string.Empty;
    private VisualStyle _visualStyle;

    /// <summary>
    /// Gets the available options for the bar position.
    /// </summary>
    public ObservableCollection<EnumViewModel<BarPosition>> BarPositions { get; } =
    [
        new(BarPosition.Top, "Top"),
        new(BarPosition.Bottom, "Bottom"),
        new(BarPosition.BottomControl, "Bottom (next to Zoom)")
    ];

    /// <summary>
    /// Gets the available options for the display style.
    /// </summary>
    public ObservableCollection<EnumViewModel<DisplayStyle>> DisplayStyles { get; } =
    [
        new(DisplayStyle.Normal, "Normal"),
        new(DisplayStyle.Compact, "Compact")
    ];

    /// <summary>
    /// Gets the available options for the visual style.
    /// </summary>
    public ObservableCollection<EnumViewModel<VisualStyle>> VisualStyles { get; } =
    [
        new(VisualStyle.FullRowTransparent, "Transparent"),
        new(VisualStyle.FullRowCommandBar, "Command Bar"),
        new(VisualStyle.FullRowToolWindow, "Tool Window")
    ];

    /// <summary>
    /// Gets the available options for the path style.
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
    /// Gets the available options for the double-click action.
    /// </summary>
    public ObservableCollection<EnumViewModel<FileAction>> DoubleClickActions { get; } =
    [
        new(FileAction.None, "Do nothing"),
        new(FileAction.OpenContainingFolder, "Open Containing Folder"),
        new(FileAction.OpenInTerminal, "Open in Terminal"),
        new(FileAction.OpenInExternalEditor, "Open in External Editor"),
        new(FileAction.OpenInDefaultEditor, "Open in Default Editor"),
        new(FileAction.CopyRelativePath, "Copy Relative path"),
        new(FileAction.CopyAbsolutePath, "Copy Full path")
    ];

    /// <summary>
    /// Gets the available default views for filtered member tree results.
    /// </summary>
    public ObservableCollection<EnumViewModel<MemberTreeSearchResultView>> MemberTreeSearchResultViews { get; } =
    [
        new(MemberTreeSearchResultView.Tree, "Tree"),
        new(MemberTreeSearchResultView.List, "List"),
        new(MemberTreeSearchResultView.RememberLastUsed, "Remember last used")
    ];

    /// <summary>
    /// Gets the available terminal presets.
    /// </summary>
    public ObservableCollection<EnumViewModel<TerminalProfile>> TerminalProfiles { get; } =
    [
        new(TerminalProfile.WindowsTerminal, "Windows Terminal"),
        new(TerminalProfile.CommandPrompt, "Command Prompt"),
        new(TerminalProfile.WindowsPowerShell, "Windows PowerShell"),
        new(TerminalProfile.PowerShell, "PowerShell (pwsh)"),
        new(TerminalProfile.DeveloperPowerShell, "Developer PowerShell"),
        new(TerminalProfile.Custom, "Custom")
    ];

    /// <summary>
    /// Gets the command to browse for an external editor.
    /// </summary>
    public ICommand BrowseForExternalEditorCommand { get; }

    /// <summary>
    /// Gets the command to browse for a terminal executable.
    /// </summary>
    public ICommand BrowseForTerminalCommand { get; }

    /// <summary>
    /// Gets the command to copy colors from the non-selected color set.
    /// </summary>
    public ICommand CopyFromOtherColorModeCommand { get; }

    /// <summary>
    /// Gets the command to reset the selected color set to defaults.
    /// </summary>
    public ICommand ResetCurrentColorModeCommand { get; }

    /// <summary>
    /// Gets or sets a value indicating whether a feature is enabled.
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
    /// Gets or sets a value indicating whether the Locate in Solution Explorer toolbar button is visible.
    /// </summary>
    public bool DisplayLocateInSolutionExplorerButton
    {
        get => this._displayLocateInSolutionExplorerButton;
        set => this.SetProperty(ref this._displayLocateInSolutionExplorerButton, value);
    }

    /// <summary>
    /// Gets or sets a value indicating whether the member list filter box stays visible when empty.
    /// </summary>
    public bool DisplayMemberListFilterBoxWhenEmpty
    {
        get => this._displayMemberListFilterBoxWhenEmpty;
        set => this.SetProperty(ref this._displayMemberListFilterBoxWhenEmpty, value);
    }

    /// <summary>
    /// Gets or sets the default presentation for filtered member tree results.
    /// </summary>
    public MemberTreeSearchResultView MemberTreeSearchResultViewDefault
    {
        get => this._memberTreeSearchResultViewDefault;
        set => this.SetProperty(ref this._memberTreeSearchResultViewDefault, value);
    }

    /// <summary>
    /// Gets or sets a value indicating whether the default editor toolbar button is visible.
    /// </summary>
    public bool DisplayOpenDefaultEditorButton
    {
        get => this._displayOpenDefaultEditorButton;
        set => this.SetProperty(ref this._displayOpenDefaultEditorButton, value);
    }

    /// <summary>
    /// Gets or sets a value indicating whether the external editor toolbar button is visible.
    /// </summary>
    public bool DisplayOpenExternalEditorButton
    {
        get => this._displayOpenExternalEditorButton;
        set => this.SetProperty(ref this._displayOpenExternalEditorButton, value);
    }

    /// <summary>
    /// Gets or sets a value indicating whether the containing folder toolbar button is visible.
    /// </summary>
    public bool DisplayOpenContainingFolderButton
    {
        get => this._displayOpenContainingFolderButton;
        set => this.SetProperty(ref this._displayOpenContainingFolderButton, value);
    }

    /// <summary>
    /// Gets or sets a value indicating whether the terminal toolbar button is visible.
    /// </summary>
    public bool DisplayOpenTerminalButton
    {
        get => this._displayOpenTerminalButton;
        set => this.SetProperty(ref this._displayOpenTerminalButton, value);
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
    /// Gets or sets the selected terminal preset.
    /// </summary>
    public TerminalProfile SelectedTerminalProfile
    {
        get => this._selectedTerminalProfile;
        set
        {
            if (this.SetProperty(ref this._selectedTerminalProfile, value))
            {
                this.NotifyTerminalPresentationChanged();
            }
        }
    }

    /// <summary>
    /// Gets or sets the terminal path.
    /// </summary>
    public string? TerminalPath
    {
        get => this._terminalPath;
        set
        {
            if (this.SetProperty(ref this._terminalPath, value))
            {
                this.UpdateTerminalPresentation();
            }
        }
    }

    /// <summary>
    /// Gets or sets the terminal arguments.
    /// </summary>
    public string? TerminalArguments
    {
        get => this._terminalArguments;
        set
        {
            if (this.SetProperty(ref this._terminalArguments, value))
            {
                this.UpdateTerminalPresentation();
            }
        }
    }

    /// <summary>
    /// Gets a value indicating whether the custom terminal fields are editable.
    /// </summary>
    public bool IsCustomTerminalSelected
    {
        get => this._isCustomTerminalSelected;
        private set => this.SetProperty(ref this._isCustomTerminalSelected, value);
    }

    /// <summary>
    /// Gets a value indicating whether the terminal command fields are read-only.
    /// </summary>
    public bool IsTerminalPresetLocked
    {
        get => this._isTerminalPresetLocked;
        private set => this.SetProperty(ref this._isTerminalPresetLocked, value);
    }

    /// <summary>
    /// Gets or sets the displayed terminal command text.
    /// </summary>
    public string TerminalPathDisplay
    {
        get => this._terminalPathDisplay;
        set
        {
            if (this.IsCustomTerminalSelected && this.SetProperty(ref this._terminalPathDisplay, value))
            {
                this.TerminalPath = value;
            }
        }
    }

    /// <summary>
    /// Gets or sets the displayed terminal arguments text.
    /// </summary>
    public string TerminalArgumentsDisplay
    {
        get => this._terminalArgumentsDisplay;
        set
        {
            if (this.IsCustomTerminalSelected && this.SetProperty(ref this._terminalArgumentsDisplay, value))
            {
                this.TerminalArguments = value;
            }
        }
    }

    /// <summary>
    /// Gets the terminal preset hint text.
    /// </summary>
    public string TerminalPresetHint
    {
        get => this._terminalPresetHint;
        private set => this.SetProperty(ref this._terminalPresetHint, value);
    }

    public string CurrentEditorAppearanceText
    {
        get => this._currentEditorAppearanceText;
        private set => this.SetProperty(ref this._currentEditorAppearanceText, value);
    }

    /// <summary>
    /// Gets or sets the foreground color that is applied to all breadcrumb segments in the selected color set.
    /// </summary>
    public Color BulkBreadcrumbForegroundColor
    {
        get => this._bulkBreadcrumbForegroundColor;
        set => this.ApplyForegroundColorToAllSegments(value);
    }

    /// <summary>
    /// Gets or sets the background color that is applied to all breadcrumb segments in the selected color set.
    /// </summary>
    public Color BulkBreadcrumbBackgroundColor
    {
        get => this._bulkBreadcrumbBackgroundColor;
        set => this.ApplyBackgroundColorToAllSegments(value);
    }

    /// <summary>
    /// Gets the text for the copy-colors action.
    /// </summary>
    public string CopyFromOtherColorModeButtonText
    {
        get => this._copyFromOtherColorModeButtonText;
        private set => this.SetProperty(ref this._copyFromOtherColorModeButtonText, value);
    }

    public EditorColorMode SelectedColorMode
    {
        get => this._selectedColorMode;
        set
        {
            if (this.SetProperty(ref this._selectedColorMode, value))
            {
                this.UpdateSelectedColorModePresentation(value);
            }
        }
    }

    /// <summary>
    /// Gets or sets a value indicating whether the light appearance color set is being edited.
    /// </summary>
    public bool IsEditingLightColors
    {
        get => this._isEditingLightColors;
        set
        {
            if (this.SetProperty(ref this._isEditingLightColors, value))
            {
                if (value)
                {
                    this.SelectedColorMode = EditorColorMode.Light;
                }
            }
        }
    }

    /// <summary>
    /// Gets or sets a value indicating whether the dark appearance color set is being edited.
    /// </summary>
    public bool IsEditingDarkColors
    {
        get => this._isEditingDarkColors;
        set
        {
            if (this.SetProperty(ref this._isEditingDarkColors, value))
            {
                if (value)
                {
                    this.SelectedColorMode = EditorColorMode.Dark;
                }
            }
        }
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
        this.BrowseForTerminalCommand = new DispatchedDelegateCommand(this.ExecuteBrowseForTerminalCommand);
        this.CopyFromOtherColorModeCommand = new DispatchedDelegateCommand(_ => this.CopyFromOtherColorMode());
        this.ResetCurrentColorModeCommand = new DispatchedDelegateCommand(_ => this.ResetCurrentColorModeToDefaults());
        this._selectedColorMode = EditorAppearanceHelper.GetCurrentMode();
        this.UpdateSelectedColorModePresentation(this._selectedColorMode);
        this.UpdateTerminalPresentation();
    }

    /// <summary>
    /// Executes the command to browse for an external editor.
    /// </summary>
    /// <param name="parameter">The command parameter.</param>
    private void ExecuteBrowseForExternalEditorCommand(object parameter)
    {
        this.BrowseForExecutable(path => this.ExternalEditorPath = path);
    }

    /// <summary>
    /// Executes the command to browse for a terminal executable.
    /// </summary>
    /// <param name="parameter">The command parameter.</param>
    private void ExecuteBrowseForTerminalCommand(object parameter)
    {
        this.BrowseForExecutable(path => this.TerminalPath = path);
    }

    private void BrowseForExecutable(Action<string> setPath)
    {
        var dlg = new OpenFileDialog { Filter = "Executables (*.exe)|*.exe|All Files|*.*" };
        if (dlg.ShowDialog() == true)
        {
            setPath(dlg.FileName);
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
            this.DisplayMemberListFilterBoxWhenEmpty = model.ShowMemberListFilterBoxWhenEmpty;
            this.MemberTreeSearchResultViewDefault = model.MemberTreeSearchResultViewDefault;
            this.DisplayLocateInSolutionExplorerButton = model.ShowLocateInSolutionExplorerButton;
            this.DisplayOpenDefaultEditorButton = model.ShowOpenDefaultEditorButton;
            this.DisplayOpenExternalEditorButton = model.ShowOpenExternalEditorButton;
            this.DisplayOpenContainingFolderButton = model.ShowOpenContainingFolderButton;
            this.DisplayOpenTerminalButton = model.ShowOpenTerminalButton;

            this.DoubleClickActionOnFileLabel = model.FileAction;
            this.DoubleClickCtrlActionOnFileLabel = model.AlternateFileAction;

            this.SolutionRootSegment.IsVisible = model.ShowSolutionRoot;
            this.SolutionRootSegment.DarkForegroundColor = model.SolutionForeground.ToMediaColor();
            this.SolutionRootSegment.DarkBackgroundColor = model.SolutionBackground.ToMediaColor();
            this.SolutionRootSegment.LightForegroundColor = model.LightSolutionForeground.ToMediaColor();
            this.SolutionRootSegment.LightBackgroundColor = model.LightSolutionBackground.ToMediaColor();
            this.NonSolutionRootSegment.DarkForegroundColor = model.NonSolutionRootForeground.ToMediaColor();
            this.NonSolutionRootSegment.DarkBackgroundColor = model.NonSolutionRootBackground.ToMediaColor();
            this.NonSolutionRootSegment.LightForegroundColor = model.LightNonSolutionRootForeground.ToMediaColor();
            this.NonSolutionRootSegment.LightBackgroundColor = model.LightNonSolutionRootBackground.ToMediaColor();

            this.SolutionFolderSegment.IsVisible = model.ShowSolutionFolders;
            this.SolutionFolderSegment.DarkForegroundColor = model.SolutionFolderForeground.ToMediaColor();
            this.SolutionFolderSegment.DarkBackgroundColor = model.SolutionFolderBackground.ToMediaColor();
            this.SolutionFolderSegment.LightForegroundColor = model.LightSolutionFolderForeground.ToMediaColor();
            this.SolutionFolderSegment.LightBackgroundColor = model.LightSolutionFolderBackground.ToMediaColor();

            this.ProjectNameSegment.IsVisible = model.ShowProject;
            this.ProjectNameSegment.DarkForegroundColor = model.ProjectForeground.ToMediaColor();
            this.ProjectNameSegment.DarkBackgroundColor = model.ProjectBackground.ToMediaColor();
            this.ProjectNameSegment.LightForegroundColor = model.LightProjectForeground.ToMediaColor();
            this.ProjectNameSegment.LightBackgroundColor = model.LightProjectBackground.ToMediaColor();

            this.ProjectFolderSegments.IsVisible = model.ShowProjectFolders;
            this.ProjectFolderSegments.DarkForegroundColor = model.ProjectFoldersForeground.ToMediaColor();
            this.ProjectFolderSegments.DarkBackgroundColor = model.ProjectFoldersBackground.ToMediaColor();
            this.ProjectFolderSegments.LightForegroundColor = model.LightProjectFoldersForeground.ToMediaColor();
            this.ProjectFolderSegments.LightBackgroundColor = model.LightProjectFoldersBackground.ToMediaColor();

            this.ParentFolderSegment.IsVisible = model.ShowParentFolder;
            this.ParentFolderSegment.DarkForegroundColor = model.ParentFolderForeground.ToMediaColor();
            this.ParentFolderSegment.DarkBackgroundColor = model.ParentFolderBackground.ToMediaColor();
            this.ParentFolderSegment.LightForegroundColor = model.LightParentFolderForeground.ToMediaColor();
            this.ParentFolderSegment.LightBackgroundColor = model.LightParentFolderBackground.ToMediaColor();

            this.FileSegment.IsVisible = model.ShowFileNameBreadcrumb;
            this.FileSegment.DarkForegroundColor = model.FileBreadcrumbForeground.ToMediaColor();
            this.FileSegment.DarkBackgroundColor = model.FileBreadcrumbBackground.ToMediaColor();
            this.FileSegment.LightForegroundColor = model.LightFileBreadcrumbForeground.ToMediaColor();
            this.FileSegment.LightBackgroundColor = model.LightFileBreadcrumbBackground.ToMediaColor();

            this.CodeStructureSegment.IsVisible = model.ShowCodeStructureBreadcrumbs;
            this.CodeStructureSegment.DarkForegroundColor = model.StructureBreadcrumbForeground.ToMediaColor();
            this.CodeStructureSegment.DarkBackgroundColor = model.StructureBreadcrumbBackground.ToMediaColor();
            this.CodeStructureSegment.LightForegroundColor = model.LightStructureBreadcrumbForeground.ToMediaColor();
            this.CodeStructureSegment.LightBackgroundColor = model.LightStructureBreadcrumbBackground.ToMediaColor();

            this.ExternalEditorPath = model.ExternalEditorCommand ?? string.Empty;
            this.ExternalEditorArguments = model.ExternalEditorCommandArguments ?? string.Empty;
            this.TerminalPath = model.TerminalCommand ?? string.Empty;
            this.TerminalArguments = model.TerminalCommandArguments ?? string.Empty;
            this.SelectedTerminalProfile = Launcher.NormalizeSupportedTerminalProfile(model.TerminalProfile);

            this.IsDebugModeEnabled = model.DebugMode;
            var currentMode = EditorAppearanceHelper.GetCurrentMode();
            this.CurrentEditorAppearanceText =
                $"Current editor appearance: {currentMode}. Editor Bar uses the editor appearance when available and falls back to the IDE theme.";
            this.SelectedColorMode = currentMode;
            this.UpdateSelectedColorModePresentation(currentMode);
            this.SyncBulkBreadcrumbColorsFromSegments();
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
            this.ApplyToModel(model);
            model.Save();
        }
        catch (Exception ex)
        {
            ex.Log();
        }
    }

    /// <summary>
    /// Applies the current view-model values to the specified model without saving it.
    /// </summary>
    /// <param name="model">The model to update.</param>
    public void ApplyToModel(GeneralOptionsModel model)
    {
        this.ApplyGeneralSettingsToModel(model);
        this.ApplyColorSettingsToModel(model);
    }

    /// <summary>
    /// Applies the general, visibility, action, and local-tool settings to the specified model.
    /// </summary>
    /// <param name="model">The model to update.</param>
    public void ApplyGeneralSettingsToModel(GeneralOptionsModel model)
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
        model.ShowMemberListFilterBoxWhenEmpty = this.DisplayMemberListFilterBoxWhenEmpty;
        model.MemberTreeSearchResultViewDefault = this.MemberTreeSearchResultViewDefault;
        model.ShowLocateInSolutionExplorerButton = this.DisplayLocateInSolutionExplorerButton;
        model.ShowOpenDefaultEditorButton = this.DisplayOpenDefaultEditorButton;
        model.ShowOpenExternalEditorButton = this.DisplayOpenExternalEditorButton;
        model.ShowOpenContainingFolderButton = this.DisplayOpenContainingFolderButton;
        model.ShowOpenTerminalButton = this.DisplayOpenTerminalButton;
        model.ShowSolutionRoot = this.SolutionRootSegment.IsVisible;
        model.ShowSolutionFolders = this.SolutionFolderSegment.IsVisible;
        model.ShowProject = this.ProjectNameSegment.IsVisible;
        model.ShowProjectFolders = this.ProjectFolderSegments.IsVisible;
        model.ShowParentFolder = this.ParentFolderSegment.IsVisible;
        model.ShowFileNameBreadcrumb = this.FileSegment.IsVisible;
        model.ShowCodeStructureBreadcrumbs = this.CodeStructureSegment.IsVisible;

        model.FileAction = this.DoubleClickActionOnFileLabel;
        model.AlternateFileAction = this.DoubleClickCtrlActionOnFileLabel;

        model.ExternalEditorCommand = (this.ExternalEditorPath ?? string.Empty).Trim();
        model.ExternalEditorCommandArguments = (this.ExternalEditorArguments ?? string.Empty).Trim();
        model.TerminalProfile = Launcher.NormalizeSupportedTerminalProfile(this.SelectedTerminalProfile);
        model.TerminalCommand = (this.TerminalPath ?? string.Empty).Trim();
        model.TerminalCommandArguments = (this.TerminalArguments ?? string.Empty).Trim();

        model.DebugMode = this.IsDebugModeEnabled;
    }

    /// <summary>
    /// Applies only breadcrumb visibility and color settings to the specified model.
    /// </summary>
    /// <param name="model">The model to update.</param>
    public void ApplyColorSettingsToModel(GeneralOptionsModel model)
    {
        model.SolutionForeground = this.SolutionRootSegment.DarkForegroundColor.ToDrawingColor();
        model.SolutionBackground = this.SolutionRootSegment.DarkBackgroundColor.ToDrawingColor();
        model.LightSolutionForeground = this.SolutionRootSegment.LightForegroundColor.ToDrawingColor();
        model.LightSolutionBackground = this.SolutionRootSegment.LightBackgroundColor.ToDrawingColor();
        model.NonSolutionRootForeground = this.NonSolutionRootSegment.DarkForegroundColor.ToDrawingColor();
        model.NonSolutionRootBackground = this.NonSolutionRootSegment.DarkBackgroundColor.ToDrawingColor();
        model.LightNonSolutionRootForeground = this.NonSolutionRootSegment.LightForegroundColor.ToDrawingColor();
        model.LightNonSolutionRootBackground = this.NonSolutionRootSegment.LightBackgroundColor.ToDrawingColor();

        model.SolutionFolderForeground = this.SolutionFolderSegment.DarkForegroundColor.ToDrawingColor();
        model.SolutionFolderBackground = this.SolutionFolderSegment.DarkBackgroundColor.ToDrawingColor();
        model.LightSolutionFolderForeground = this.SolutionFolderSegment.LightForegroundColor.ToDrawingColor();
        model.LightSolutionFolderBackground = this.SolutionFolderSegment.LightBackgroundColor.ToDrawingColor();

        model.ProjectForeground = this.ProjectNameSegment.DarkForegroundColor.ToDrawingColor();
        model.ProjectBackground = this.ProjectNameSegment.DarkBackgroundColor.ToDrawingColor();
        model.LightProjectForeground = this.ProjectNameSegment.LightForegroundColor.ToDrawingColor();
        model.LightProjectBackground = this.ProjectNameSegment.LightBackgroundColor.ToDrawingColor();

        model.ProjectFoldersForeground = this.ProjectFolderSegments.DarkForegroundColor.ToDrawingColor();
        model.ProjectFoldersBackground = this.ProjectFolderSegments.DarkBackgroundColor.ToDrawingColor();
        model.LightProjectFoldersForeground = this.ProjectFolderSegments.LightForegroundColor.ToDrawingColor();
        model.LightProjectFoldersBackground = this.ProjectFolderSegments.LightBackgroundColor.ToDrawingColor();

        model.ParentFolderForeground = this.ParentFolderSegment.DarkForegroundColor.ToDrawingColor();
        model.ParentFolderBackground = this.ParentFolderSegment.DarkBackgroundColor.ToDrawingColor();
        model.LightParentFolderForeground = this.ParentFolderSegment.LightForegroundColor.ToDrawingColor();
        model.LightParentFolderBackground = this.ParentFolderSegment.LightBackgroundColor.ToDrawingColor();

        model.FileBreadcrumbForeground = this.FileSegment.DarkForegroundColor.ToDrawingColor();
        model.FileBreadcrumbBackground = this.FileSegment.DarkBackgroundColor.ToDrawingColor();
        model.LightFileBreadcrumbForeground = this.FileSegment.LightForegroundColor.ToDrawingColor();
        model.LightFileBreadcrumbBackground = this.FileSegment.LightBackgroundColor.ToDrawingColor();

        model.StructureBreadcrumbForeground = this.CodeStructureSegment.DarkForegroundColor.ToDrawingColor();
        model.StructureBreadcrumbBackground = this.CodeStructureSegment.DarkBackgroundColor.ToDrawingColor();
        model.LightStructureBreadcrumbForeground = this.CodeStructureSegment.LightForegroundColor.ToDrawingColor();
        model.LightStructureBreadcrumbBackground = this.CodeStructureSegment.LightBackgroundColor.ToDrawingColor();
    }

    private void NotifyTerminalPresentationChanged()
    {
        this.UpdateTerminalPresentation();
    }

    private void UpdateSelectedColorModePresentation(EditorColorMode value)
    {
        this.ApplyColorModeToSegments(value);
        this.SetProperty(ref this._isEditingLightColors, value == EditorColorMode.Light, nameof(this.IsEditingLightColors));
        this.SetProperty(ref this._isEditingDarkColors, value == EditorColorMode.Dark, nameof(this.IsEditingDarkColors));
        this.CopyFromOtherColorModeButtonText = value == EditorColorMode.Light
            ? "Copy colors from Dark"
            : "Copy colors from Light";
        this.SyncBulkBreadcrumbColorsFromSegments();
    }

    private void ApplyColorModeToSegments(EditorColorMode colorMode)
    {
        this.SolutionRootSegment.ActiveColorMode = colorMode;
        this.NonSolutionRootSegment.ActiveColorMode = colorMode;
        this.SolutionFolderSegment.ActiveColorMode = colorMode;
        this.ProjectNameSegment.ActiveColorMode = colorMode;
        this.ProjectFolderSegments.ActiveColorMode = colorMode;
        this.ParentFolderSegment.ActiveColorMode = colorMode;
        this.FileSegment.ActiveColorMode = colorMode;
        this.CodeStructureSegment.ActiveColorMode = colorMode;
    }

    private void CopyFromOtherColorMode()
    {
        var sourceMode = this.SelectedColorMode == EditorColorMode.Light
            ? EditorColorMode.Dark
            : EditorColorMode.Light;

        CopySegmentColors(this.SolutionRootSegment, sourceMode, this.SelectedColorMode);
        CopySegmentColors(this.NonSolutionRootSegment, sourceMode, this.SelectedColorMode);
        CopySegmentColors(this.SolutionFolderSegment, sourceMode, this.SelectedColorMode);
        CopySegmentColors(this.ProjectNameSegment, sourceMode, this.SelectedColorMode);
        CopySegmentColors(this.ProjectFolderSegments, sourceMode, this.SelectedColorMode);
        CopySegmentColors(this.ParentFolderSegment, sourceMode, this.SelectedColorMode);
        CopySegmentColors(this.FileSegment, sourceMode, this.SelectedColorMode);
        CopySegmentColors(this.CodeStructureSegment, sourceMode, this.SelectedColorMode);
        this.SyncBulkBreadcrumbColorsFromSegments();
    }

    private static void CopySegmentColors(
        EditorSegmentOptionsViewModel segment,
        EditorColorMode sourceMode,
        EditorColorMode targetMode)
    {
        if (sourceMode == EditorColorMode.Light)
        {
            if (targetMode == EditorColorMode.Dark)
            {
                segment.DarkForegroundColor = segment.LightForegroundColor;
                segment.DarkBackgroundColor = segment.LightBackgroundColor;
            }
        }
        else if (targetMode == EditorColorMode.Light)
        {
            segment.LightForegroundColor = segment.DarkForegroundColor;
            segment.LightBackgroundColor = segment.DarkBackgroundColor;
        }
    }

    private void ApplyForegroundColorToAllSegments(Color color)
    {
        foreach (var segment in this.GetColorSegments())
        {
            segment.ForegroundColor = color;
        }
    }

    private void ApplyBackgroundColorToAllSegments(Color color)
    {
        foreach (var segment in this.GetColorSegments())
        {
            segment.BackgroundColor = color;
        }
    }

    private void ResetCurrentColorModeToDefaults()
    {
        var defaultModel = new GeneralOptionsModel();

        if (this.SelectedColorMode == EditorColorMode.Light)
        {
            this.SolutionRootSegment.LightForegroundColor = defaultModel.LightSolutionForeground.ToMediaColor();
            this.SolutionRootSegment.LightBackgroundColor = defaultModel.LightSolutionBackground.ToMediaColor();
            this.NonSolutionRootSegment.LightForegroundColor = defaultModel.LightNonSolutionRootForeground.ToMediaColor();
            this.NonSolutionRootSegment.LightBackgroundColor = defaultModel.LightNonSolutionRootBackground.ToMediaColor();
            this.SolutionFolderSegment.LightForegroundColor = defaultModel.LightSolutionFolderForeground.ToMediaColor();
            this.SolutionFolderSegment.LightBackgroundColor = defaultModel.LightSolutionFolderBackground.ToMediaColor();
            this.ProjectNameSegment.LightForegroundColor = defaultModel.LightProjectForeground.ToMediaColor();
            this.ProjectNameSegment.LightBackgroundColor = defaultModel.LightProjectBackground.ToMediaColor();
            this.ProjectFolderSegments.LightForegroundColor = defaultModel.LightProjectFoldersForeground.ToMediaColor();
            this.ProjectFolderSegments.LightBackgroundColor = defaultModel.LightProjectFoldersBackground.ToMediaColor();
            this.ParentFolderSegment.LightForegroundColor = defaultModel.LightParentFolderForeground.ToMediaColor();
            this.ParentFolderSegment.LightBackgroundColor = defaultModel.LightParentFolderBackground.ToMediaColor();
            this.FileSegment.LightForegroundColor = defaultModel.LightFileBreadcrumbForeground.ToMediaColor();
            this.FileSegment.LightBackgroundColor = defaultModel.LightFileBreadcrumbBackground.ToMediaColor();
            this.CodeStructureSegment.LightForegroundColor = defaultModel.LightStructureBreadcrumbForeground.ToMediaColor();
            this.CodeStructureSegment.LightBackgroundColor = defaultModel.LightStructureBreadcrumbBackground.ToMediaColor();
        }
        else
        {
            this.SolutionRootSegment.DarkForegroundColor = defaultModel.SolutionForeground.ToMediaColor();
            this.SolutionRootSegment.DarkBackgroundColor = defaultModel.SolutionBackground.ToMediaColor();
            this.NonSolutionRootSegment.DarkForegroundColor = defaultModel.NonSolutionRootForeground.ToMediaColor();
            this.NonSolutionRootSegment.DarkBackgroundColor = defaultModel.NonSolutionRootBackground.ToMediaColor();
            this.SolutionFolderSegment.DarkForegroundColor = defaultModel.SolutionFolderForeground.ToMediaColor();
            this.SolutionFolderSegment.DarkBackgroundColor = defaultModel.SolutionFolderBackground.ToMediaColor();
            this.ProjectNameSegment.DarkForegroundColor = defaultModel.ProjectForeground.ToMediaColor();
            this.ProjectNameSegment.DarkBackgroundColor = defaultModel.ProjectBackground.ToMediaColor();
            this.ProjectFolderSegments.DarkForegroundColor = defaultModel.ProjectFoldersForeground.ToMediaColor();
            this.ProjectFolderSegments.DarkBackgroundColor = defaultModel.ProjectFoldersBackground.ToMediaColor();
            this.ParentFolderSegment.DarkForegroundColor = defaultModel.ParentFolderForeground.ToMediaColor();
            this.ParentFolderSegment.DarkBackgroundColor = defaultModel.ParentFolderBackground.ToMediaColor();
            this.FileSegment.DarkForegroundColor = defaultModel.FileBreadcrumbForeground.ToMediaColor();
            this.FileSegment.DarkBackgroundColor = defaultModel.FileBreadcrumbBackground.ToMediaColor();
            this.CodeStructureSegment.DarkForegroundColor = defaultModel.StructureBreadcrumbForeground.ToMediaColor();
            this.CodeStructureSegment.DarkBackgroundColor = defaultModel.StructureBreadcrumbBackground.ToMediaColor();
        }

        this.SyncBulkBreadcrumbColorsFromSegments();
    }

    private EditorSegmentOptionsViewModel[] GetColorSegments()
    {
        return
        [
            this.SolutionRootSegment,
            this.NonSolutionRootSegment,
            this.SolutionFolderSegment,
            this.ProjectNameSegment,
            this.ProjectFolderSegments,
            this.ParentFolderSegment,
            this.FileSegment,
            this.CodeStructureSegment
        ];
    }

    private void SyncBulkBreadcrumbColorsFromSegments()
    {
        this.SetProperty(ref this._bulkBreadcrumbForegroundColor, this.FileSegment.ForegroundColor, nameof(this.BulkBreadcrumbForegroundColor));
        this.SetProperty(ref this._bulkBreadcrumbBackgroundColor, this.FileSegment.BackgroundColor, nameof(this.BulkBreadcrumbBackgroundColor));
    }

    private void UpdateTerminalPresentation()
    {
        var isCustomTerminalSelected = this.SelectedTerminalProfile == TerminalProfile.Custom;
        this.IsCustomTerminalSelected = isCustomTerminalSelected;
        this.IsTerminalPresetLocked = !isCustomTerminalSelected;
        this.TerminalPathDisplay = isCustomTerminalSelected
            ? this.TerminalPath ?? string.Empty
            : Launcher.GetTerminalDisplayCommand(this.SelectedTerminalProfile);
        this.TerminalArgumentsDisplay = isCustomTerminalSelected
            ? this.TerminalArguments ?? string.Empty
            : Launcher.GetTerminalDisplayArguments(this.SelectedTerminalProfile);
        this.TerminalPresetHint = isCustomTerminalSelected
            ? "Use $(WorkingDirectory) for the terminal start directory and $(ItemPath) for the invoked file or folder path."
            : Launcher.GetTerminalPresetNote(this.SelectedTerminalProfile);
    }
}
