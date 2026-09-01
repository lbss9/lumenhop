namespace Lumenhop.Tests;

public sealed class TargetValidatorTests
{
    [Theory]
    [InlineData("Casa")]
    [InlineData("Cloudflare DNS")]
    public void IsTitleValid_accepts_normal_titles(string title)
    {
        Assert.True(TargetValidator.IsTitleValid(title));
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    [InlineData("01234567890123456789012345678901234567890")]
    public void IsTitleValid_rejects_empty_or_too_long(string title)
    {
        Assert.False(TargetValidator.IsTitleValid(title));
    }

    [Theory]
    [InlineData("1.1.1.1")]
    [InlineData("8.8.8.8")]
    [InlineData("example.com")]
    [InlineData("router.local")]
    [InlineData("br1.api.riotgames.com")]
    [InlineData("https://br1.api.riotgames.com")]
    [InlineData("https://br1.api.riotgames.com/lol/status/v4/platform-data")]
    [InlineData("2001:4860:4860::8888")]
    public void IsHostValid_accepts_ip_and_hostname(string host)
    {
        Assert.True(TargetValidator.IsHostValid(host));
    }

    [Fact]
    public void NormalizeHost_strips_scheme_path_and_port()
    {
        Assert.Equal(
            "br1.api.riotgames.com",
            TargetValidator.NormalizeHost("https://br1.api.riotgames.com/lol/status/v4")
        );
        Assert.Equal("1.1.1.1", TargetValidator.NormalizeHost("1.1.1.1:53"));
    }

    [Fact]
    public void TitleFromHost_uses_normalized_host()
    {
        Assert.Equal(
            "br1.api.riotgames.com",
            TargetValidator.TitleFromHost("https://br1.api.riotgames.com")
        );
    }

    [Theory]
    [InlineData("")]
    [InlineData("not a host")]
    [InlineData("-bad.com")]
    public void IsHostValid_rejects_garbage(string host)
    {
        Assert.False(TargetValidator.IsHostValid(host));
    }

    [Fact]
    public void IsValid_requires_all_fields()
    {
        var target = new PingTarget
        {
            Title = "Casa",
            Host = "192.168.1.1",
            PollingSeconds = 5,
        };
        Assert.True(TargetValidator.IsValid(target));
        target.Host = "";
        Assert.False(TargetValidator.IsValid(target));
    }
}
