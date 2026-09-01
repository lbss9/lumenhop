namespace Lumenhop.Tests;

public sealed class AppLanguageTests
{
    [Fact]
    public void Resolve_portuguese_stays_portuguese()
    {
        Assert.Equal(AppLanguage.Portuguese, AppLanguage.Resolve("pt-BR", "en-US"));
    }

    [Fact]
    public void Resolve_english_stays_english()
    {
        Assert.Equal(AppLanguage.English, AppLanguage.Resolve("en", "pt-BR"));
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("System")]
    [InlineData("system")]
    public void Resolve_system_follows_os(string? stored)
    {
        Assert.Equal(AppLanguage.Portuguese, AppLanguage.Resolve(stored, "pt-BR"));
        Assert.Equal(AppLanguage.English, AppLanguage.Resolve(stored, "en-US"));
    }

    [Theory]
    [InlineData("pt")]
    [InlineData("pt-PT")]
    [InlineData("pt-BR")]
    public void FromOs_portuguese_variants_map_to_pt_br(string os)
    {
        Assert.Equal(AppLanguage.Portuguese, AppLanguage.FromOs(os));
    }

    [Theory]
    [InlineData("en")]
    [InlineData("en-GB")]
    [InlineData("ja-JP")]
    public void FromOs_other_locales_map_to_english(string os)
    {
        Assert.Equal(AppLanguage.English, AppLanguage.FromOs(os));
    }
}
