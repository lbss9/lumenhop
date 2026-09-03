namespace Lumenhop.Tests;

public sealed class LatencyPaletteTests
{
    [Theory]
    [InlineData(0, LatencyPalette.DefaultGoodColor)]
    [InlineData(5, LatencyPalette.DefaultGoodColor)]
    [InlineData(6, LatencyPalette.DefaultFairColor)]
    [InlineData(15, LatencyPalette.DefaultFairColor)]
    [InlineData(16, LatencyPalette.DefaultPoorColor)]
    [InlineData(30, LatencyPalette.DefaultPoorColor)]
    [InlineData(31, LatencyPalette.DefaultBadColor)]
    [InlineData(999, LatencyPalette.DefaultBadColor)]
    public void ColorFor_maps_each_band(long ms, string expected)
    {
        Assert.Equal(expected, LatencyPalette.Default.ColorFor(ms));
    }

    [Fact]
    public void Normalized_orders_thresholds()
    {
        var palette = new LatencyPalette
        {
            GoodMax = 50,
            FairMax = 10,
            PoorMax = 5,
        }.Normalized();

        Assert.True(palette.GoodMax < palette.FairMax);
        Assert.True(palette.FairMax < palette.PoorMax);
    }

    [Fact]
    public void Normalized_clamps_out_of_range()
    {
        var palette = new LatencyPalette
        {
            GoodMax = -10,
            FairMax = 99999,
            PoorMax = 99999,
        }.Normalized();

        Assert.InRange(palette.GoodMax, LatencyPalette.MinThreshold, LatencyPalette.MaxThreshold);
        Assert.InRange(palette.PoorMax, LatencyPalette.MinThreshold, LatencyPalette.MaxThreshold);
    }

    [Theory]
    [InlineData("#2EE6C7", true)]
    [InlineData("#abcdef", true)]
    [InlineData("2EE6C7", false)]
    [InlineData("#12345", false)]
    [InlineData("#GGGGGG", false)]
    [InlineData(null, false)]
    [InlineData("", false)]
    public void IsValidColor_checks_hex(string? hex, bool expected)
    {
        Assert.Equal(expected, LatencyPalette.IsValidColor(hex));
    }

    [Fact]
    public void Normalized_replaces_invalid_color_with_default()
    {
        var palette = new LatencyPalette { GoodColor = "nope" }.Normalized();
        Assert.Equal(LatencyPalette.DefaultGoodColor, palette.GoodColor);
    }

    [Fact]
    public void Normalized_uppercases_valid_color()
    {
        var palette = new LatencyPalette { FairColor = "#abcdef" }.Normalized();
        Assert.Equal("#ABCDEF", palette.FairColor);
    }
}
