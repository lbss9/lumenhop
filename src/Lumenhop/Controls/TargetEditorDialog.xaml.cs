using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Media.Imaging;
using Windows.Storage.Pickers;
using WinRT.Interop;

namespace Lumenhop.Controls;

public sealed partial class TargetEditorDialog : ContentDialog
{
    private string _glyph = TargetIcons.DefaultGlyph;
    private string? _pendingImagePath;
    private readonly string _targetId;
    private readonly bool _isEnabled;

    public TargetEditorDialog(PingTarget? existing = null)
    {
        InitializeComponent();
        _targetId = PathGuard.SanitizeId(existing?.Id);
        _isEnabled = existing?.IsEnabled ?? true;
        Title = existing is null ? Loc.Get("Editor_NewTitle") : Loc.Get("Editor_EditTitle");
        PrimaryButtonText = Loc.Get("Editor_Save");
        CloseButtonText = Loc.Get("Editor_Cancel");
        _glyph = existing?.IconGlyph ?? TargetIcons.DefaultGlyph;
        _pendingImagePath = existing?.IconPath;
        FillPolling(existing?.PollingSeconds ?? SettingsStore.Load().DefaultPollingSeconds);
        FillIcons(_glyph);
        TitleBox.Text = existing?.Title ?? string.Empty;
        HostBox.Text = existing?.Host ?? string.Empty;
        RefreshPreview();
        Loaded += OnEditorLoaded;
        PointerWheelChanged += OnEditorWheel;
    }

    private void OnEditorLoaded(object sender, RoutedEventArgs e)
    {
        var hostHeight = XamlRoot?.Size.Height ?? MainWindow.FlyoutHeight;
        EditorScroll.MaxHeight = Math.Clamp(hostHeight - 220, 240, 420);
    }

    private void OnEditorWheel(object sender, PointerRoutedEventArgs e)
    {
        if (PollingBox.IsDropDownOpen)
            return;

        var delta = e.GetCurrentPoint(EditorScroll).Properties.MouseWheelDelta;
        if (delta == 0)
            return;

        EditorScroll.ChangeView(null, EditorScroll.VerticalOffset - delta, null, true);
        e.Handled = true;
    }

    public PingTarget? Result { get; private set; }

    private void FillPolling(int selected)
    {
        PollingBox.Items.Clear();
        foreach (var seconds in PollingOptions.Seconds)
        {
            PollingBox.Items.Add(
                new ComboBoxItem
                {
                    Content = string.Format(Loc.Get("Editor_PollingItem"), seconds),
                    Tag = seconds,
                }
            );
        }
        SelectPolling(PollingOptions.Clamp(selected));
    }

    private void SelectPolling(int seconds)
    {
        foreach (ComboBoxItem item in PollingBox.Items)
        {
            if (item.Tag is int value && value == seconds)
            {
                PollingBox.SelectedItem = item;
                return;
            }
        }
        if (PollingBox.Items.Count > 0)
            PollingBox.SelectedIndex = 0;
    }

