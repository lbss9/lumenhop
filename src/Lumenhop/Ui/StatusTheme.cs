using Microsoft.UI.Xaml.Media;
using Windows.UI;

namespace Lumenhop;

/// <summary>Resolves the status-dot and metric colour from state, latency and the user palette.</summary>
public static class StatusTheme
{
    private static readonly Color Probing = Color.FromArgb(0xFF, 0x5B, 0x9D, 0xFF);
    private static readonly Color Down = Color.FromArgb(0xFF, 0xFF, 0x5C, 0x7A);
    private static readonly Color Muted = Color.FromArgb(0xFF, 0x7A, 0x7A, 0x84);

    /// <summary>Latency bands currently in effect; updated when the user edits Settings.</summary>
    public static LatencyPalette Palette { get; set; } = LatencyPalette.Default;

    public static Color Resolve(PingState state, long? roundtripMs) =>
        state switch
        {
            PingState.Probing => Probing,
            PingState.Down => Down,
            PingState.Off or PingState.Idle => Muted,
            _ when roundtripMs is long ms => FromHex(Palette.ColorFor(ms)),
            _ => Muted,
        };

    public static Brush BrushFor(PingState state, long? roundtripMs) =>
        new SolidColorBrush(Resolve(state, roundtripMs));

    public static Color FromHex(string hex)
    {
        var value = hex.TrimStart('#');
        var r = System.Convert.ToByte(value.Substring(0, 2), 16);
        var g = System.Convert.ToByte(value.Substring(2, 2), 16);
        var b = System.Convert.ToByte(value.Substring(4, 2), 16);
        return Color.FromArgb(0xFF, r, g, b);
    }
}
