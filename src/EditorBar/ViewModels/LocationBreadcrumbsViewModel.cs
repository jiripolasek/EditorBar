// ------------------------------------------------------------
// 
// Copyright (c) Jiří Polášek. All rights reserved.
// 
// ------------------------------------------------------------

#nullable enable

using System.Reactive;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Threading;
using System.Windows;
using System.Windows.Media;
using Community.VisualStudio.Toolkit;
using JPSoftworks.EditorBar.Commands;
using JPSoftworks.EditorBar.Commands.Abstractions;
using JPSoftworks.EditorBar.Helpers;
using JPSoftworks.EditorBar.Helpers.Events;
using JPSoftworks.EditorBar.Helpers.Events.Abstractions;
using JPSoftworks.EditorBar.Helpers.Presentation;
using JPSoftworks.EditorBar.Options;
using JPSoftworks.EditorBar.Resources;
using JPSoftworks.EditorBar.Services;
using JPSoftworks.EditorBar.Services.LocationProviders;
using Microsoft;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Text;
using Microsoft.IO;
using Microsoft.VisualStudio.Language.Intellisense;
using Microsoft.VisualStudio.PlatformUI;
using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Text.Editor;
using Microsoft.VisualStudio.Threading;

namespace JPSoftworks.EditorBar.ViewModels;

internal class LocationBreadcrumbsViewModel : ObservableObject, IDisposable
{
    private readonly CompositeDisposable _disposables = [];

    private readonly JoinableTaskFactory _joinableTaskFactory;
    private readonly LocationBreadcrumbEventAggregator _locationBreadcrumbEventAggregator;
    private readonly DispatchedDelegateCommand _openMenuOnPhysicalCrumb;
    private readonly DispatchedDelegateCommand _openMenuOnProjectCrumb;
    private readonly EditorBarViewModel _parent;

    private readonly DispatchedDelegateCommand _switchProjectCommand;

    private readonly IWpfTextView _textView;
    private readonly IWorkspaceMonitor _workspaceMonitor;
    private readonly SettingsRefreshAggregator _settingsRefreshAggregator;

    public BulkObservableCollection<BreadcrumbModel> LocationBreadcrumbs { get; } = [];

    public LegacyLabelViewModel LegacyLabelModel { get; }

    public LocationBreadcrumbsViewModel(
        EditorBarViewModel parent,
        IWorkspaceMonitor workspaceMonitor,
        ITextDocument textDocument,
        IWpfTextView textView,
        JoinableTaskFactory joinableTaskFactory,
        SettingsRefreshAggregator settingsRefreshAggregator)
    {
        this._parent = Requires.NotNull(parent, nameof(parent));
        this._textView = Requires.NotNull(textView, nameof(textView));
        Requires.NotNull(textDocument, nameof(textDocument));
        this._joinableTaskFactory = Requires.NotNull(joinableTaskFactory, nameof(joinableTaskFactory));
        this._workspaceMonitor = Requires.NotNull(workspaceMonitor, nameof(workspaceMonitor));
        this._settingsRefreshAggregator = settingsRefreshAggregator;
        
        this.LegacyLabelModel = new LegacyLabelViewModel(textDocument);

        this._settingsRefreshAggregator.SettingsRefreshRequested += this.HandleSettingsChanged;

        var solutionProjectChangeTracker = new SolutionProjectChangeEventAggregator();
        solutionProjectChangeTracker.AddTo(this._disposables);

        this._locationBreadcrumbEventAggregator =
            new LocationBreadcrumbEventAggregator(textDocument, this._workspaceMonitor, solutionProjectChangeTracker);
        this._locationBreadcrumbEventAggregator.AddTo(this._disposables);

        CreateObservableForLocationChanges()
         .ObserveOnDispatcher()
         .Subscribe()
         .AddTo(this._disposables);

        this._switchProjectCommand = new DispatchedDelegateCommand(this.SwitchProject);
        this._openMenuOnProjectCrumb = new DispatchedDelegateCommand(this.OpenContextMenuOnProjectCrumb);
        this._openMenuOnPhysicalCrumb = new DispatchedDelegateCommand(this.OpenContextMenuOnPhysicalBreadcrumb);

        return;

        IObservable<Unit> CreateObservableForLocationChanges()
        {
            var locationInfoChanges = Observable.FromEventPattern(
                    this._locationBreadcrumbEventAggregator,
                    nameof(this._locationBreadcrumbEventAggregator.RefreshRequested))
                .Replay(1)
                .RefCount();

            var throttled = locationInfoChanges.ThrottleFirst(TimeSpan.FromMilliseconds(500));
            var debounced = locationInfoChanges.Throttle(TimeSpan.FromMilliseconds(50));

            return Observable
                .CombineLatest(throttled, debounced)
                .DistinctUntilChanged()
                .PausableBuffered(this._parent.SuspendedChanged)
                .Select(async _ =>
                {
                    await this.UpdateLocationAsync();
                    return Unit.Default;
                })
                .Switch();
        }
    }

