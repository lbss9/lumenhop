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
        _lastColor = StatusTheme.ColorFor("idle");
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

    private static void OnStateChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) =>
        ((StatusDot)d).Apply((string)(e.NewValue ?? "idle"));

    private void OnTapped(object sender, TappedRoutedEventArgs e)
    {
        e.Handled = true;
        if (DataContext is PingTargetViewModel vm)
            PingMonitor.Instance.SetEnabled(vm.Id, !vm.IsEnabled);
    }

    private void Apply(string state)
    {
        Pulse.Stop();
        var key = state.ToLowerInvariant();
        var color = StatusTheme.ColorFor(key);
        var crossedPower = IsOff(_lastKey) != IsOff(key);
        if (crossedPower && _lastKey != key)
            PlayToggle(_lastColor, color, IsOff(key));
        else
            Snap(color);

        _lastKey = key;
        _lastColor = color;

        if (key is "online" or "probing" or "slow")
        {
            Halo.Opacity = 0.5;
            Pulse.Begin();
            return;
        }

        Halo.Opacity = 0;
    }

    private void Snap(Color color)
    {
        _dotBrush.Color = color;
        _haloBrush.Color = color;
        _ringBrush.Color = color;
        Pop.ScaleX = 1;
        Pop.ScaleY = 1;
    }

    private void PlayToggle(Color from, Color to, bool turningOff)
    {
        _toggle?.Stop();
        Snap(from);
        var duration = new Duration(TimeSpan.FromMilliseconds(280));
        var ease = new CubicEase { EasingMode = EasingMode.EaseInOut };
        var board = new Storyboard();
        board.Children.Add(ColorAnim(_dotBrush, from, to, duration, ease));
        board.Children.Add(ColorAnim(_haloBrush, from, to, duration, ease));
        board.Children.Add(ColorAnim(_ringBrush, from, to, duration, ease));
        board.Children.Add(ScaleAnim("ScaleX", turningOff ? 0.72 : 1.18, duration, ease));
        board.Children.Add(ScaleAnim("ScaleY", turningOff ? 0.72 : 1.18, duration, ease));
        _toggle = board;
        board.Begin();
    }

    private static ColorAnimation ColorAnim(
        SolidColorBrush brush,
        Color from,
        Color to,
        Duration duration,
        EasingFunctionBase ease
    )
    {
        var anim = new ColorAnimation
        {
            From = from,
            To = to,
            Duration = duration,
            EasingFunction = ease,
        };
        Storyboard.SetTarget(anim, brush);
        Storyboard.SetTargetProperty(anim, "Color");
        return anim;
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
