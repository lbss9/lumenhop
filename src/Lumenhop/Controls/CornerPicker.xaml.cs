using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Media;
using Windows.UI;

namespace Lumenhop.Controls;

public sealed partial class CornerPicker : UserControl
{
    public event EventHandler<WindowAnchor>? AnchorChanged;

    private WindowAnchor _anchor = WindowAnchor.BottomLeft;

    public CornerPicker()
    {
        InitializeComponent();
        Loaded += (_, _) => Paint();
    }

    public WindowAnchor Anchor
    {
        get => _anchor;
        set
        {
            if (_anchor == value)
                return;
            _anchor = value;
            Paint();
        }
    }

    private void OnCornerClick(object sender, RoutedEventArgs e)
    {
        if (sender is not Button button || button.Tag is not string tag)
            return;

        var next = WindowPlacement.Parse(tag);
        if (next == _anchor)
            return;

        _anchor = next;
        Paint();
        AnchorChanged?.Invoke(this, _anchor);
    }

    private void Paint()
    {
        StyleButton(TopLeftButton, WindowAnchor.TopLeft);
        StyleButton(TopRightButton, WindowAnchor.TopRight);
        StyleButton(BottomLeftButton, WindowAnchor.BottomLeft);
        StyleButton(BottomRightButton, WindowAnchor.BottomRight);
        CaptionText.Text = Loc.Get($"Settings_Anchor_{_anchor}");
    }

    private void StyleButton(Button button, WindowAnchor corner)
    {
        var selected = corner == _anchor;
        button.CornerRadius = new CornerRadius(8);
        button.BorderThickness = new Thickness(selected ? 0 : 1);
        button.Background = selected
            ? new SolidColorBrush(Color.FromArgb(0xFF, 0x2E, 0xE6, 0xC7))
            : new SolidColorBrush(Color.FromArgb(0x33, 0x2E, 0xE6, 0xC7));
        button.BorderBrush = new SolidColorBrush(Color.FromArgb(0x55, 0x2E, 0xE6, 0xC7));
        button.Content = null;
    }
}
