// ------------------------------------------------------------
// 
// Copyright (c) Jiří Polášek. All rights reserved.
// 
// ------------------------------------------------------------

#nullable enable

using System.Reactive.Disposables;
using System.Reactive.Subjects;
using System.Text;
using System.Windows.Input;
using Community.VisualStudio.Toolkit;
using JPSoftworks.EditorBar.Helpers;
using JPSoftworks.EditorBar.Helpers.Events;
using JPSoftworks.EditorBar.Options;
using JPSoftworks.EditorBar.Services;
using JPSoftworks.EditorBar.Services.StructureProviders;
using Microsoft;
using Microsoft.VisualStudio.Language.Intellisense;
using Microsoft.VisualStudio.PlatformUI;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;
using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Text.Editor;
using Microsoft.VisualStudio.Threading;

namespace JPSoftworks.EditorBar.ViewModels;

internal class EditorBarViewModel : ObservableObject, IDisposable
{
    private readonly CompositeDisposable _disposables = [];

    private readonly IWpfTextView _textView;
    private readonly IWorkspaceMonitor _workspaceMonitor;
    internal readonly BehaviorSubject<bool> SuspendedChanged = new(true);

    private bool _isDevelopmentModeEnabled;
    private bool _isUpdateSuspended = true;

    public LocationBreadcrumbsViewModel LocationBreadcrumbs { get; }
    public StructuralBreadcrumbsViewModel StructuralBreadcrumbs { get; }
    public BulkObservableCollection<BreadcrumbModel> Breadcrumbs { get; } = [];

    public ICommand ShowDebugInformationCommand { get; }
    public ICommand OpenContainingFolderCommand { get; }
    public ICommand OpenSettingsCommand { get; }
    public ICommand OpenExternalEditorCommand { get; }
    public ICommand OpenDefaultEditorCommand { get; }

    public bool IsDevelopmentModeEnabled
    {
        get => this._isDevelopmentModeEnabled;
        set => this.SetProperty(ref this._isDevelopmentModeEnabled, value);
    }

    public bool IsUpdateSuspended
    {
        get => this._isUpdateSuspended;
        private set
        {
            if (this.SetProperty(ref this._isUpdateSuspended, value))
            {
                this.SuspendedChanged.OnNext(this._isUpdateSuspended);
            }
        }
    }

    public EditorBarViewModel(
        IWpfTextView textView,
        ITextDocument textDocument,
        JoinableTaskFactory joinableTaskFactory,
        IStructureProviderService structureProviderService)
    {
        this._textView = Requires.NotNull(textView, nameof(textView));
        Requires.NotNull(textDocument, nameof(textDocument));
        Requires.NotNull(joinableTaskFactory, nameof(joinableTaskFactory));
        Requires.NotNull(structureProviderService, nameof(structureProviderService));

        this._workspaceMonitor = new WorkspaceMonitor(this._textView);
        this._workspaceMonitor.AddTo(this._disposables);

        var settingsRefreshAggregator = new SettingsRefreshAggregator();
        settingsRefreshAggregator.AddTo(this._disposables);

        this.LocationBreadcrumbs = new LocationBreadcrumbsViewModel(
            this,
            this._workspaceMonitor,
            textDocument,
            textView,
            joinableTaskFactory,
            settingsRefreshAggregator);
        this.LocationBreadcrumbs.LocationBreadcrumbs.CollectionChanged
            += (_, _) => this.CombineBreadcrumbsAsync().FireAndForget();
        this.LocationBreadcrumbs.AddTo(this._disposables);

        this.StructuralBreadcrumbs =
            new StructuralBreadcrumbsViewModel(textView, this._workspaceMonitor, structureProviderService);
        this.StructuralBreadcrumbs.StructuralBreadcrumbs.CollectionChanged
            += (_, _) => this.CombineBreadcrumbsAsync().FireAndForget();
        this.StructuralBreadcrumbs.AddTo(this._disposables);

        this.ShowDebugInformationCommand = new DispatchedDelegateCommand(this.ExecuteShowDebugInfo);
        this.OpenContainingFolderCommand
            = new DispatchedDelegateCommand(_ => Launcher.OpenContaingFolder(textDocument.FilePath));
        this.OpenSettingsCommand
            = new DispatchedDelegateCommand(static _ => VS.Settings.OpenAsync<GeneralOptionPage>().FireAndForget());
        this.OpenExternalEditorCommand
            = new DispatchedDelegateCommand(_ => Launcher.OpenInExternalEditor(textDocument.FilePath));
        this.OpenDefaultEditorCommand
            = new DispatchedDelegateCommand(_ => Launcher.OpenInDefaultEditor(textDocument.FilePath));
    }

    public async Task InitializeAsync()
    {
        await this.LocationBreadcrumbs.InitializeAsync();
        await this.StructuralBreadcrumbs.InitializeAsync();
    }

    /// <inheritdoc />
    public void Dispose()
    {
        this._disposables.Dispose();
    }

    public void Suspend()
    {
        this.IsUpdateSuspended = true;
    }

    public async Task ResumeAsync()
    {
        this.IsUpdateSuspended = false;
        try
        {
            await GeneralOptionsModel.Instance.UpgradeAsync();
        }
        catch (Exception ex)
        {
            await ex.LogAsync();
        }

        RatingService.RegisterSuccessfulUsage();
    }

    private void ExecuteShowDebugInfo(object obj)
    {
        var sb = new StringBuilder();

        sb.AppendLine(this._textView.GetTextDocumentFromDocumentBuffer()?.FilePath ?? "(null file name)")
            .AppendLine();

        sb.AppendLine("--------------------------------");
        sb.AppendLine("Workspace: " + this._workspaceMonitor.CurrentWorkspace?.GetType());

        var roles = this._textView.Roles;
        if (roles != null)
        {
            sb.AppendLine("--------------------------------");
            sb.AppendLine("Roles:");
            foreach (var role in roles.OrderBy(static t => t))
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

    private async Task CombineBreadcrumbsAsync()
    {
        await ThreadHelper.JoinableTaskFactory!.SwitchToMainThreadAsync();
        BreadcrumbModel[] breadcrumbs = [
            .. this.LocationBreadcrumbs.LocationBreadcrumbs,
            .. this.StructuralBreadcrumbs.StructuralBreadcrumbs
        ];
        this.Breadcrumbs.SetRange(breadcrumbs);
    }
}