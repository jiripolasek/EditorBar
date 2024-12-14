// ------------------------------------------------------------
//
// Copyright (c) Jiří Polášek. All rights reserved.
//
// ------------------------------------------------------------

using System.IO;
using System.Text;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using Community.VisualStudio.Toolkit;
using JPSoftworks.EditorBar.Commands;
using JPSoftworks.EditorBar.Helpers;
using JPSoftworks.EditorBar.Options;
using Microsoft.VisualStudio.PlatformUI;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;
using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Text.Editor;
using Microsoft.VisualStudio.Threading;

namespace JPSoftworks.EditorBar;

/// <summary>
/// Interaction logic for EditorBarControl.xaml
/// </summary>
public partial class EditorBarControl : IDisposable
{
    public static readonly DependencyProperty SolutionElementBackgroundProperty =
        DependencyProperty.Register(nameof(SolutionElementBackground), typeof(Brush), typeof(EditorBarControl), new PropertyMetadata(Brushes.Purple));

    public static readonly DependencyProperty SolutionElementForegroundProperty =
        DependencyProperty.Register(nameof(SolutionElementForeground), typeof(Brush), typeof(EditorBarControl), new PropertyMetadata(Brushes.Black));

    public static readonly DependencyProperty ProjectElementBackgroundProperty =
        DependencyProperty.Register(nameof(ProjectElementBackground), typeof(Brush), typeof(EditorBarControl), new PropertyMetadata(Brushes.LightSkyBlue));

    public static readonly DependencyProperty ProjectElementForegroundProperty =
        DependencyProperty.Register(nameof(ProjectElementForeground), typeof(Brush), typeof(EditorBarControl), new PropertyMetadata(Brushes.Black));

    public static readonly DependencyProperty SolutionFolderElementBackgroundProperty =
        DependencyProperty.Register(nameof(SolutionFolderElementBackground), typeof(Brush), typeof(EditorBarControl), new PropertyMetadata(Brushes.Gold));

    public static readonly DependencyProperty SolutionFolderElementForegroundProperty =
        DependencyProperty.Register(nameof(SolutionFolderElementForeground), typeof(Brush), typeof(EditorBarControl), new PropertyMetadata(Brushes.Black));

    public static readonly DependencyProperty ParentFolderElementBackgroundProperty =
        DependencyProperty.Register(nameof(ParentFolderElementBackground), typeof(Brush), typeof(EditorBarControl), new PropertyMetadata(Brushes.YellowGreen));

    public static readonly DependencyProperty ParentFolderElementForegroundProperty =
        DependencyProperty.Register(nameof(ParentFolderElementForeground), typeof(Brush), typeof(EditorBarControl), new PropertyMetadata(Brushes.Black));

    public static readonly DependencyProperty ShowParentFolderElementProperty =
        DependencyProperty.Register(nameof(ShowParentFolderElement), typeof(bool), typeof(EditorBarControl), new PropertyMetadata(false));

    public static readonly DependencyProperty ShowSolutionFoldersProperty =
        DependencyProperty.Register(nameof(ShowSolutionFolders), typeof(bool), typeof(EditorBarControl), new PropertyMetadata(false));

    public static readonly DependencyProperty ShowProjectElementProperty =
        DependencyProperty.Register(nameof(ShowProjectElement), typeof(bool), typeof(EditorBarControl), new PropertyMetadata(false));

    public static readonly DependencyProperty ShowSolutionRootProperty =
        DependencyProperty.Register(nameof(ShowSolutionRoot), typeof(bool), typeof(EditorBarControl), new PropertyMetadata(false));

    public static readonly DependencyProperty IsDevelopmentModeEnabledProperty =
        DependencyProperty.Register(nameof(IsDevelopmentModeEnabled), typeof(bool), typeof(EditorBarControl), new PropertyMetadata(false));

    public static readonly DependencyProperty ParentFolderElementCornerRadiusProperty = DependencyProperty.Register(
        nameof(ParentFolderElementCornerRadius), typeof(CornerRadius), typeof(EditorBarControl), new PropertyMetadata(default(CornerRadius)));

