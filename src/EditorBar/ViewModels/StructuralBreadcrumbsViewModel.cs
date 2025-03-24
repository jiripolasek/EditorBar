// ------------------------------------------------------------
// 
// Copyright (c) Jiří Polášek. All rights reserved.
// 
// ------------------------------------------------------------

#nullable enable

using System.Globalization;
using System.Reactive;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Threading;
using System.Windows;
using JPSoftworks.EditorBar.Commands;
using JPSoftworks.EditorBar.Commands.Abstractions;
using JPSoftworks.EditorBar.Helpers;
using JPSoftworks.EditorBar.Helpers.Events;
using JPSoftworks.EditorBar.Helpers.Events.Abstractions;
using JPSoftworks.EditorBar.Options;
using JPSoftworks.EditorBar.Services;
using JPSoftworks.EditorBar.Services.StructureProviders;
using Microsoft;
using Microsoft.VisualStudio.Imaging.Interop;
using Microsoft.VisualStudio.Language.Intellisense;
using Microsoft.VisualStudio.PlatformUI;
using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Text.Editor;

namespace JPSoftworks.EditorBar.ViewModels;

internal class StructuralBreadcrumbsViewModel : ObservableObject, IDisposable
{
    private static readonly FileNameToImageMonikerConverter FileNameToImageMonikerConverter = new();

    private readonly BehaviorSubject<IObservable<BreadcrumbState>> _activeStructureProviderStream
        = SubjectFactory.CreateEmpty<BreadcrumbState>();

    private readonly CompositeDisposable _disposables = [];
    private readonly ITextDocument _document;
    private readonly IObservable<string> _documentFilePathChanged;
    private readonly IStructureProviderService _structureProviderService;
    private readonly IWpfTextView _textView;
    private readonly IWorkspaceMonitor _workspaceMonitor;

    private FileModel? _previousFileCrumb;
    private IReadOnlyList<BaseStructureModel>? _previousStructureModel;

    public BulkObservableCollection<BreadcrumbModel> StructuralBreadcrumbs { get; } = [];

    public StructuralBreadcrumbsViewModel(
        IWpfTextView textView,
        IWorkspaceMonitor workspaceMonitor,
        IStructureProviderService structureProviderService)
    {
        this._textView = Requires.NotNull(textView, nameof(textView));
        this._workspaceMonitor = Requires.NotNull(workspaceMonitor, nameof(workspaceMonitor));
        this._structureProviderService = Requires.NotNull(structureProviderService, nameof(structureProviderService));

        this._document = this._textView.GetTextDocumentFromDocumentBuffer()!;
        this._documentFilePathChanged = Observable
            .Defer(() => Observable.Return(this._document.FilePath))
            .Concat(Observable
                .FromEventPattern<TextDocumentFileActionEventArgs>(this._document,
                    nameof(this._document.FileActionOccurred))
                .Select(static e => e.EventArgs.FilePath))
            .DistinctUntilChanged();

        IStructureRefreshAggregator structureRefreshAggregator =
            new StructureRefreshAggregator(this._textView, this._workspaceMonitor);
        structureRefreshAggregator.AddTo(this._disposables);
        _ = Observable
            .FromEventPattern<EventArgs>(structureRefreshAggregator,
                nameof(IStructureRefreshAggregator.RefreshRequested))
            .Replay(1)
            .RefCount()
            .Subscribe(_ => this.UpdateFileStructureProvider())
            .AddTo(this._disposables);

        this._activeStructureProviderStream
            .Switch()
            .SelectMany(async tuple =>
            {
                await this.UpdateFileStructureAsync(tuple);
                return Unit.Default;
            })
            .Subscribe()
            .AddTo(this._disposables);

        this.UpdateFileStructureProvider();
    }

    public void Dispose()
    {
        this._disposables.Dispose();
    }

    private void UpdateFileStructureProvider()
    {
        if (this._textView.IsClosed)
        {
            return;
        }

        var currentProvider = this._textView.GetCurrentFileStructureProvider();
        var shouldShowBreadcrumbs = GeneralOptionsModel.Instance.ShowCodeStructureBreadcrumbs;
        var provider = shouldShowBreadcrumbs
            ? this._structureProviderService.CreateProvider(this._textView, this._workspaceMonitor.CurrentWorkspace)
            : null;

        // TODO: don't replace the provider if it's the same

        currentProvider?.Dispose();
        this._textView.SetCurrentFileStructureProvider(provider);

        var newBreadcrumbsSource = provider == null
            ? Observable.Empty<BreadcrumbState>()
            : Observable.CombineLatest(
                    provider.BreadcrumbsChanged,
                    this._documentFilePathChanged,
                    static (breadcrumbs, filePath) => new BreadcrumbState(breadcrumbs, filePath))
                .LogAndRetry();
        this._activeStructureProviderStream.OnNext(newBreadcrumbsSource);
    }

    private async Task UpdateFileStructureAsync(BreadcrumbState state)
    {
        if (this._textView.IsClosed)
        {
            return;
        }

        var fileCrumb = FileModel.Create(
            state.FilePath ?? "",
            state.Breadcrumbs?.CanRootHaveChildren ?? false);

        BreadcrumbModel[] structureBreadcrumbs;
        if (GeneralOptionsModel.Instance.ShowCodeStructureBreadcrumbs && state.Breadcrumbs != null)
        {
            var fileStructureModel = state.Breadcrumbs.Breadcrumbs;
            if (this._previousStructureModel?.SequenceEqual(fileStructureModel) == true &&
                fileCrumb != this._previousFileCrumb)
            {
                return;
            }

            this._previousStructureModel = fileStructureModel;
            structureBreadcrumbs = await this.RebuildStructureBreadcrumbsAsync([fileCrumb, .. fileStructureModel]);
        }
        else
        {
            this._previousStructureModel = null;
            structureBreadcrumbs = await this.RebuildStructureBreadcrumbsAsync([fileCrumb]);
        }

        this._previousFileCrumb = fileCrumb;
        this.StructuralBreadcrumbs.SetRange(structureBreadcrumbs);
    }

