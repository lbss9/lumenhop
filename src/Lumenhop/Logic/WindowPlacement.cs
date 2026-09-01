namespace Lumenhop;

/// <summary>Computes flyout coordinates for a work area and a corner anchor.</summary>
public static class WindowPlacement
{
    public const int DefaultMargin = 12;

    public static WindowAnchor Parse(string? value) =>
        Enum.TryParse(value, ignoreCase: true, out WindowAnchor anchor)
            ? anchor
            : WindowAnchor.BottomLeft;

    public static (int X, int Y) Compute(
        int workX,
        int workY,
        int workWidth,
        int workHeight,
        int width,
        int height,
        WindowAnchor anchor,
        int margin = DefaultMargin
    )
    {
        var right = anchor is WindowAnchor.TopRight or WindowAnchor.BottomRight;
        var bottom = anchor is WindowAnchor.BottomLeft or WindowAnchor.BottomRight;
        var x = right ? workX + workWidth - width - margin : workX + margin;
        var y = bottom ? workY + workHeight - height - margin : workY + margin;
        return (x, y);
    }
}
