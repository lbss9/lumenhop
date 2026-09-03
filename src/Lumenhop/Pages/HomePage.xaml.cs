using Lumenhop.Controls;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Input;

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
        ToolTipService.SetToolTip(StartAllButton, Loc.Get("Home_StartAll"));
        ToolTipService.SetToolTip(StopAllButton, Loc.Get("Home_StopAll"));
        RefreshChrome();
    }

    private void OnStartAll(object sender, RoutedEventArgs e) =>
        PingMonitor.Instance.SetAllEnabled(true);

    private void OnStopAll(object sender, RoutedEventArgs e) =>
        PingMonitor.Instance.SetAllEnabled(false);

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