    public async Task InitializeAsync()
    {
        await this.UpdateLocationAsync();
    }

    public void Dispose()
    {
        this._settingsRefreshAggregator.Dispose();
        this._disposables.Dispose();
    }

    private void HandleSettingsChanged(object sender, SettingsRefreshEventArgs e)
    {
        this.UpdateLocationAsync().Forget();
    }

    private async Task UpdateLocationAsync()
    {
        if (this._parent.IsUpdateSuspended)
        {
            return;
        }

        var locationProvider = await VS.GetRequiredServiceAsync<ILocationProvider, ILocationProvider>();
        var locationModel = await locationProvider.CreateAsync(this._textView, CancellationToken.None);

        try
        {
            var currentWorkspace = this._workspaceMonitor.CurrentWorkspace;
            if (currentWorkspace != null)
            {
                // run only on file we know are part of a project
                if (locationModel?.Project is GenericProjectInfo projectWrapper)
                {
                    var projects = this._textView.TextBuffer?.AsTextContainer()
                        .GetProjectsAssociatedWithDocument(currentWorkspace);

                    if (projects?.AlternativeContextDocuments is { Count: > 1 })
                    {
                        projectWrapper.UpdateIntelliSenseContext(projects.Value);
                    }
                }
            }
        }
        catch (Exception ex)
        {
            await ex.LogAsync();
        }

        try
        {
            await this.UpdateFilePathLabelAsync(locationModel);
        }
        catch (Exception ex)
        {
            await ex.LogAsync();
        }

        try
        {
            var locationBreadcrumbs = locationModel == null
                ? []
                : this.RebuildLocationBreadcrumbs(GeneralOptionsModel.Instance, locationModel);
            this.LocationBreadcrumbs.SetRange(locationBreadcrumbs);
        }
        catch (Exception ex)
        {
            await ex.LogAsync();
        }
    }

    private async Task UpdateFilePathLabelAsync(LocationNavModel? locationModel)
    {
        await this._joinableTaskFactory.SwitchToMainThreadAsync();
        this.LegacyLabelModel.LocationNavModel = locationModel;
    }

