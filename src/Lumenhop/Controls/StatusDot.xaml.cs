using Microsoft.UI.Input;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Media.Animation;
using Windows.UI;

namespace Lumenhop.Controls;

public sealed partial class StatusDot : UserControl
{
    private readonly SolidColorBrush _dotBrush = new();
    private readonly SolidColorBrush _haloBrush = new();
    private readonly SolidColorBrush _ringBrush = new();
    private string _lastKey = "idle";
    private Color _lastColor;
    private Storyboard? _toggle;

    public StatusDot()
    {
        InitializeComponent();
        ProtectedCursor = InputSystemCursor.Create(InputSystemCursorShape.Hand);
        Dot.Fill = _dotBrush;
        Halo.Fill = _haloBrush;
        Ring.Stroke = _ringBrush;
        _lastColor = StatusTheme.Resolve(PingState.Idle, null);
        Snap(_lastColor);
    }

    public static readonly DependencyProperty StateProperty = DependencyProperty.Register(
        nameof(State),
        typeof(string),
        typeof(StatusDot),
        new PropertyMetadata("idle", OnStateChanged)
    );

    public string State
    {
        get => (string)GetValue(StateProperty);
        set => SetValue(StateProperty, value);
    }

    public static readonly DependencyProperty DotColorProperty = DependencyProperty.Register(
        nameof(DotColor),
        typeof(Color),
        typeof(StatusDot),
        new PropertyMetadata(Color.FromArgb(0xFF, 0x7A, 0x7A, 0x84), OnColorChanged)
    );

    public Color DotColor
    {
        get => (Color)GetValue(DotColorProperty);
        set => SetValue(DotColorProperty, value);
    }

    private static void OnStateChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) =>
        ((StatusDot)d).ApplyState((string)(e.NewValue ?? "idle"));

    private static void OnColorChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) =>
        ((StatusDot)d).SetColor((Color)e.NewValue);

    private void OnTapped(object sender, TappedRoutedEventArgs e)
    {
        e.Handled = true;
        if (DataContext is PingTargetViewModel vm)
            PingMonitor.Instance.SetEnabled(vm.Id, !vm.IsEnabled);
    }

    /// <summary>Sets the dot, halo and ring to the band colour supplied by the view-model.</summary>
    private void SetColor(Color color)
    {
        _dotBrush.Color = color;
        _haloBrush.Color = color;
        _ringBrush.Color = color;
        _lastColor = color;
    }

    private void ApplyState(string state)
    {
        var key = state.ToLowerInvariant();
        if (IsOff(_lastKey) != IsOff(key))
            PlayPop(IsOff(key));
        _lastKey = key;

        if (key is "online" or "probing" or "slow")
        {
            Halo.Opacity = 0.5;
            Pulse.Begin();
            return;
        }

        Pulse.Stop();
        Halo.Opacity = 0;
    }

    private void Snap(Color color)
    {
        SetColor(color);
        Pop.ScaleX = 1;
        Pop.ScaleY = 1;
    }

    private void PlayPop(bool turningOff)
    {
        _toggle?.Stop();
        var duration = new Duration(TimeSpan.FromMilliseconds(280));
        var ease = new CubicEase { EasingMode = EasingMode.EaseInOut };
        var board = new Storyboard();
        board.Children.Add(ScaleAnim("ScaleX", turningOff ? 0.72 : 1.18, duration, ease));
        board.Children.Add(ScaleAnim("ScaleY", turningOff ? 0.72 : 1.18, duration, ease));
        _toggle = board;
        board.Begin();
    }

    private DoubleAnimation ScaleAnim(
        string property,
        double peak,
        Duration duration,
        EasingFunctionBase ease
    )
    {
        var anim = new DoubleAnimation
        {
            From = 1,
            To = peak,
            Duration = duration,
            AutoReverse = true,
            EasingFunction = ease,
        };
        Storyboard.SetTarget(anim, Pop);
        Storyboard.SetTargetProperty(anim, property);
        return anim;
    }

    private static bool IsOff(string key) => key is "off" or "idle";
}
