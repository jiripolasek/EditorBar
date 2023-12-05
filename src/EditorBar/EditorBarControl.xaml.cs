// ------------------------------------------------------------
//
// Copyright (c) Jiří Polášek. All rights reserved.
//
// ------------------------------------------------------------

using System.Diagnostics;
using System.IO;
using System.Windows;
using Community.VisualStudio.Toolkit;
using JPSoftworks.EditorBar.Options;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Text.Editor;

namespace JPSoftworks.EditorBar;

/// <summary>
///     Interaction logic for EditorBarControl.xaml
/// </summary>
public partial class EditorBarControl : IWpfTextViewMargin
{
    private readonly IWpfTextView? _textView;

    public string? FilePath { get; private set; }

    public string? RelativePath { get; private set; }

    public EditorBarControl(IWpfTextView textView)
    {
        _textView = textView;

        InitializeComponent();

        Loaded += (_, _) => UpdateAsync().FireAndForget();
        GeneralPage.Saved += _ => UpdateAsync().FireAndForget();
        VS.Events.ProjectItemsEvents.AfterRenameProjectItems += _ => UpdateAsync().FireAndForget();
        VS.Events.ProjectItemsEvents.AfterAddProjectItems += _ => UpdateAsync().FireAndForget();
        VS.Events.ProjectItemsEvents.AfterRemoveProjectItems += _ => UpdateAsync().FireAndForget();
        VS.Events.DocumentEvents.Saved += _ => UpdateAsync().FireAndForget();
        VS.Events.SolutionEvents.OnAfterRenameProject += _ => UpdateAsync().FireAndForget();
        VS.Events.SolutionEvents.OnAfterOpenSolution += _ => UpdateAsync().FireAndForget();
    }

    public void Dispose()
    {
        // TODO: handle margin position change
        //if (this.Parent is Panel parentPanel)
        //{
        //    parentPanel.Children.Remove(this);
        //}
    }

    public ITextViewMargin? GetTextViewMargin(string marginName)
    {
        return !string.Equals(marginName, nameof(EditorBarControl), StringComparison.OrdinalIgnoreCase)
            ? null
            : this as ITextViewMargin;
    }


    public double MarginSize => ActualHeight;

    public bool Enabled => true;

    public FrameworkElement VisualElement => this;

    private async Task UpdateAsync()
    {
        var document = GetCurrentDocument();
        if (document == null)
        {
            return;
        }

        await ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync();

        document.FileActionOccurred -= DocumentOnFileActionOccurred;
        document.FileActionOccurred += DocumentOnFileActionOccurred;
        UpdateFilePathLabel(document);
        var project = await VisualStudioHelper.GetProjectFromDocumentAsync(document);
        ProjectNameLabel.Content = project == null ? "(no project)" : project.Name;
        if (project != null)
        {
            var parents = await VisualStudioHelper.GetSolutionFolderPathAsync(await VisualStudioHelper.ConvertToSolutionItemAsync(project));
            SolutionFoldersList.ItemsSource = parents;
        }
        else
        {
            var allProjects = await VS.Solutions.GetAllProjectsAsync(ProjectStateFilter.All);
            var project2 = allProjects.FirstOrDefault(t => t.FullPath == document.FilePath);
            if (project2 != null)
            {
                ProjectNameLabel.Content = project2.Name;
                var parents = await VisualStudioHelper.GetSolutionFolderPathAsync(project2);
                SolutionFoldersList.ItemsSource = parents;
            }
        }
    }

    private void UpdateFilePathLabel(ITextDocument document)
    {
        FilePath = document.FilePath;

        var currentSolution = VS.Solutions.GetCurrentSolution();
        if (currentSolution != null)
        {
            var slnPath = currentSolution.FullPath;
            var slnDir = Path.GetDirectoryName(slnPath);
            RelativePath = slnDir == null ? FilePath : GetRelativePath(FilePath, slnDir);
        }
        else
        {
            RelativePath = FilePath;
        }

        PathLabel.Content = GeneralPage.Instance.ShowPathRelativeToSolutionRoot
            ? RelativePath
            : FilePath;
    }

    private string GetRelativePath(string documentFilePath, string slnDir)
    {
        if (!documentFilePath.StartsWith(slnDir, StringComparison.OrdinalIgnoreCase))
        {
            return documentFilePath;
        }

        return documentFilePath.Substring(slnDir.Length);
    }


    private async void DocumentOnFileActionOccurred(object sender, TextDocumentFileActionEventArgs e)
    {
        FilePath = e.FilePath;
        UpdateAsync().FireAndForget();
    }

    private ITextDocument? GetCurrentDocument()
    {
        if (_textView == null)
        {
            return null;
        }

        _textView.TextDataModel.DocumentBuffer.Properties.TryGetProperty<ITextDocument>(typeof(ITextDocument), out var document);
        return document;
    }

    private void OpenFolderClicked(object sender, RoutedEventArgs e)
    {
        var fileName = this.FilePath;
        if (!string.IsNullOrWhiteSpace(fileName))
        {
            var directoryName = Path.GetDirectoryName(fileName);
            if (!string.IsNullOrWhiteSpace(directoryName) && Directory.Exists(directoryName))
            {
                Process.Start(new ProcessStartInfo("explorer.exe", "/select, " + fileName) { UseShellExecute = true });
            }
        }
    }

    private async void SettingsClicked(object sender, RoutedEventArgs e)
    {
        await VS.Settings.OpenAsync<OptionsProvider.GeneralPageOptions>();
    }

    private void CopyPathClicked(object sender, RoutedEventArgs e)
    {
        var filePath = FilePath;
        if (!string.IsNullOrWhiteSpace(filePath))
        {
            Clipboard.SetText(filePath, TextDataFormat.UnicodeText);
        }
    }

    private void CopyRelativePathClicked(object sender, RoutedEventArgs e)
    {
        var filePath = RelativePath;
        if (!string.IsNullOrWhiteSpace(filePath))
        {
            Clipboard.SetText(filePath, TextDataFormat.UnicodeText);
        }
    }
}