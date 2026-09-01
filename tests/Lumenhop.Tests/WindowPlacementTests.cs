namespace Lumenhop.Tests;

public sealed class WindowPlacementTests
{
    [Fact]
    public void Parse_unknown_falls_back_to_bottom_left()
    {
        Assert.Equal(WindowAnchor.BottomLeft, WindowPlacement.Parse(null));
        Assert.Equal(WindowAnchor.BottomLeft, WindowPlacement.Parse("nope"));
    }

    [Fact]
    public void Parse_reads_known_anchor()
    {
        Assert.Equal(WindowAnchor.TopRight, WindowPlacement.Parse("TopRight"));
    }

    [Theory]
    [InlineData(WindowAnchor.TopLeft, 12, 12)]
    [InlineData(WindowAnchor.TopRight, 588, 12)]
    [InlineData(WindowAnchor.BottomLeft, 12, 388)]
    [InlineData(WindowAnchor.BottomRight, 588, 388)]
    public void Compute_pins_each_corner(WindowAnchor anchor, int expectedX, int expectedY)
    {
        var (x, y) = WindowPlacement.Compute(0, 0, 1000, 800, 400, 400, anchor, margin: 12);
        Assert.Equal(expectedX, x);
        Assert.Equal(expectedY, y);
    }
}
