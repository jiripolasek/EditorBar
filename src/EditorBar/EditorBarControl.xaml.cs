// ------------------------------------------------------------
//
// Copyright (c) Jiří Polášek. All rights reserved.
//
// ------------------------------------------------------------

using System.Diagnostics;
using System.IO;
using System.Windows;
using System.Windows.Media;
using Community.VisualStudio.Toolkit;
using JPSoftworks.EditorBar.Helpers;
using JPSoftworks.EditorBar.Options;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Text.Editor;

namespace JPSoftworks.EditorBar;

/// <summary>
/// Interaction logic for EditorBarControl.xaml
/// </summary>
public partial class EditorBarControl
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

    public static readonly DependencyProperty ShowSolutionFoldersProperty = DependencyProperty.Register(
        nameof(ShowSolutionFolders), typeof(bool), typeof(EditorBarControl), new PropertyMetadata(default(bool)));

    public static readonly DependencyProperty ShowSolutionRootProperty = DependencyProperty.Register(
        nameof(ShowSolutionRoot), typeof(bool), typeof(EditorBarControl), new PropertyMetadata(default(bool)));

    private readonly IWpfTextView? _textView;

    public EditorBarControl(IWpfTextView textView)
    {
        this.InitializeComponent();

        this._textView = textView;
        this.Loaded += (_, _) =>
        {
            this.OnSomethingAboutDocumentNameChanged();
            this.OnSettingsChanged();
        };
        this.IsVisibleChanged += this.OnIsVisibleChanged;

        GeneralPage.Saved += _ => this.OnSettingsChanged();

        VS.Events.ProjectItemsEvents.AfterRenameProjectItems += _ => this.OnSomethingAboutDocumentNameChanged();
        VS.Events.ProjectItemsEvents.AfterAddProjectItems += _ => this.OnSomethingAboutDocumentNameChanged();
        VS.Events.ProjectItemsEvents.AfterRemoveProjectItems += _ => this.OnSomethingAboutDocumentNameChanged();
        VS.Events.DocumentEvents.Saved += _ => this.OnSomethingAboutDocumentNameChanged();
        VS.Events.SolutionEvents.OnAfterRenameProject += _ => this.OnSomethingAboutDocumentNameChanged();
        VS.Events.SolutionEvents.OnAfterOpenSolution += _ => this.OnSomethingAboutDocumentNameChanged();
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

    private string? FilePath { get; set; }

    private string? RelativePath { get; set; }

    private void OnSomethingAboutDocumentNameChanged()
    {
        this.UpdateAsync(true).FireAndForget();
    }

    private void OnIsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
    {
        if ((bool)e.NewValue)
        {
            this.UpdateAsync().FireAndForget();
        }
    }

    private void OnSettingsChanged()
    {
        this.UpdateAsync(true).FireAndForget();
        this.ReapplySettings();
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
        this.UpdateFilePathLabel(document, forced);
        var project = await VisualStudioHelper.GetProjectFromDocumentAsync(document);
        this.ProjectNameLabel!.Content = project?.Name ?? "(no project)";
        if (project != null)
        {
            var parents =
                await VisualStudioHelper.GetSolutionFolderPathAsync(
                    await VisualStudioHelper.ConvertToSolutionItemAsync(project));
            this.SolutionFoldersList!.ItemsSource = parents;
        }
        else
        {
            var allProjects = await VS.Solutions.GetAllProjectsAsync(ProjectStateFilter.All);
            var project2 = allProjects.FirstOrDefault(t => t.FullPath == document.FilePath);
            if (project2 != null)
            {
                this.ProjectNameLabel.Content = project2.Name;
                var parents = await VisualStudioHelper.GetSolutionFolderPathAsync(project2);
                this.SolutionFoldersList!.ItemsSource = parents;
            }
        }
    }

    private void ReapplySettings()
    {
        this.SolutionElementBackground = new SolidColorBrush(GeneralPage.Instance.SolutionBackground.ToMediaColor());
        this.SolutionElementForeground = new SolidColorBrush(GeneralPage.Instance.SolutionForeground.ToMediaColor());

        this.ProjectElementBackground = new SolidColorBrush(GeneralPage.Instance.ProjectBackground.ToMediaColor());
        this.ProjectElementForeground = new SolidColorBrush(GeneralPage.Instance.ProjectForeground.ToMediaColor());

        this.SolutionFolderElementBackground = new SolidColorBrush(GeneralPage.Instance.SolutionFolderBackground.ToMediaColor());
        this.SolutionFolderElementForeground = new SolidColorBrush(GeneralPage.Instance.SolutionFolderForeground.ToMediaColor());

        this.ShowSolutionFolders = GeneralPage.Instance.ShowSolutionFolders;
        this.ShowSolutionRoot = GeneralPage.Instance.ShowSolutionRoot;

        this.ReloadStyle();
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
        var newStyleUri = new Uri($"pack://application:,,,/EditorBar;component/Styles/{GeneralPage.Instance.DisplayStyle}Style.xaml");
        var newResourceDict = new ResourceDictionary { Source = newStyleUri };
        this.Resources.MergedDictionaries.Add(newResourceDict);
        return;

        static bool IsEditorBarStyleXamlComponent(ResourceDictionary resourceDictionary) =>
            resourceDictionary.Source != null
            && resourceDictionary.Source.IsAbsoluteUri
            && resourceDictionary.Source.AbsolutePath.Contains("/EditorBar;")
            && resourceDictionary.Source.AbsolutePath.Contains("Style.xaml");
    }

    private void UpdateFilePathLabel(ITextDocument document, bool forced = false)
    {
        if (!forced && string.Equals(this.FilePath!, document.FilePath!, StringComparison.Ordinal))
        {
            return;
        }

        this.FilePath = document.FilePath;
        this.RelativePath = this.GetRelativePathToSolution(this.FilePath);

        var pathLabelText = GeneralPage.Instance.ShowPathRelativeToSolutionRoot ? this.RelativePath : this.FilePath;
        this.PathLabel!.Content = pathLabelText ?? "(unnamed document)";
    }

    private string? GetRelativePathToSolution(string? path)
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
        this._textView?.TextDataModel?.DocumentBuffer?.Properties?.TryGetProperty(typeof(ITextDocument), out document);
        return document;
    }

    private void OpenFolderClicked(object sender, RoutedEventArgs e)
    {
        var fileName = this.FilePath;
        if (!StringHelper.IsNullOrWhiteSpace(fileName))
        {
            var directoryName = Path.GetDirectoryName(fileName);
            if (!StringHelper.IsNullOrWhiteSpace(directoryName!) && Directory.Exists(directoryName))
            {
                Process.Start(new ProcessStartInfo("explorer.exe", "/select, " + fileName) { UseShellExecute = true });
            }
        }
    }

    private void SettingsClicked(object sender, RoutedEventArgs e)
    {
        VS.Settings.OpenAsync<OptionsProvider.GeneralPageOptions>().FireAndForget();
    }

    private void CopyPathClicked(object sender, RoutedEventArgs e)
    {
        var filePath = this.FilePath;
        if (!StringHelper.IsNullOrWhiteSpace(filePath))
        {
            Clipboard.SetText(filePath, TextDataFormat.UnicodeText);
        }
    }

    private void CopyRelativePathClicked(object sender, RoutedEventArgs e)
    {
        var filePath = this.RelativePath;
        if (!StringHelper.IsNullOrWhiteSpace(filePath))
        {
            Clipboard.SetText(filePath, TextDataFormat.UnicodeText);
        }
    }
}