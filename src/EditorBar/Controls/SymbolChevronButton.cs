// ------------------------------------------------------------
// 
// Copyright (c) Jiří Polášek. All rights reserved.
// 
// ------------------------------------------------------------

#nullable enable

using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Media;
using JPSoftworks.EditorBar.ViewModels;
using Microsoft.VisualStudio.Shell;

namespace JPSoftworks.EditorBar.Controls;

[TemplatePart(Name = PartButtonName, Type = typeof(Button))]
[TemplatePart(Name = PartPopupName, Type = typeof(MemberListPopup))]
public class SymbolChevronButton : ChevronButton, IDisposable
{
    private const string PartButtonName = "PART_Button";
    private const string PartPopupName = "PART_Popup";

    public static readonly DependencyProperty ModelsAccessorProperty = DependencyProperty.Register(
        nameof(ModelsAccessor), typeof(Func<Task<IList<MemberListItemViewModel>>>), typeof(SymbolChevronButton),
        new PropertyMetadata(null!));

    public static readonly DependencyProperty CommandProperty = DependencyProperty.Register(
        nameof(Command), typeof(ICommand), typeof(SymbolChevronButton), new PropertyMetadata(null!));

    public static readonly DependencyProperty CommandParameterProperty = DependencyProperty.Register(
        nameof(CommandParameter), typeof(object), typeof(SymbolChevronButton), new PropertyMetadata(null!));

    public static readonly DependencyProperty CustomBackgroundProperty = DependencyProperty.Register(
        nameof(CustomBackground), typeof(Brush), typeof(SymbolChevronButton), new PropertyMetadata(default(Brush)));

    public static readonly DependencyProperty CustomForegroundProperty = DependencyProperty.Register(
        nameof(CustomForeground), typeof(Brush), typeof(SymbolChevronButton), new PropertyMetadata(default(Brush)));

    private Button? _buttonElement;

    private MemberListPopup? _popup;

    public Brush CustomBackground
    {
        get => (Brush)this.GetValue(CustomBackgroundProperty);
        set => this.SetValue(CustomBackgroundProperty, value);
    }

    public Brush CustomForeground
    {
        get => (Brush)this.GetValue(CustomForegroundProperty);
        set => this.SetValue(CustomForegroundProperty, value);
    }

    public ICommand Command
    {
        get => (ICommand)this.GetValue(CommandProperty);
        set => this.SetValue(CommandProperty, value);
    }

    public object CommandParameter
    {
        get => (object)this.GetValue(CommandParameterProperty);
        set => this.SetValue(CommandParameterProperty, value);
    }

    public Func<Task<IList<MemberListItemViewModel>>>? ModelsAccessor
    {
        get => (Func<Task<IList<MemberListItemViewModel>>>?)this.GetValue(ModelsAccessorProperty);
        set => this.SetValue(ModelsAccessorProperty, value!);
    }

    static SymbolChevronButton()
    {
        DefaultStyleKeyProperty!.OverrideMetadata(typeof(SymbolChevronButton),
            new FrameworkPropertyMetadata(typeof(SymbolChevronButton)));
    }

    public void Dispose()
    {
        if (this._popup != null)
        {
            this._popup.IsOpen = false;
        }
    }

    protected void OnClick()
    {
        this.Command?.Execute(this.CommandParameter);
        this.ShowPopupAsync().FireAndForget();
    }

    public override void OnApplyTemplate()
    {
        base.OnApplyTemplate();
        this._popup = this.Template?.FindName(PartPopupName, this) as MemberListPopup;
        this._buttonElement = this.Template?.FindName(PartButtonName, this) as Button;
        this._buttonElement.Click += (sender, args) => this.OnClick();
        this._buttonElement.MouseRightButtonUp += this.ButtonElementOnMouseRightButtonUp;
    }

    private void ButtonElementOnMouseRightButtonUp(object sender, MouseButtonEventArgs e)
    {
        if (this.ContextCommand != null)
        {
            this.ContextCommand.Execute(this);
            e.Handled = true;
        }
    }

    private async Task ShowPopupAsync()
    {
        var members = await this.EvalMembersAsync();
        if (members.Count == 0)
        {
            return;
        }

        this.EnsurePopupIsCreated(members);
        this._popup.IsOpen = true;
    }

    private async Task<IList<MemberListItemViewModel>> EvalMembersAsync()
    {
        IList<MemberListItemViewModel> members = null!;
        if (this.ModelsAccessor != null)
        {
            members = await this.ModelsAccessor.Invoke();
        }

        members ??= [];
        return members;
    }

    private Popup? EnsurePopupIsCreated(IEnumerable<MemberListItemViewModel> members)
    {
        var memberList = new MemberList(members);
        memberList.ItemSelected += OnMemberListOnItemSelected;

        this._popup.Content = memberList;
        return this._popup;

        void OnMemberListOnItemSelected(object sender, EventArgs eventArgs)
        {
            var senderMemberList = (MemberList)sender;
            if (senderMemberList.ListBox!.SelectedItem is MemberListItemViewModel selectedItem)
            {
                if (this._popup is not null)
                {
                    this._popup.IsOpen = false;
                }

                selectedItem.Command?.Execute(selectedItem.CommandParameter!);
            }
        }
    }

    protected override void OnPreviewKeyDown(KeyEventArgs e)
    {
        base.OnPreviewKeyDown(e);
        if (e.Key == Key.Down && this._buttonElement != null && this._buttonElement.IsFocused &&
            this._popup?.IsOpen != true)
        {
            this.ShowPopupAsync().FireAndForget();
            ;
            e.Handled = true;
        }
    }

    public void FocusButton()
    {
        this._buttonElement?.Focus();
    }
}