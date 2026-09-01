using Microsoft.UI.Xaml.Media;
using Windows.UI;

namespace Lumenhop;

/// <summary>Aurora palette used by the status dot and latency label.</summary>
public static class StatusTheme
{
    public static Brush BrushFor(string stateKey) => new SolidColorBrush(ColorFor(stateKey));

    public static Color ColorFor(string stateKey) =>
        stateKey switch
        {
            "online" => Color.FromArgb(0xFF, 0x2E, 0xE6, 0xC7),
            "slow" => Color.FromArgb(0xFF, 0xE3, 0xB3, 0x41),
            "down" => Color.FromArgb(0xFF, 0xFF, 0x5C, 0x7A),
            "probing" => Color.FromArgb(0xFF, 0x5B, 0x9D, 0xFF),
            "off" or "idle" => Color.FromArgb(0xFF, 0x7A, 0x7A, 0x84),
            _ => Color.FromArgb(0xFF, 0x7A, 0x7A, 0x84),
        };
}
