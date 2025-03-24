// ------------------------------------------------------------
// 
// Copyright (c) Jiří Polášek. All rights reserved.
// 
// ------------------------------------------------------------

#nullable enable

using Community.VisualStudio.Toolkit;
using JPSoftworks.EditorBar.Commands;
using JPSoftworks.EditorBar.Commands.Abstractions;
using JPSoftworks.EditorBar.Helpers;
using JPSoftworks.EditorBar.Options;
using JPSoftworks.EditorBar.Services.LocationProviders;
using Microsoft;
using Microsoft.IO;
using Microsoft.VisualStudio.PlatformUI;
using Microsoft.VisualStudio.Text;

namespace JPSoftworks.EditorBar.ViewModels;

internal class LegacyLabelViewModel : ObservableObject
{
    private string? _label;
    private LocationNavModel? _locationNavModel;
    private FileLabel _style;

    public DispatchedDelegateCommand PrimaryCommand { get; }
    public DispatchedDelegateCommand SecondaryCommand { get; }
    public DispatchedDelegateCommand ContextMenuCommand { get; }

    public LocationNavModel? LocationNavModel
    {
        get => this._locationNavModel;
        set
        {
            if (this.SetProperty(ref this._locationNavModel, value))
            {
                this.UpdateLabel();
            }
        }
    }

    public string Label
    {
        get => this._label ?? "";
        private set => this.SetProperty(ref this._label, value);
    }

    public FileLabel Style
    {
        get => this._style;
        set
        {
            if (this.SetProperty(ref this._style, value))
            {
                this.UpdateLabel();
            }
        }
    }

    public LegacyLabelViewModel(ITextDocument textDocument)
    {
        Requires.NotNull(textDocument, nameof(textDocument));

        this.PrimaryCommand = new(_ => this.ExecuteFileAction(false));
        this.SecondaryCommand = new(_ => this.ExecuteFileAction(true));
        this.ContextMenuCommand = new(_ => new FileActionMenuContext(textDocument).ShowMenu());

        GeneralOptionsModel.Saved += this.GeneralOptionsModelOnSaved;
        this.Style = GeneralOptionsModel.Instance.FileLabelStyle;
    }

    private void GeneralOptionsModelOnSaved(GeneralOptionsModel obj)
    {
        this.Style = obj.FileLabelStyle;
    }

    private void UpdateLabel()
    {
        this.Label = FormatFileNameLabel(this.LocationNavModel?.FilePath, this.LocationNavModel?.Project) ?? "";
    }

    private static string? FormatFileNameLabel(string? fullFileName, IProjectInfo? projectInfo)
    {
        if (string.IsNullOrWhiteSpace(fullFileName!))
        {
            return null;
        }

        return GeneralOptionsModel.Instance.FileLabelStyle switch
        {
            FileLabel.AbsolutePath => fullFileName,
            FileLabel.RelativePathInProject => GetRelativePathToProject(fullFileName ?? "", projectInfo),
            FileLabel.RelativePathInSolution => GetRelativePathToSolution(fullFileName),
            FileLabel.FileName => Path.GetFileName(fullFileName),
            FileLabel.Hidden => null,
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
        return slnDir == null ? path : PathUtils.GetRelativePath(slnDir, path);
    }

    private static string? GetRelativePathToProject(string? path, IProjectInfo? project)
    {
        return path == null || project == null ? path : PathUtils.GetRelativePath(project.DirectoryPath ?? "", path);
    }

    private void ExecuteFileAction(bool isAlternative)
    {
        if (this.LocationNavModel == null)
        {
            return;
        }

        var action = isAlternative
            ? GeneralOptionsModel.Instance.AlternateFileAction
            : GeneralOptionsModel.Instance.FileAction;

        switch (action)
        {
            case FileAction.None:
                break;
            case FileAction.OpenContainingFolder:
                Launcher.OpenContaingFolder(this.LocationNavModel.FilePath);
                break;
            case FileAction.OpenInExternalEditor:
                Launcher.OpenInExternalEditor(this.LocationNavModel.FilePath);
                break;
            case FileAction.OpenInDefaultEditor:
                Launcher.OpenInDefaultEditor(this.LocationNavModel.FilePath);
                break;
            case FileAction.CopyRelativePath:
                Launcher.CopyRelativePathFromFullPath(this.LocationNavModel.FilePath);
                break;
            case FileAction.CopyAbsolutePath:
                Launcher.CopyAbsolutePath(this.LocationNavModel.FilePath);
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
    }
}