    public static readonly DependencyProperty ProjectElementCornerRadiusProperty = DependencyProperty.Register(
        nameof(ProjectElementCornerRadius), typeof(CornerRadius), typeof(EditorBarControl), new PropertyMetadata(default(CornerRadius)));

    private readonly IWpfTextView? _textView;
    private readonly JoinableTaskFactory _joinableTaskFactory;
    private bool _showParentFolderElementEnabled;

    public EditorBarControl(IWpfTextView textView, JoinableTaskFactory joinableTaskFactory)
    {
        if (textView == null)
            throw new ArgumentNullException(nameof(textView));
        if (joinableTaskFactory == null)
            throw new ArgumentNullException(nameof(joinableTaskFactory));

        this.InitializeComponent();
        this._textView = textView;
        this._joinableTaskFactory = joinableTaskFactory;
        this.IsVisibleChanged += this.OnIsVisibleChanged;

        GeneralOptionsModel.Saved += this.OnGeneralPageOnSaved;

        VS.Events.ProjectItemsEvents.AfterRenameProjectItems += this.OnProjectItemsEventsOnAfterRenameProjectItems;
        VS.Events.ProjectItemsEvents.AfterAddProjectItems += this.OnProjectItemsEventsOnAfterAddProjectItems;
        VS.Events.ProjectItemsEvents.AfterRemoveProjectItems += this.OnProjectItemsEventsOnAfterRemoveProjectItems;
        VS.Events.DocumentEvents.Saved += this.OnDocumentEventsOnSaved;
        VS.Events.SolutionEvents.OnAfterRenameProject += this.OnSolutionEventsOnOnAfterRenameProject;
        VS.Events.SolutionEvents.OnAfterOpenSolution += this.OnSolutionEventsOnOnAfterOpenSolution;
        VS.Events.ShellEvents.EnvironmentColorChanged += this.ShellEventsOnEnvironmentColorChanged;
    }

    public Brush SolutionElementBackground
    {
        get => (Brush)this.GetValue(SolutionElementBackgroundProperty)!;
        set => this.SetValue(SolutionElementBackgroundProperty, value);
    }

    public Brush SolutionElementForeground
    {
        get => (Brush)this.GetValue(SolutionElementForegroundProperty)!;
        set => this.SetValue(SolutionElementForegroundProperty, value);
    }

    public Brush ProjectElementBackground
    {
        get => (Brush)this.GetValue(ProjectElementBackgroundProperty)!;
        set => this.SetValue(ProjectElementBackgroundProperty, value);
    }

    public Brush ProjectElementForeground
    {
        get => (Brush)this.GetValue(ProjectElementForegroundProperty)!;
        set => this.SetValue(ProjectElementForegroundProperty, value);
    }

    public Brush SolutionFolderElementBackground
    {
        get => (Brush)this.GetValue(SolutionFolderElementBackgroundProperty)!;
        set => this.SetValue(SolutionFolderElementBackgroundProperty, value);
    }

    public Brush SolutionFolderElementForeground
    {
        get => (Brush)this.GetValue(SolutionFolderElementForegroundProperty)!;
        set => this.SetValue(SolutionFolderElementForegroundProperty, value);
    }

    public bool ShowSolutionFolders
    {
        get => (bool)this.GetValue(ShowSolutionFoldersProperty);
        set => this.SetValue(ShowSolutionFoldersProperty, value);
    }

    public bool ShowSolutionRoot
    {
        get => (bool)this.GetValue(ShowSolutionRootProperty);
        set => this.SetValue(ShowSolutionRootProperty, value);
    }

    public bool ShowProjectElement
    {
        get { return (bool)this.GetValue(ShowProjectElementProperty); }
        set { this.SetValue(ShowProjectElementProperty, value); }
    }

    public bool ShowParentFolderElement
    {
        get => (bool)this.GetValue(ShowParentFolderElementProperty);
        set => this.SetValue(ShowParentFolderElementProperty, value);
    }

    public Brush ParentFolderElementBackground
    {
        get => (Brush)this.GetValue(ParentFolderElementBackgroundProperty)!;
        set => this.SetValue(ParentFolderElementBackgroundProperty, value);
    }

    public Brush ParentFolderElementForeground
    {
        get => (Brush)this.GetValue(ParentFolderElementForegroundProperty)!;
        set => this.SetValue(ParentFolderElementForegroundProperty, value);
    }

