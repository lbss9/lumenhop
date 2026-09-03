using System.IO;
using Lumenhop.Controls;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Input;
using Windows.Storage.Pickers;
using WinRT.Interop;

namespace Lumenhop.Pages;

public sealed partial class HomePage : Page
{
    public HomePage()
    {
        InitializeComponent();
        TargetList.ItemsSource = PingMonitor.Instance.Targets;
        Loaded += OnLoaded;
        Unloaded += OnUnloaded;
    }

    private void OnLoaded(object sender, RoutedEventArgs e)
    {
        PingMonitor.Instance.Updated -= OnMonitorUpdated;
        PingMonitor.Instance.Updated += OnMonitorUpdated;
        ToolTipService.SetToolTip(TransferButton, Loc.Get("Home_Transfer"));
        RefreshChrome();
    }

    private void OnUnloaded(object sender, RoutedEventArgs e)
    {
        PingMonitor.Instance.Updated -= OnMonitorUpdated;
    }

    private void OnMonitorUpdated(object? sender, EventArgs e)
    {
        DispatcherQueue.TryEnqueue(RefreshChrome);
    }

    private void RefreshChrome()
    {
        var empty = PingMonitor.Instance.Targets.Count == 0;
        EmptyState.Visibility = empty ? Visibility.Visible : Visibility.Collapsed;
        TargetList.Visibility = empty ? Visibility.Collapsed : Visibility.Visible;
        SummaryText.Text = PingMonitor.Instance.Summary();
    }

    private void OnCardPointerEntered(object sender, PointerRoutedEventArgs e) =>
        SetMoreVisible(sender, true);

    private void OnCardPointerExited(object sender, PointerRoutedEventArgs e) =>
        SetMoreVisible(sender, false);

    private static void SetMoreVisible(object sender, bool visible)
    {
        if (sender is FrameworkElement card && card.FindName("MoreButton") is Button button)
            button.Opacity = visible ? 1 : 0;
    }

    private async void OnAdd(object sender, RoutedEventArgs e) => await ShowEditorAsync(null);

    private void OnTransferMenu(object sender, RoutedEventArgs e)
    {
        if (sender is not Button button)
            return;

        var flyout = new MenuFlyout();
        flyout.Items.Add(MenuItem(Loc.Get("Home_Export"), async () => await ExportAsync()));
        flyout.Items.Add(MenuItem(Loc.Get("Home_Import"), async () => await ImportAsync()));
        flyout.ShowAt(button);
    }

    private async Task ExportAsync()
    {
        if (PingMonitor.Instance.Targets.Count == 0)
        {
            await NoticeAsync(Loc.Get("Export_Empty"));
            return;
        }

        var picker = new FileSavePicker { SuggestedFileName = "lumenhop-destinos" };
        picker.FileTypeChoices.Add("Lumenhop", [TargetTransfer.FileExtension]);
        InitializeWithWindow.Initialize(picker, WindowNative.GetWindowHandle(App.Main));

        var file = await picker.PickSaveFileAsync();
        if (file is null)
            return;

        var json = TargetTransfer.Export(
            PingMonitor.Instance.Targets.Select(vm => vm.ToTarget()),
            AppInfo.Version
        );
        await File.WriteAllTextAsync(file.Path, json);
        await NoticeAsync(Loc.Get("Export_Done"));
    }

    private async Task ImportAsync()
    {
        var picker = new FileOpenPicker();
        picker.FileTypeFilter.Add(TargetTransfer.FileExtension);
        InitializeWithWindow.Initialize(picker, WindowNative.GetWindowHandle(App.Main));

        var file = await picker.PickSingleFileAsync();
        if (file is null)
            return;

        var content = await File.ReadAllTextAsync(file.Path);
        var result = TargetTransfer.Import(content);
        if (!result.Ok)
        {
            await NoticeAsync(Loc.Get("Import_Invalid"));
            return;
        }
        if (result.Targets.Count == 0)
        {
            await NoticeAsync(Loc.Get("Import_None"));
            return;
        }

        var confirm = new ContentDialog
        {
            XamlRoot = XamlRoot,
            Title = Loc.Get("Transfer_Title"),
            Content = string.Format(Loc.Get("Import_Body"), result.Targets.Count),
            PrimaryButtonText = Loc.Get("Import_Confirm"),
            CloseButtonText = Loc.Get("Editor_Cancel"),
            DefaultButton = ContentDialogButton.Primary,
        };
        if (await confirm.ShowAsync() != ContentDialogResult.Primary)
            return;

        var added = PingMonitor.Instance.ImportTargets(result.Targets);
        await NoticeAsync(string.Format(Loc.Get("Import_Added"), added));
    }

    private async Task NoticeAsync(string body)
    {
        var dialog = new ContentDialog
        {
            XamlRoot = XamlRoot,
            Title = Loc.Get("Transfer_Title"),
            Content = body,
            CloseButtonText = "OK",
        };
        await dialog.ShowAsync();
    }

    private void OnCardMenuClick(object sender, RoutedEventArgs e)
    {
        if (sender is not Button button || button.Tag is not PingTargetViewModel vm)
            return;

        BuildCardMenu(vm).ShowAt(button);
    }

    private MenuFlyout BuildCardMenu(PingTargetViewModel vm)
    {
        var flyout = new MenuFlyout();
        flyout.Items.Add(
            MenuItem(Loc.Get("Card_Edit"), async () => await ShowEditorAsync(vm.ToTarget()))
        );
        flyout.Items.Add(new MenuFlyoutSeparator());
        flyout.Items.Add(
            MenuItem(Loc.Get("Card_Remove"), async () => await ConfirmRemoveAsync(vm))
        );
        return flyout;
    }

    private static MenuFlyoutItem MenuItem(string text, Action action)
    {
        var item = new MenuFlyoutItem { Text = text };
        item.Click += (_, _) => action();
        return item;
    }

    private async Task ShowEditorAsync(PingTarget? existing)
    {
        var dialog = new TargetEditorDialog(existing) { XamlRoot = XamlRoot };
        var result = await dialog.ShowAsync();
        if (result != ContentDialogResult.Primary || dialog.Result is null)
            return;

        if (existing is null)
            PingMonitor.Instance.Add(dialog.Result);
        else
            PingMonitor.Instance.Update(dialog.Result);
    }

    private async Task ConfirmRemoveAsync(PingTargetViewModel vm)
    {
        var dialog = new ContentDialog
        {
            XamlRoot = XamlRoot,
            Title = Loc.Get("Card_RemoveTitle"),
            Content = string.Format(Loc.Get("Card_RemoveBody"), vm.Title),
            PrimaryButtonText = Loc.Get("Card_Remove"),
            CloseButtonText = Loc.Get("Editor_Cancel"),
            DefaultButton = ContentDialogButton.Close,
        };
        if (await dialog.ShowAsync() == ContentDialogResult.Primary)
            PingMonitor.Instance.Remove(vm.Id);
    }
}
