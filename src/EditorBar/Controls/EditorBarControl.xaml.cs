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
using JPSoftworks.EditorBar.Models;
using JPSoftworks.EditorBar.Options;
using Microsoft.VisualStudio.Language.Intellisense;
using Microsoft.VisualStudio.PlatformUI;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;
using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Text.Editor;
using Microsoft.VisualStudio.Threading;

namespace JPSoftworks.EditorBar.Controls;

/// <summary>
/// Interaction logic for EditorBarControl.xaml
/// </summary>
public partial class EditorBarControl : IDisposable
{
    public static readonly DependencyProperty IsDevelopmentModeEnabledProperty = DependencyProperty.Register(nameof(IsDevelopmentModeEnabled), typeof(bool), typeof(EditorBarControl), new PropertyMetadata(false));
    public static readonly DependencyProperty FilePathProperty = DependencyProperty.Register(nameof(FilePath), typeof(string), typeof(EditorBarControl), new PropertyMetadata(null!));

    private static readonly char[] DirectorySeparators = [Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar];
    private const string MiscProjectKindGuid = "{66A26720-8FB5-11D2-AA7E-00C04F688DDE}";

    private readonly IWpfTextView? _textView;
    private readonly JoinableTaskFactory _joinableTaskFactory;

    private List<string>? _solutionFolders;
    private string[] _inProjectPathElements = [];
    private SolidColorBrush _projectFoldersBackground = Brushes.White;
    private SolidColorBrush _projectFoldersForeground = Brushes.Black;
    private string? _semiRoot;

    public bool IsDevelopmentModeEnabled
    {
        get => (bool)this.GetValue(IsDevelopmentModeEnabledProperty);
        set => this.SetValue(IsDevelopmentModeEnabledProperty, value);
    }

    public BulkObservableCollection<BreadcrumbModel> Breadcrumbs { get; } = [];

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

        IProjectWrapper? projectWrapper = null;

        var physicalDocument = await PhysicalFile.FromFileAsync(document.FilePath!);
        var project = physicalDocument?.ContainingProject;
        if (project != null)
        {
            projectWrapper = new ProjectWrapper(project);
        }
        else
        {
            // find in other projects (e.g. misc files) that are not convered by Tookit Project and thus not by PhysicalDocument.ContainingProject
            projectWrapper ??= await FindParentProjectAsync(document);

            // handle solution root folder (for the files that are in solution folder, but not part of the solution or project itself)
            // - add as a file system root instead of solution root to mark that the file is not directly part of solution
            if (projectWrapper == null)
            {
                // if the file is in the solution folder, but not part of the solution or project itself
                var solution = await VS.Solutions.GetCurrentSolutionAsync();
                if (solution != null)
                {
                    var solutionPath = Path.GetDirectoryName(solution.FullPath!);
                    if (solutionPath != null && document.FilePath!.StartsWith(solutionPath))
                    {
                        projectWrapper = new FileSystemWrapper("\\", solutionPath);
                    }
                }
            }

            // handle known fake roots (like temp, app data, etc.)
            if (projectWrapper == null)
            {
                var documentPath = document.FilePath!;
                foreach (var knownFakeRoot in KnownFakeRoots.FakeRoots)
                {
                    if (documentPath.StartsWith(knownFakeRoot.Path, StringComparison.OrdinalIgnoreCase))
                    {
                        projectWrapper = new FileSystemWrapper(knownFakeRoot.DisplayName, knownFakeRoot.Path);
                        break;
                    }
                }
            }
        }


        // fallback for all other external paths
        // distinguis between local and network files from the path and change the display name accordingly (This PC, Network, ...)

        if (projectWrapper == null)
        {
            var displayName = document.FilePath!.StartsWith(@"\\") ? "Network" : "This PC"; // network path
            projectWrapper = new FileSystemWrapper(displayName, null);
        }


        this._solutionFolders = projectWrapper switch
        {
            MiscFilesWrapper => await this.GetSolutionFoldersAsync(physicalDocument),
            ProjectWrapper p => await this.GetSolutionFoldersAsync(p.Project),
            _ => []
        };

        this.UpdateFilePathLabel(document, projectWrapper, forced);

        var options = GeneralOptionsModel.Instance;