    public CornerRadius ParentFolderElementCornerRadius
    {
        get => (CornerRadius)this.GetValue(ParentFolderElementCornerRadiusProperty);
        set => this.SetValue(ParentFolderElementCornerRadiusProperty, value);
    }

    public CornerRadius ProjectElementCornerRadius
    {
        get => (CornerRadius)this.GetValue(ProjectElementCornerRadiusProperty);
        set => this.SetValue(ProjectElementCornerRadiusProperty, value);
    }

    public bool IsDevelopmentModeEnabled
    {
        get => (bool)this.GetValue(IsDevelopmentModeEnabledProperty);
        set => this.SetValue(IsDevelopmentModeEnabledProperty, value);
    }

    private string? FilePath { get; set; }

    private string? RelativePath { get; set; }

    public void Dispose()
    {
        GeneralOptionsModel.Saved -= this.OnGeneralPageOnSaved;

        VS.Events.ProjectItemsEvents.AfterRenameProjectItems -= this.OnProjectItemsEventsOnAfterRenameProjectItems;
        VS.Events.ProjectItemsEvents.AfterAddProjectItems -= this.OnProjectItemsEventsOnAfterAddProjectItems;
        VS.Events.ProjectItemsEvents.AfterRemoveProjectItems -= this.OnProjectItemsEventsOnAfterRemoveProjectItems;
        VS.Events.DocumentEvents.Saved -= this.OnDocumentEventsOnSaved;
        VS.Events.SolutionEvents.OnAfterRenameProject -= this.OnSolutionEventsOnOnAfterRenameProject;
        VS.Events.SolutionEvents.OnAfterOpenSolution -= this.OnSolutionEventsOnOnAfterOpenSolution;
        VS.Events.ShellEvents.EnvironmentColorChanged -= this.ShellEventsOnEnvironmentColorChanged;
    }

    private void OnSomethingAboutDocumentNameChanged()
    {
        this.UpdateAsync(true).FireAndForget();
    }

    private async Task UpdateAsync(bool forced = false)
    {
        if (this.IsVisible == false)
        {
            return;
        }

        var document = this.GetCurrentDocument();
        if (document == null)
        {
            return;
        }

        await ThreadHelper.JoinableTaskFactory!.SwitchToMainThreadAsync();

        document.FileActionOccurred -= this.HandleDocumentOnFileAction;
        document.FileActionOccurred += this.HandleDocumentOnFileAction;

        var project = await FindParentProjectAsync(document);
        await this.UpdateProjectElementAsync(project);
        this.UpdateFilePathLabel(document, project, forced);
    }

    private static async Task<Project?> FindParentProjectAsync(ITextDocument document)
    {
        var dteProject = await VisualStudioHelper.GetProjectFromDocumentAsync(document);

        if (dteProject != null)
        {
            return await VisualStudioHelper.ConvertToSolutionItemAsync(dteProject);
        }

        var allProjects = await VS.Solutions.GetAllProjectsAsync(ProjectStateFilter.All);
        return allProjects.FirstOrDefault(t => t.FullPath == document.FilePath);
    }

    private async Task UpdateProjectElementAsync(Project? project)
    {
        await this._joinableTaskFactory.SwitchToMainThreadAsync();

        if (project != null)
        {
            var parents = await VisualStudioHelper.GetSolutionFolderPathAsync(project);
            this.ProjectNameLabel!.Content = project.Name;
            this.SolutionFoldersList!.ItemsSource = parents;
        }
        else
        {
            this.ProjectNameLabel!.Content = "(no project)";
            this.SolutionFoldersList!.ItemsSource = Array.Empty<string>();
        }
    }

