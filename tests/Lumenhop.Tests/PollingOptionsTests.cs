namespace Lumenhop.Tests;

public sealed class PollingOptionsTests
{
    [Fact]
    public void Clamp_keeps_known_interval()
    {
        Assert.Equal(5, PollingOptions.Clamp(5));
    }

    [Fact]
    public void Clamp_snaps_to_nearest()
    {
        Assert.Equal(10, PollingOptions.Clamp(12));
        Assert.Equal(1, PollingOptions.Clamp(0));
    }
}
