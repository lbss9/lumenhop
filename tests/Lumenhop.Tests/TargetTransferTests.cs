namespace Lumenhop.Tests;

public sealed class TargetTransferTests
{
    private static List<PingTarget> Sample() =>
        [
            new PingTarget
            {
                Title = "Cloudflare",
                Host = "1.1.1.1",
                IconGlyph = "\uE753",
                PollingSeconds = 5,
                IconPath = @"C:\Users\someone\AppData\Local\Lumenhop\icons\abc.png",
            },
            new PingTarget
            {
                Title = "Riot",
                Host = "br1.api.riotgames.com",
                PollingSeconds = 10,
            },
        ];

    [Fact]
    public void Export_then_import_round_trips_fields()
    {
        var json = TargetTransfer.Export(Sample(), "1.2.0");
        var result = TargetTransfer.Import(json);

        Assert.True(result.Ok);
        Assert.Equal(2, result.Targets.Count);
        Assert.Equal("Cloudflare", result.Targets[0].Title);
        Assert.Equal("1.1.1.1", result.Targets[0].Host);
        Assert.Equal(10, result.Targets[1].PollingSeconds);
    }

    [Fact]
    public void Import_regenerates_id_and_drops_icon_path()
    {
        var original = Sample();
        var json = TargetTransfer.Export(original, "1.2.0");
        var result = TargetTransfer.Import(json);

        Assert.Null(result.Targets[0].IconPath);
        Assert.NotEqual(original[0].Id, result.Targets[0].Id);
        Assert.True(result.Targets[0].IsEnabled);
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    [InlineData("not json")]
    [InlineData("{\"format\":\"something.else\",\"targets\":[]}")]
    public void Import_rejects_unrecognized_content(string content)
    {
        Assert.False(TargetTransfer.Import(content).Ok);
    }

    [Fact]
    public void Import_rejects_tampered_checksum()
    {
        var json = TargetTransfer.Export(Sample(), "1.2.0");
        var tampered = json.Replace("1.1.1.1", "9.9.9.9");

        var result = TargetTransfer.Import(tampered);

        Assert.False(result.Ok);
        Assert.Equal(ImportError.Tampered, result.Error);
    }

    [Fact]
    public void Export_skips_invalid_targets()
    {
        var targets = new List<PingTarget>
        {
            new() { Title = "Good", Host = "1.1.1.1", PollingSeconds = 5 },
            new() { Title = "", Host = "bad host", PollingSeconds = 5 },
        };

        var result = TargetTransfer.Import(TargetTransfer.Export(targets, "1.2.0"));

        Assert.True(result.Ok);
        Assert.Single(result.Targets);
        Assert.Equal("Good", result.Targets[0].Title);
    }
}
