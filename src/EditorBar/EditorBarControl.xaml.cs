#nullable enable

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using Community.VisualStudio.Toolkit;
using EnvDTE;
using JPSoftworks.EditorBar.Options;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Text.Editor;
using Project = EnvDTE.Project;

namespace JPSoftworks.EditorBar;

/// <summary>
///     Interaction logic for EditorBarControl.xaml
/// </summary>
public partial class EditorBarControl : UserControl, IWpfTextViewMargin
{
    private readonly IWpfTextView? _textView;

    public string? FilePath { get; private set; }

    public string? RelativePath { get; private set; }

    public EditorBarControl(IWpfTextView textView)
    {
        _textView = textView;

        InitializeComponent();

        Loaded += async (_, _) => await UpdateAsync();
        GeneralPage.Saved += async _ => await UpdateAsync();
        VS.Events.ProjectItemsEvents.AfterRenameProjectItems += async _ => await UpdateAsync();
        VS.Events.ProjectItemsEvents.AfterAddProjectItems += async _ => await UpdateAsync();
        VS.Events.ProjectItemsEvents.AfterRemoveProjectItems += async _ => await UpdateAsync();
        VS.Events.DocumentEvents.Saved += async _ => await UpdateAsync();
        VS.Events.SolutionEvents.OnAfterRenameProject += async _ => await UpdateAsync();
        VS.Events.SolutionEvents.OnAfterOpenSolution += async _ => await UpdateAsync();
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
        var project = await GetProjectFromDocumentAsync(document);
        ProjectNameLabel.Content = project == null ? "(no project)" : project.Name;
        if (project != null)
        {
            var parents = await GetSolutionFolderPathAsync(await ConvertToSolutionItemAsync(project));
            SolutionFoldersList.ItemsSource = parents;
        }
        else
        {
            var allProjects = await VS.Solutions.GetAllProjectsAsync(ProjectStateFilter.All);
            var project2 = allProjects.FirstOrDefault(t => t.FullPath == document.FilePath);
            if (project2 != null)
            {
                ProjectNameLabel.Content = project2.Name;
                var parents = await GetSolutionFolderPathAsync(project2);
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
            RelativePath = GetRelativePath(FilePath, slnDir);
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

    private async Task<SolutionItem> ConvertToSolutionItemAsync(Project dteProject)
    {
        await ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync();
        var all = await VS.Solutions.GetAllProjectsAsync(ProjectStateFilter.All);
        return all.FirstOrDefault(t => t.FullPath == dteProject.FullName);
    }


    private async Task<Project?> GetProjectFromDocumentAsync(ITextDocument document)
    {
        await ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync();

        var dte = (DTE)Package.GetGlobalService(typeof(DTE));
        var projects = dte.Solution.Projects.OfType<Project>().ToList();
        var projectFile = projects.FirstOrDefault(t => string.Equals(t.FullName, document.FilePath, StringComparison.OrdinalIgnoreCase));
        if (projectFile != null)
        {
            return projectFile;
        }

        var projectItem = dte?.Solution.FindProjectItem(document.FilePath);
        return projectItem?.ContainingProject;
    }

    private async Task<List<string>> GetSolutionFolderPathAsync(SolutionItem? project)
    {
        await ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync();

        var folderPath = new List<string>();
        if (project == null)
        {
            return folderPath;
        }

        var parent = project.FindParent(SolutionItemType.SolutionFolder);
        while (parent != null)
        {
            folderPath.Add(parent.Name);
            parent = parent.FindParent(SolutionItemType.SolutionFolder);
        }

        // Reverse to get the order from solution to project
        folderPath.Reverse();
        return folderPath;
    }


    private async void DocumentOnFileActionOccurred(object sender, TextDocumentFileActionEventArgs e)
    {
        FilePath = e.FilePath;
        await UpdateAsync();
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
                System.Diagnostics.Process.Start(new ProcessStartInfo("explorer.exe", "/select, " + fileName) { UseShellExecute = true });
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