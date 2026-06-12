using DmbSidecar.Api.Utilities;

namespace DmbSidecar.Api.Tests.Utilities;

/// <summary>
/// Unit tests for <see cref="TextHelper"/> string truncation used in API responses.
/// </summary>
public sealed class TextHelperTests
{
    /// <summary>
    /// Verifies short strings pass through unchanged when under the max length.
    /// </summary>
    [Fact]
    public void Truncate_short_text_unchanged() =>
        TextHelper.Truncate("hello", 10).Should().Be("hello");

    /// <summary>
    /// Verifies long strings are truncated with an ellipsis character appended.
    /// </summary>
    [Fact]
    public void Truncate_long_text_adds_ellipsis()
    {
        var result = TextHelper.Truncate("abcdefghij", 5);
        result.Should().Be("abcde…");
        result.Length.Should().Be(6);
    }
}