    private async Task<BreadcrumbModel[]> RebuildStructureBreadcrumbsAsync(
        IEnumerable<BaseStructureModel?> structureModels)
    {
        var options = await GeneralOptionsModel.GetLiveInstanceAsync();

        return structureModels
            .OfType<BaseStructureModel>()
            .Select(structureModel => this.BuildCrumb(structureModel, options))
            .ToArray();
    }

    private BreadcrumbModel BuildCrumb(BaseStructureModel baseStructureModel, GeneralOptionsModel options)
    {
        var crumb = baseStructureModel switch
        {
            FileModel fileModel => new StructureBreadcrumbViewModel(fileModel, fileModel.DisplayName,
                options.FileBreadcrumbBackground, options.FileBreadcrumbForeground)
            {
                ItemsProvider = fileModel.CanHaveChildren ? () => this.GetChildrenItemsAsync(fileModel) : null,
                ImageMoniker = GetMonikerForFile(fileModel),
                ContextCommand = new DispatchedDelegateCommand(this.OpenContextMenuOnFileCrumb)
            },
            TypeModel typeModel => new StructureBreadcrumbViewModel(typeModel, typeModel.DisplayName,
                options.StructureBreadcrumbBackground,
                options.StructureBreadcrumbForeground)
            {
                ItemsProvider = typeModel.CanHaveChildren ? () => this.GetChildrenItemsAsync(typeModel) : null,
                ContextCommand = new DispatchedDelegateCommand(this.OpenContextMenuOnTypeCrumb)
            },
            FunctionModel methodModel => new StructureBreadcrumbViewModel(methodModel, methodModel.DisplayName,
                options.StructureBreadcrumbBackground,
                options.StructureBreadcrumbForeground)
            {
                ContextCommand = new DispatchedDelegateCommand(this.OpenContextMenuOnMemberCrumb)
            },
            TypeMemberModel memberModel => new StructureBreadcrumbViewModel(memberModel, memberModel.DisplayName,
                options.StructureBreadcrumbBackground,
                options.StructureBreadcrumbForeground)
            {
                ContextCommand = new DispatchedDelegateCommand(this.OpenContextMenuOnMemberCrumb)
            },
            { } model => new StructureBreadcrumbViewModel(baseStructureModel, baseStructureModel.DisplayName,
                options.StructureBreadcrumbBackground,
                options.StructureBreadcrumbForeground)
            {
                ItemsProvider = model.CanHaveChildren ? () => this.GetChildrenItemsAsync(model) : null
            }
        };

        if (crumb.ImageMoniker.IsEmpty && crumb.Icon == null && !baseStructureModel.ImageMoniker.IsEmpty)
        {
            crumb.ImageMoniker = baseStructureModel.ImageMoniker;
        }

        return crumb;
    }

    private static ImageMoniker GetMonikerForFile(FileModel fileModel)
    {
        return (ImageMoniker?)FileNameToImageMonikerConverter.Convert(
            fileModel.AnchorPoint.FilePath,
            typeof(ImageMoniker),
            null,
            CultureInfo.CurrentCulture) ?? default;
    }

    private async Task<IList<MemberListItemViewModel>> GetChildrenItemsAsync(BaseStructureModel model)
    {
        var currentProvider = this._textView.GetCurrentFileStructureProvider();
        if (currentProvider == null)
        {
            return [];
        }

        var childItems = await currentProvider.GetChildItemsAsync(model, CancellationToken.None);

        var childItemViewModel = childItems
            .Select(t =>
            {
                var viewModel = MemberListItemViewModel.FromModel(t);
                viewModel.Command = new DispatchedDelegateCommand(_ => t.NavigationAction(this._textView));
                return viewModel;
            });

        return [.. childItemViewModel];
    }

    private void OpenContextMenuOnFileCrumb(object obj)
    {
        var doc = this._textView.GetTextDocumentFromDocumentBuffer();
        if (doc is null)
        {
            return;
        }

        new FileActionMenuContext(doc).ShowMenu();
    }

    private void OpenContextMenuOnMemberCrumb(object obj)
    {
        var breadcrumbModel = (obj as FrameworkElement)?.DataContext as StructureBreadcrumbViewModel;
        if (breadcrumbModel?.Model == null)
        {
            return;
        }

        new StructureBreadcrumbMenuContext(breadcrumbModel.Model, this._textView).ShowMenu();
    }

    private void OpenContextMenuOnTypeCrumb(object obj)
    {
        var breadcrumbModel = (obj as FrameworkElement)?.DataContext as StructureBreadcrumbViewModel;
        var typeModel = breadcrumbModel?.Model as TypeModel;
        if (typeModel == null)
        {
            return;
        }

        new StructureBreadcrumbMenuContext(typeModel, this._textView).ShowMenu();
    }

    private record struct BreadcrumbState(StructureNavModel? Breadcrumbs, string? FilePath);

    public Task InitializeAsync()
    {
        return Task.CompletedTask;
    }
}