    private void FillIcons(string selected)
    {
        IconHost.Children.Clear();
        IconHost.ColumnDefinitions.Clear();
        IconHost.RowDefinitions.Clear();
        const int columns = 6;
        for (var i = 0; i < columns; i++)
            IconHost.ColumnDefinitions.Add(
                new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) }
            );
        IconHost.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });
        IconHost.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });

        var index = 0;
        foreach (var (glyph, _) in TargetIcons.Catalog)
        {
            var button = CreateIconButton(glyph);
            Grid.SetColumn(button, index % columns);
            Grid.SetRow(button, index / columns);
            IconHost.Children.Add(button);
            index++;
        }

        HighlightIcon(selected);
    }

    private Button CreateIconButton(string glyph)
    {
        var button = new Button
        {
            Tag = glyph,
            Width = 40,
            Height = 40,
            Padding = new Thickness(0),
            Margin = new Thickness(2),
            CornerRadius = new CornerRadius(8),
            Content = new FontIcon { Glyph = glyph, FontSize = 16 },
        };
        button.Click += OnIconClick;
        return button;
    }

    private void OnIconClick(object sender, RoutedEventArgs e)
    {
        if (sender is not Button { Tag: string glyph })
            return;

        _glyph = glyph;
        _pendingImagePath = null;
        HighlightIcon(glyph);
        RefreshPreview();
    }

    private void HighlightIcon(string? selected)
    {
        var useGlyph = string.IsNullOrEmpty(_pendingImagePath);
        foreach (var child in IconHost.Children)
        {
            if (child is not Button button || button.Tag is not string glyph)
                continue;

            var on = useGlyph && glyph == selected;
            if (on)
                button.Background = (Brush)Application.Current.Resources["LumenAccentSoftBrush"];
            else
                button.ClearValue(BackgroundProperty);
        }
    }

    private void OnFieldsChanged(object sender, TextChangedEventArgs e) => RefreshPreview();

    private async void OnPickImage(object sender, RoutedEventArgs e)
    {
        var picker = new FileOpenPicker();
        picker.FileTypeFilter.Add(".png");
        picker.FileTypeFilter.Add(".jpg");
        picker.FileTypeFilter.Add(".jpeg");
        picker.FileTypeFilter.Add(".ico");
        picker.FileTypeFilter.Add(".webp");
        InitializeWithWindow.Initialize(picker, WindowNative.GetWindowHandle(App.Main));
        var file = await picker.PickSingleFileAsync();
        if (file is null)
            return;
        _pendingImagePath = file.Path;
        HighlightIcon(null);
        RefreshPreview();
    }

    private void OnPrimaryClick(ContentDialog sender, ContentDialogButtonClickEventArgs args)
    {
        var draft = BuildDraft();
        if (TargetValidator.IsValid(draft))
        {
            Result = draft;
            ErrorText.Visibility = Visibility.Collapsed;
            return;
        }

        args.Cancel = true;
        ErrorText.Text = Loc.Get("Editor_Invalid");
        ErrorText.Visibility = Visibility.Visible;
    }

    private PingTarget BuildDraft()
    {
        var polling =
            PollingBox.SelectedItem is ComboBoxItem item && item.Tag is int seconds ? seconds : 5;
        var iconPath = _pendingImagePath;
        if (!IconStore.IsManaged(iconPath))
            iconPath = IconStore.Import(iconPath ?? string.Empty, _targetId);

        var host = TargetValidator.NormalizeHost(HostBox.Text);
        var title = TitleBox.Text.Trim();
        if (string.IsNullOrEmpty(title))
            title = TargetValidator.TitleFromHost(host);

        return new PingTarget
        {
            Id = _targetId,
            Title = title,
            Host = host,
            IconGlyph = string.IsNullOrEmpty(_glyph) ? TargetIcons.DefaultGlyph : _glyph,
            IconPath = iconPath,
            PollingSeconds = polling,
            IsEnabled = _isEnabled,
        };
    }

    private void RefreshPreview()
    {
        PreviewTitle.Text = string.IsNullOrWhiteSpace(TitleBox.Text)
            ? Loc.Get("Editor_PreviewFallback")
            : TitleBox.Text.Trim();
        PreviewHost.Text = string.IsNullOrWhiteSpace(HostBox.Text) ? "—" : HostBox.Text.Trim();
        var hasImage = !string.IsNullOrEmpty(_pendingImagePath) && File.Exists(_pendingImagePath);
        PreviewImage.Visibility = hasImage ? Visibility.Visible : Visibility.Collapsed;
        PreviewGlyph.Visibility = hasImage ? Visibility.Collapsed : Visibility.Visible;
        PreviewGlyph.Glyph = _glyph;
        if (hasImage)
            PreviewImage.Source = new BitmapImage(new Uri(_pendingImagePath!, UriKind.Absolute));
    }
}