        var breadcrumbs = this.RebuildBreadcrumbs(options, projectWrapper);
        this.SetBreadcrumbs(breadcrumbs);
    }

    private List<BreadcrumbModel> RebuildBreadcrumbs(GeneralOptionsModel options, IProjectWrapper projectWrapper)
    {
        // Rebuild breadcrumbs
        var breadcrumbs = new List<BreadcrumbModel>();

        // 1) Root
        if (options.ShowSolutionRoot)
        {
            var crumb = projectWrapper switch
            {
                SolutionProject s => new BreadcrumbModel("\\", options.SolutionBackground, options.SolutionForeground) { AssociatedFile = s.DirectoryPath },
                FileSystemWrapper f => new BreadcrumbModel(f.DisplayName, options.NonSolutionRootBackground, options.NonSolutionRootForeground) { AssociatedDirectory = f.DirectoryPath },
                _ => throw new ArgumentOutOfRangeException()
            };
            breadcrumbs.Add(crumb);
        }

        // 2) Solution folders
        if (options.ShowSolutionFolders && this._solutionFolders != null && this._solutionFolders.Any())
        {
            breadcrumbs.AddRange(this._solutionFolders.Select(folder => new BreadcrumbModel(folder, options.SolutionFolderBackground, options.SolutionFolderForeground)));
        }

        // 3) Project
        if (options.ShowProject)
        {
            var crumb = projectWrapper switch
            {
                SolutionProject p => new BreadcrumbModel(p.DisplayName, options.ProjectBackground, options.ProjectForeground) { AssociatedDirectory = p.DirectoryPath },
                _ => null
            };

            if (crumb != null)
            {
                breadcrumbs.Add(crumb);
            }
        }

        // 4) project folders + parent folder
        if (options.ShowProjectFolders && this._inProjectPathElements.Length > 0)
        {
            // show in-project path part, if there's more that two elements; skip the last element, if that is the parent folder that is displayed separately
            var endIndex = options.ShowParentFolder ? this._inProjectPathElements.Length - 1 : this._inProjectPathElements.Length;
            for (var i = 0; i < endIndex; i++)
            {
                breadcrumbs.Add(new BreadcrumbModel(this._inProjectPathElements[i], this._projectFoldersBackground, this._projectFoldersForeground));
            }
        }

        // 5) immediate parent folder (if enabled explictly, to be displayed as a breadcrumb with a different color or without 
        if (options.ShowParentFolder && this._inProjectPathElements.Length > 0)
        {
            breadcrumbs.Add(new BreadcrumbModel(this._inProjectPathElements.Last(), options.ParentFolderBackground, options.ParentFolderForeground));
        }

        // find what is the first element and mark it, so it is rendered without the left border
        var first = breadcrumbs.FirstOrDefault();
        if (first != null)
        {
            first.IsMiddle = false;
        }

        return breadcrumbs;
    }

    private void SetBreadcrumbs(List<BreadcrumbModel> breadcrumbs)
    {
        this.Breadcrumbs.BeginBulkOperation();
        this.Breadcrumbs.Clear();
        this.Breadcrumbs.AddRange(breadcrumbs);
        this.Breadcrumbs.EndBulkOperation();
    }

    private static async Task<IProjectWrapper?> FindParentProjectAsync(ITextDocument document)
    {
        await ThreadHelper.JoinableTaskFactory!.SwitchToMainThreadAsync();

        var dteProject = await VisualStudioHelper.GetProjectFromDocumentAsync(document);

        if (dteProject != null)
        {
            // is misc:
            if (dteProject.Kind == MiscProjectKindGuid)
            {
                var currentSolution = await VS.Solutions.GetCurrentSolutionAsync();
                return new MiscFilesWrapper(currentSolution?.FullPath);
            }

            var project2 = await VisualStudioHelper.ConvertToSolutionItemAsync(dteProject);
            if (project2 != null)
            {
                return new ProjectWrapper(project2);
            }
        }

        // handle case when the document is a project file itself
        var allProjects = await VS.Solutions.GetAllProjectsAsync(ProjectStateFilter.All);
        var project = allProjects.FirstOrDefault(t => t.FullPath == document.FilePath);
        return project == null ? null : new ProjectWrapper(project);
    }

    private async Task<List<string>> GetSolutionFoldersAsync(SolutionItem? solutionItem)
    {
        await this._joinableTaskFactory.SwitchToMainThreadAsync();
        return solutionItem == null ? [] : await VisualStudioHelper.GetSolutionFolderPathAsync(solutionItem);
    }

    private void ReapplySettings()
    {
        var options = GeneralOptionsModel.Instance;

        this._projectFoldersBackground = new SolidColorBrush(options.ProjectFoldersBackground.ToMediaColor());
        this._projectFoldersForeground = new SolidColorBrush(options.ProjectFoldersForeground.ToMediaColor());

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
        currentStyleResourceDictionaries.ForEach(resourceDictionary => this.Resources.MergedDictionaries.Remove(resourceDictionary));

        // add new style
        this.Resources.MergedDictionaries.Add(new ResourceDictionary { Source = new Uri($"pack://application:,,,/EditorBar;component/Themes/EditorBar.{GeneralOptionsModel.Instance.DisplayStyle}.xaml") });
        return;

        static bool IsEditorBarStyleXamlComponent(ResourceDictionary resourceDictionary) =>
            resourceDictionary.Source != null
            && resourceDictionary.Source.IsAbsoluteUri
            && resourceDictionary.Source.AbsolutePath.Contains("/EditorBar;component/Themes/EditorBar.");
    }



    private void UpdateFilePathLabel(ITextDocument document, IProjectWrapper projectWrapper, bool forced)
    {
        ThreadHelper.ThrowIfNotOnUIThread();

        if (!forced && string.Equals(this.FilePath!, document.FilePath!, StringComparison.Ordinal))
        {
            return;
        }

        this.FilePath = document.FilePath;
        var pathLabelText = this.FormatFileNameLabel(document.FilePath ?? "", projectWrapper);
        this.PathLabel!.Content = pathLabelText ?? "(Untitled document)";

        // get immediate parent folder name
        // if the file is in a project, the use path relative to the project and hide the parent folder if it's the same as the project name
        // if project element is not visible, simply display the parent folder name; if it is visible, then display the parent folder name only if it's different from the project name

        string? semiRoot = projectWrapper.DirectoryPath;

        string[] inProjectPathElements;

        if (semiRoot != null)
        {
            var relativePath = GetRelativePath(Path.GetDirectoryName(this.FilePath!)!, semiRoot);
            inProjectPathElements = relativePath.Trim(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar).Split(DirectorySeparators, StringSplitOptions.RemoveEmptyEntries);
        }
        else
        {
            var absolutePath = Path.GetDirectoryName(this.FilePath!) ?? "";
            inProjectPathElements = absolutePath.Trim(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar).Split(DirectorySeparators, StringSplitOptions.RemoveEmptyEntries);
        }

        this._semiRoot = semiRoot;
        this._inProjectPathElements = inProjectPathElements;
    }

    private string? FormatFileNameLabel(string fullFileName, IProjectWrapper projectWrapper)
    {
        return GeneralOptionsModel.Instance.FileLabelStyle switch
        {
            FileLabel.AbsolutePath => fullFileName,
            FileLabel.RelativePathInProject => GetRelativePathToProject(this.FilePath ?? "", projectWrapper),
            FileLabel.RelativePathInSolution => GetRelativePathToSolution(this.FilePath),
            FileLabel.FileName => Path.GetFileName(fullFileName),
            _ => throw new ArgumentOutOfRangeException()
        };
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

    private static string? GetRelativePathToProject(string? path, IProjectWrapper? project)
    {
        if (path == null)
        {
            return null;
        }

        return project == null ? path : GetRelativePath(path, project.DirectoryPath ?? "");
    }

    private static string GetRelativePath(string filePath, string parentDir)
    {
        return !filePath.StartsWith(parentDir, StringComparison.OrdinalIgnoreCase)
            ? filePath
            : filePath.Substring(parentDir.Length);
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

    public string? FilePath
    {
        get => (string?)this.GetValue(FilePathProperty);
        set => this.SetValue(FilePathProperty, value!);
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

    private void OnSomethingAboutDocumentNameChanged()
    {
        this.UpdateAsync(true).FireAndForget();
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