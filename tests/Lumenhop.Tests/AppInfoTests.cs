namespace Lumenhop.Tests;

public sealed class AppInfoTests
{
    [Fact]
    public void DefaultRepoUrl_is_trusted_github()
    {
        Assert.True(AppInfo.IsTrustedGitHubRepo(AppInfo.DefaultRepoUrl));
    }

    [Theory]
    [InlineData("https://github.com/acme/lumenhop")]
    [InlineData("https://github.com/acme/lumenhop/")]
    public void IsTrustedGitHubRepo_accepts_https_github(string url)
    {
        Assert.True(AppInfo.IsTrustedGitHubRepo(url));
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("http://github.com/acme/lumenhop")]
    [InlineData("https://evil.com/acme/lumenhop")]
    [InlineData("https://github.com.evil.com/acme/lumenhop")]
    [InlineData("https://github.com/acme/lumenhop/extra")]
    [InlineData("file:///C:/tmp")]
    [InlineData("https://github.com/acme")]
    public void IsTrustedGitHubRepo_rejects_untrusted(string? url)
    {
        Assert.False(AppInfo.IsTrustedGitHubRepo(url));
    }
}