    private void ReapplySettings()
    {
        var options = GeneralOptionsModel.Instance;

        this.SolutionElementBackground = new SolidColorBrush(options.SolutionBackground.ToMediaColor());
        this.SolutionElementForeground = new SolidColorBrush(options.SolutionForeground.ToMediaColor());

        this.ProjectElementBackground = new SolidColorBrush(options.ProjectBackground.ToMediaColor());
        this.ProjectElementForeground = new SolidColorBrush(options.ProjectForeground.ToMediaColor());

        this.SolutionFolderElementBackground = new SolidColorBrush(options.SolutionFolderBackground.ToMediaColor());
        this.SolutionFolderElementForeground = new SolidColorBrush(options.SolutionFolderForeground.ToMediaColor());

        this.ParentFolderElementBackground = new SolidColorBrush(options.ParentFolderBackground.ToMediaColor());
        this.ParentFolderElementForeground = new SolidColorBrush(options.ParentFolderForeground.ToMediaColor());

        this.ShowSolutionFolders = options.ShowSolutionFolders;
        this.ShowSolutionRoot = options.ShowSolutionRoot;
        this.ShowProjectElement = options.ShowProject;
        this._showParentFolderElementEnabled = options.ShowParentFolder;

        this.ParentFolderElementCornerRadius = new CornerRadius(0, 4, 4, 0);
        this.ProjectElementCornerRadius = new CornerRadius(0, 4, 4, 0);

        this.OpenExternalEditorButton!.Visibility = StringHelper.IsNullOrWhiteSpace(options.ExternalEditorCommand) ? Visibility.Collapsed : Visibility.Visible;

        this.IsDevelopmentModeEnabled = options.DebugMode;

        this.ReloadStyle();
        // allow to change background color of the editor bar
        // in case of the top panel, we can follow the color of the editor so it integrates seemlesly;
        // bottom panel is below the scrollbar, so it doesn't make sense to follow the editor color
        if (GeneralOptionsModel.Instance.BarPosition == BarPosition.Top)
        {
            switch (GeneralOptionsModel.Instance.VisualStyle)
            {
                case VisualStyle.FullRowCommandBar:
                    this.Background = (Brush)this.FindResource(VsBrushes.CommandBarGradientKey!)!;
                    this.BorderBrush = (Brush)this.FindResource(SearchControlColors.PopupBorderBrushKey!)!;
                    this.BorderThickness = new Thickness(0, 0, 0, 1);
                    break;
                case VisualStyle.FullRowTransparent:
                    // copy background from the editor so the theme ImageThemingUtilities.ImageBackgroundColor will work
                    this.Background = this._textView?.Background ?? Brushes.Transparent;
                    this.BorderBrush = (Brush)this.FindResource(SearchControlColors.PopupBorderBrushKey!)!;
                    this.BorderThickness = new Thickness(0, 0, 0, 1);
                    break;
                case VisualStyle.FullRowToolWindow:
                    this.Background = (Brush)this.FindResource(VsBrushes.ToolWindowBackgroundKey!)!;
                    this.BorderBrush = (Brush)this.FindResource(SearchControlColors.PopupBorderBrushKey!)!;
                    this.BorderThickness = new Thickness(0, 0, 0, 1);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            // floating island
            //this.Background = Brushes.Transparent;
            //this.BorderThickness = new Thickness(0);

            //EditorBarBorder.CornerRadius = new CornerRadius(4);
            //EditorBarBorder.Background = (Brush)this.FindResource(EnvironmentColors.ToolWindowBackgroundBrushKey!)!;
            //EditorBarBorder.Margin = new Thickness(4);
        }
        else
        {
            this.Background = (Brush)this.FindResource(VsBrushes.CommandBarGradientKey!)!;
            this.BorderBrush = Brushes.Transparent;
            this.BorderThickness = new Thickness(0);
        }
    }

    private void ReloadStyle()
    {
        // remove old style (remove only styles from this extension), we have also VS theme loaded on the control (toolkit:Themes.UseVsTheme="True"
        var currentStyleResourceDictionaries = this.Resources.MergedDictionaries.Where(IsEditorBarStyleXamlComponent).ToList();

        foreach (var c in currentStyleResourceDictionaries)
        {
            this.Resources.MergedDictionaries.Remove(c);
        }

        // add new style
        var newStyleUri = new Uri($"pack://application:,,,/EditorBar;component/Styles/{GeneralOptionsModel.Instance.DisplayStyle}Style.xaml");
        var newResourceDict = new ResourceDictionary { Source = newStyleUri };
        this.Resources.MergedDictionaries.Add(newResourceDict);
        return;

        static bool IsEditorBarStyleXamlComponent(ResourceDictionary resourceDictionary) =>
            resourceDictionary.Source != null
            && resourceDictionary.Source.IsAbsoluteUri
            && resourceDictionary.Source.AbsolutePath.Contains("/EditorBar;")
            && resourceDictionary.Source.AbsolutePath.Contains("Style.xaml");
    }



    private void UpdateFilePathLabel(ITextDocument document, Project? project, bool forced)
    {
        ThreadHelper.ThrowIfNotOnUIThread();

        if (!forced && string.Equals(this.FilePath!, document.FilePath!, StringComparison.Ordinal))
        {
            return;
        }

        this.FilePath = document.FilePath;
        this.RelativePath = GetRelativePathToSolution(this.FilePath);

        var pathLabelText = GeneralOptionsModel.Instance.ShowPathRelativeToSolutionRoot
            ? this.RelativePath
            : this.FilePath;
        this.PathLabel!.Content = pathLabelText ?? "(unnamed document)";

        // get immediate parent folder name
        // if the file is in a project, the use path relative to the project and hide the parent folder if it's the same as the project name
        // if project element is not visible, simply display the parent folder name; if it is visible, then display the parent folder name only if it's different from the project name
        var parentFolderName = "";

        if (!this.ShowProjectElement)
        {
            parentFolderName = Path.GetFileName(Path.GetDirectoryName(this.FilePath!) ?? "");
        }
        else
        {
            if (project != null && !string.IsNullOrWhiteSpace(project.FullPath!))
            {
                var projectPath = project.FullPath ?? "";
                var projectDir = Path.GetDirectoryName(projectPath);
                if (projectDir != null)
                {
                    var relativePath = GetRelativePath(this.FilePath!, projectDir);
                    parentFolderName = Path.GetFileName(Path.GetDirectoryName(relativePath) ?? "");
                }
            }
            else
            {
                var solution = VS.Solutions.GetCurrentSolution();
                if (solution != null)
                {
                    var solutionDir = Path.GetDirectoryName(solution.FullPath ?? "");
                    if (solutionDir != null)
                    {
                        var relativePath = GetRelativePath(this.FilePath!, solutionDir);
                        parentFolderName = Path.GetFileName(Path.GetDirectoryName(relativePath) ?? "");
                    }
                }
                else
                {
                    parentFolderName = Path.GetFileName(Path.GetDirectoryName(this.FilePath!) ?? "");
                }
            }
        }

        if (this._showParentFolderElementEnabled && !string.IsNullOrWhiteSpace(parentFolderName))
        {
            this.ShowParentFolderElement = true;
            this.ParentFolderLabel!.Content = parentFolderName;
        }
        else
        {
            this.ShowParentFolderElement = false;
            this.ParentFolderLabel!.Content = "";
        }
    }

    private static string? GetRelativePathToSolution(string? path)
    {
        if (path == null)
        {
            return null;
        }

        var currentSolution = VS.Solutions.GetCurrentSolution();
        var slnPath = currentSolution?.FullPath;
        if (string.IsNullOrWhiteSpace(slnPath!))
        {
            return path;
        }

        var slnDir = Path.GetDirectoryName(slnPath!);
        return slnDir == null ? path : GetRelativePath(path, slnDir);
    }

    private static string GetRelativePath(string filePath, string slnDir)
    {
        return !filePath.StartsWith(slnDir, StringComparison.OrdinalIgnoreCase)
            ? filePath
            : filePath.Substring(slnDir.Length);
    }


    private void HandleDocumentOnFileAction(object sender, TextDocumentFileActionEventArgs e)
    {
        this.UpdateAsync().FireAndForget();
    }

    private ITextDocument? GetCurrentDocument()
    {
        ITextDocument? document = null;
        this._textView?.TextDataModel?.DocumentBuffer?.Properties.TryGetProperty(typeof(ITextDocument), out document);
        return document;
    }

    private void OpenFolderClicked(object sender, RoutedEventArgs e)
    {
        Launcher.OpenContaingFolder(this.FilePath);
    }

    private void OpenOptionsButtonClicked(object sender, RoutedEventArgs e)
    {
        VS.Settings.OpenAsync<GeneralOptionPage>().FireAndForget();
    }

    private void PathLabel_OnMouseDoubleClick(object sender, MouseButtonEventArgs e)
    {
        var hasControl = Keyboard.IsKeyDown(Key.LeftCtrl) || Keyboard.IsKeyDown(Key.RightCtrl);
        var action = hasControl
            ? GeneralOptionsModel.Instance.AlternateFileAction
            : GeneralOptionsModel.Instance.FileAction;

        switch (action)
        {
            case FileAction.None:
                break;
            case FileAction.OpenContainingFolder:
                Launcher.OpenContaingFolder(this.FilePath);
                break;
            case FileAction.OpenInExternalEditor:
                Launcher.OpenInExternalEditor(this.FilePath);
                break;
            case FileAction.OpenInDefaultEditor:
                Launcher.OpenInDefaultEditor(this.FilePath);
                break;
            case FileAction.CopyRelativePath:
                Launcher.CopyRelativePathFromFullPath(this.FilePath);
                break;
            case FileAction.CopyAbsolutePath:
                Launcher.CopyAbsolutePath(this.FilePath);
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
    }

    private void OpenExternalEditorClicked(object sender, RoutedEventArgs e)
    {
        Launcher.OpenInExternalEditor(this.FilePath);
    }

    private void OpenDefaultEditorClicked(object sender, RoutedEventArgs e)
    {
        Launcher.OpenInDefaultEditor(this.FilePath);
    }

    private void OnIsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
    {
        if ((bool)e.NewValue)
        {
            this.OnSettingsChanged();
        }
    }

    private void OnSettingsChanged()
    {
        if (this.IsVisible)
        {
            this.ReapplySettings();
            this.UpdateAsync(true).FireAndForget();
        }
    }

    private void OnGeneralPageOnSaved(GeneralOptionsModel _)
    {
        this.OnSettingsChanged();
    }

    private void OnProjectItemsEventsOnAfterRenameProjectItems(AfterRenameProjectItemEventArgs? _)
    {
        this.OnSomethingAboutDocumentNameChanged();
    }

    private void OnProjectItemsEventsOnAfterAddProjectItems(IEnumerable<SolutionItem> _)
    {
        this.OnSomethingAboutDocumentNameChanged();
    }

    private void OnProjectItemsEventsOnAfterRemoveProjectItems(AfterRemoveProjectItemEventArgs? _)
    {
        this.OnSomethingAboutDocumentNameChanged();
    }

    private void OnDocumentEventsOnSaved(string _)
    {
        this.OnSomethingAboutDocumentNameChanged();
    }

    private void OnSolutionEventsOnOnAfterRenameProject(Project? _)
    {
        this.OnSomethingAboutDocumentNameChanged();
    }

    private void OnSolutionEventsOnOnAfterOpenSolution(Solution? _)
    {
        this.OnSomethingAboutDocumentNameChanged();
    }

    private void ShellEventsOnEnvironmentColorChanged()
    {
        this.ReapplySettings();
    }

    private void UIElement_OnMouseRightButtonUp(object sender, MouseButtonEventArgs e)
    {
        var doc = this.GetCurrentDocument();
        if (doc is null)
        {
            return;
        }

        this._joinableTaskFactory.RunAsync(async () =>
        {
            try
            {
                var bridge = await VS.GetRequiredServiceAsync<EditorBarFileActionMenuBridge, EditorBarFileActionMenuBridge>();
                Point point = this.PointToScreen(e.GetPosition(this));
                await bridge.ShowAsync(doc, (int)point.X, (int)point.Y);
            }
            catch (Exception ex)
            {
                await ex.LogAsync();
            }
        }).FireAndForget();
    }

    private void DebugButtonClicked(object sender, RoutedEventArgs e)
    {
        var sb = new StringBuilder();

        var roles = this._textView?.Roles;
        if (roles != null)
        {
            sb.AppendLine("Roles:");
            foreach (var role in roles.OrderBy(t => t))
            {
                sb.AppendFormat(" - {0}", role).AppendLine();
            }
        }

        // show message box:
        VS.MessageBox.Show(sb.ToString(),
            "",
            OLEMSGICON.OLEMSGICON_INFO,
            OLEMSGBUTTON.OLEMSGBUTTON_OK);
    }
}