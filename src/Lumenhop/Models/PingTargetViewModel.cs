using System.ComponentModel;
using System.Runtime.CompilerServices;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Media.Imaging;
using Windows.UI;

namespace Lumenhop;

/// <summary>Live card state for one monitored destination.</summary>
public sealed class PingTargetViewModel : INotifyPropertyChanged
{
    private string _title = string.Empty;
    private string _host = string.Empty;
    private string _iconGlyph = TargetIcons.DefaultGlyph;
    private string? _iconPath;
    private string _latencyText = "—";
    private PingState _state = PingState.Idle;
    private ImageSource? _iconImage;
    private long? _roundtripMs;
    private bool _isEnabled = true;

    public event PropertyChangedEventHandler? PropertyChanged;

    public string Id { get; init; } = Guid.NewGuid().ToString("N");

    public int PollingSeconds { get; set; } = 5;

    public bool IsEnabled
    {
        get => _isEnabled;
        set
        {
            if (!Set(ref _isEnabled, value))
                return;
            if (value)
                return;
            State = PingState.Off;
            LatencyText = LatencyFormat.Format(null, PingState.Off);
        }
    }

    public string Title
    {
        get => _title;
        set => Set(ref _title, value);
    }

    public string Host
    {
        get => _host;
        set => Set(ref _host, value);
    }

    public string IconGlyph
    {
        get => _iconGlyph;
        set => Set(ref _iconGlyph, value);
    }

    public string? IconPath
    {
        get => _iconPath;
        set
        {
            if (!Set(ref _iconPath, value))
                return;
            IconImage = CreateImage(value);
            OnPropertyChanged(nameof(GlyphVisibility));
            OnPropertyChanged(nameof(ImageVisibility));
        }
    }

    public string LatencyText
    {
        get => _latencyText;
        private set => Set(ref _latencyText, value);
    }

    public PingState State
    {
        get => _state;
        private set
        {
            if (!Set(ref _state, value))
                return;
            OnPropertyChanged(nameof(StateKey));
            RaiseColorChanged();
        }
    }

    public string StateKey => State.ToString().ToLowerInvariant();

    public Color StatusColor => StatusTheme.Resolve(State, _roundtripMs);

    public Brush StatusBrush => new SolidColorBrush(StatusColor);

    /// <summary>Re-evaluates the dot colour after the user edits the latency palette.</summary>
    public void RefreshColor() => RaiseColorChanged();

    private void RaiseColorChanged()
    {
        OnPropertyChanged(nameof(StatusColor));
        OnPropertyChanged(nameof(StatusBrush));
    }

    public ImageSource? IconImage
    {
        get => _iconImage;
        private set => Set(ref _iconImage, value);
    }

    public Visibility GlyphVisibility =>
        string.IsNullOrEmpty(IconPath) ? Visibility.Visible : Visibility.Collapsed;

    public Visibility ImageVisibility =>
        string.IsNullOrEmpty(IconPath) ? Visibility.Collapsed : Visibility.Visible;

    public long? RoundtripMs => _roundtripMs;

    public void ApplyProbe(PingProbeResult result, PingState state)
    {
        if (!_isEnabled)
            return;
        _roundtripMs = result.RoundtripMs;
        State = state;
        LatencyText = LatencyFormat.Format(result.RoundtripMs, state);
        RaiseColorChanged();
    }

    public void SetProbing()
    {
        if (!_isEnabled)
            return;
        State = PingState.Probing;
        LatencyText = LatencyFormat.Format(null, PingState.Probing);
    }

    public PingTarget ToTarget() =>
        new()
        {
            Id = Id,
            Title = Title,
            Host = Host,
            IconGlyph = IconGlyph,
            IconPath = IconPath,
            PollingSeconds = PollingSeconds,
            IsEnabled = IsEnabled,
        };

    public void CopyFrom(PingTarget target)
    {
        Title = target.Title;
        Host = target.Host;
        IconGlyph = string.IsNullOrEmpty(target.IconGlyph)
            ? TargetIcons.DefaultGlyph
            : target.IconGlyph;
        IconPath = target.IconPath;
        PollingSeconds = PollingOptions.Clamp(target.PollingSeconds);
        IsEnabled = target.IsEnabled;
    }

    public static PingTargetViewModel FromTarget(PingTarget target)
    {
        var vm = new PingTargetViewModel { Id = target.Id };
        vm.CopyFrom(target);
        return vm;
    }

    private static ImageSource? CreateImage(string? path)
    {
        if (!IconStore.IsManaged(path) || !File.Exists(path))
            return null;

        return SafeTry.Run<ImageSource>(() => new BitmapImage(new Uri(path, UriKind.Absolute)));
    }

    private bool Set<T>(ref T field, T value, [CallerMemberName] string? name = null)
    {
        if (Equals(field, value))
            return false;
        field = value;
        OnPropertyChanged(name);
        return true;
    }

    private void OnPropertyChanged([CallerMemberName] string? name = null) =>
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
}
