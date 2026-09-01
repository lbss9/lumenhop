namespace Lumenhop.Tests;

public sealed class PingStatusMapperTests
{
    [Fact]
    public void FromProbe_failed_is_down()
    {
        var result = new PingProbeResult(false, null, "timeout");
        Assert.Equal(PingState.Down, PingStatusMapper.FromProbe(result));
    }

    [Fact]
    public void FromProbe_fast_success_is_online()
    {
        var result = new PingProbeResult(true, 18, null);
        Assert.Equal(PingState.Online, PingStatusMapper.FromProbe(result));
    }

    [Fact]
    public void FromProbe_high_latency_is_slow()
    {
        var result = new PingProbeResult(true, 240, null);
        Assert.Equal(PingState.Slow, PingStatusMapper.FromProbe(result));
    }

    [Fact]
    public void FromProbe_success_without_rtt_is_down()
    {
        var result = new PingProbeResult(true, null, null);
        Assert.Equal(PingState.Down, PingStatusMapper.FromProbe(result));
    }
}
