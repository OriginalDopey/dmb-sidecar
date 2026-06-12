using DmbSidecar.Api.Utilities;

namespace DmbSidecar.Api.Tests.Utilities;

public sealed class TextHelperTests
{
    [Fact]
    public void Truncate_short_text_unchanged() =>
        TextHelper.Truncate("hello", 10).Should().Be("hello");

    [Fact]
    public void Truncate_long_text_adds_ellipsis()
    {
        var result = TextHelper.Truncate("abcdefghij", 5);
        result.Should().Be("abcde…");
        result.Length.Should().Be(6);
    }
}