    private List<BreadcrumbModel> RebuildLocationBreadcrumbs(
        GeneralOptionsModel options,
        LocationNavModel locationModel)
    {
        Requires.NotNull(options, nameof(options));
        Requires.NotNull(locationModel, nameof(locationModel));

        var project = locationModel.Project;

        // Rebuild breadcrumbs
        var breadcrumbs = new List<BreadcrumbModel>();

        // 1) Root
        if (options.ShowSolutionRoot)
        {
            var crumb = project switch
            {
                BaseSolutionProjectInfo s => new BreadcrumbModel(
                    "\\",
                    options.SolutionBackground,
                    options.SolutionForeground)
                { AssociatedFile = s.DirectoryPath },

                FileSystemProjectInfo fsProject => new BreadcrumbModel(
                    fsProject.DisplayName,
                    options.NonSolutionRootBackground,
                    options.NonSolutionRootForeground)
                { AssociatedDirectory = fsProject.DirectoryPath },

                NullProjectInfo => new BreadcrumbModel(
                    Strings.CodeFragment!,
                    options.NonSolutionRootBackground,
                    options.NonSolutionRootForeground),

                _ => throw new InvalidOperationException("Invalid project type")
            };
            breadcrumbs.Add(crumb);
        }

        // 2) Solution folders
        if (options.ShowSolutionFolders && project is IHasSolutionFolders hasSolutionFolders)
        {
            breadcrumbs.AddRange(hasSolutionFolders.SolutionFolders.Select(folder =>
                new BreadcrumbModel(folder, options.SolutionFolderBackground, options.SolutionFolderForeground)));
        }

        // 3) Project
        if (options.ShowProject)
        {
            if (!project.ImplicitProject)
            {
                var crumb = project switch
                {
                    GenericProjectInfo { IntelliSenseAlternativeContextsDocuments.Count: > 1 } p => new
                        ProjectContainerBreadcrumbModel(p, p.DisplayName, options.ProjectBackground,
                            options.ProjectForeground)
                    {
                        AssociatedDirectory = p.DirectoryPath,
                        ItemsProvider = () => Task.FromResult<IList<MemberListItemViewModel>>(
                            p.IntelliSenseAlternativeContextsDocuments
                                .Select(alternativeContextDocument =>
                                    new MemberListItemViewModel
                                    {
                                        PrimaryName = alternativeContextDocument.Project.Name,
                                        SearchText = alternativeContextDocument.Project.Name,
                                        Command = this._switchProjectCommand,
                                        CommandParameter = alternativeContextDocument
                                    })
                                .OrderBy(static t => t.PrimaryName)
                                .ToList()),
                        ContextCommand = this._openMenuOnProjectCrumb
                    },

                    BaseSolutionProjectInfo => new ProjectContainerBreadcrumbModel(project, project.DisplayName,
                        options.ProjectBackground, options.ProjectForeground)
                    {
                        AssociatedDirectory = project.DirectoryPath,
                        ItemsProvider = null,
                        ContextCommand = this._openMenuOnProjectCrumb
                    },

                    _ => null
                };

                if (crumb != null)
                {
                    breadcrumbs.Add(crumb);
                }
            }
        }


        // 4) project folders + parent folder
        if (options.ShowProjectFolders && locationModel.ProjectFolders.Length > 0)
        {
            var partialPath = locationModel.Project.DirectoryPath ?? "";

            var projectFoldersBackground = new SolidColorBrush(options.ProjectFoldersBackground.ToMediaColor());
            projectFoldersBackground.Freeze();

            var projectFoldersForeground = new SolidColorBrush(options.ProjectFoldersForeground.ToMediaColor());
            projectFoldersForeground.Freeze();

            // show in-project path part, if there's more that two elements; skip the last element, if that is the parent folder that is displayed separately
            var endIndex = options.ShowParentFolder
                ? locationModel.ProjectFolders.Length - 1
                : locationModel.ProjectFolders.Length;
            for (var i = 0; i < endIndex; i++)
            {
                var pathSegment = locationModel.ProjectFolders[i];
                partialPath = Path.Combine(partialPath, pathSegment);
                var model = new PhysicalDirectoryModel { FullPath = partialPath, Name = pathSegment };
                breadcrumbs.Add(
                    new PhysicalDirectoryBreadcrumbModel(model, pathSegment, projectFoldersBackground,
                        projectFoldersForeground)
                    { ContextCommand = this._openMenuOnPhysicalCrumb });
            }
        }

        // 5) immediate parent folder (if enabled explicitly, to be displayed as a breadcrumb with a different color or without 
        if (options.ShowParentFolder && locationModel.ProjectFolders.Length > 0)
        {
            var parentFolder = Path.GetDirectoryName(locationModel.FilePath)!;
            var parentFolderName = Path.GetFileName(parentFolder);

            var model = new PhysicalDirectoryModel { FullPath = parentFolder, Name = parentFolderName };
            breadcrumbs.Add(
                new PhysicalDirectoryBreadcrumbModel(model, locationModel.ProjectFolders.Last(),
                    options.ParentFolderBackground, options.ParentFolderForeground)
                {
                    ContextCommand = this._openMenuOnPhysicalCrumb
                });
        }

        for (var index = 0; index < breadcrumbs.Count; index++)
        {
            var breadcrumbModel = breadcrumbs[index]!;
            // set IsFirst, IsLast, PreviousBreadcrumb and LastBreadcrumb
            breadcrumbModel.IsFirst = index == 0;
            breadcrumbModel.IsLast = index == breadcrumbs.Count - 1;
            breadcrumbModel.PreviousBreadcrumb = index > 0 ? breadcrumbs[index - 1] : null;
            breadcrumbModel.NextBreadcrumb = index < breadcrumbs.Count - 1 ? breadcrumbs[index + 1] : null;
        }

        return breadcrumbs;
    }


    private void SwitchProject(object parameter)
    {
        var currentWorkspace = this._workspaceMonitor.CurrentWorkspace;
        if (currentWorkspace != null && parameter is Document document)
        {
            currentWorkspace.SetDocumentContextHack(document.Id);
        }
    }

    private void OpenContextMenuOnPhysicalBreadcrumb(object obj)
    {
        var breadcrumbModel = (obj as FrameworkElement)?.DataContext as PhysicalDirectoryBreadcrumbModel;
        if (breadcrumbModel?.Model == null)
        {
            return;
        }

        new LocationBreadcrumbMenuContext(breadcrumbModel.Model, this._textView).ShowMenu();
    }

    private void OpenContextMenuOnProjectCrumb(object obj)
    {
        var breadcrumbModel = (obj as FrameworkElement)?.DataContext as ProjectContainerBreadcrumbModel;
        if (breadcrumbModel?.Model == null)
        {
            return;
        }

        new LocationBreadcrumbMenuContext(breadcrumbModel.Model, this._textView).ShowMenu();
    }
}