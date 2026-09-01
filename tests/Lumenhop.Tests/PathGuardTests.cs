namespace Lumenhop.Tests;

public sealed class PathGuardTests
{
    [Fact]
    public void IsInsideDirectory_accepts_child_file()
    {
        var root = Path.Combine(Path.GetTempPath(), "lumenhop-icons");
        var child = Path.Combine(root, "abc.png");
        Assert.True(PathGuard.IsInsideDirectory(child, root));
    }

    [Fact]
    public void IsInsideDirectory_rejects_sibling_prefix()
    {
        var root = Path.Combine(Path.GetTempPath(), "lumenhop-icons");
        var sibling = root + "_evil" + Path.DirectorySeparatorChar + "x.png";
        Assert.False(PathGuard.IsInsideDirectory(sibling, root));
    }

    [Fact]
    public void IsInsideDirectory_rejects_traversal()
    {
        var root = Path.Combine(Path.GetTempPath(), "lumenhop-icons");
        var outside = Path.Combine(root, "..", "secret.txt");
        Assert.False(PathGuard.IsInsideDirectory(outside, root));
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("cf-1")]
    [InlineData("../icons")]
    [InlineData("abcd")]
    public void IsSafeFileStem_rejects_unsafe_names(string? name)
    {
        Assert.False(PathGuard.IsSafeFileStem(name));
    }

    [Fact]
    public void SanitizeId_keeps_guid_n()
    {
        var id = Guid.NewGuid().ToString("N");
        Assert.Equal(id, PathGuard.SanitizeId(id));
    }

    [Fact]
    public void SanitizeId_replaces_unsafe_value()
    {
        var id = PathGuard.SanitizeId("../evil");
        Assert.True(PathGuard.IsSafeFileStem(id));
        Assert.NotEqual("../evil", id);
    }
}
