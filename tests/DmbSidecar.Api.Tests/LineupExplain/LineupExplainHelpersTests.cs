using DmbSidecar.Api.Services.LineupExplain;
using DmbSidecar.Api.Tests.Fixtures;

namespace DmbSidecar.Api.Tests.LineupExplain;

/// <summary>
/// Unit tests for <see cref="LineupExplainHelpers"/> parsing utilities:
/// side labels, name normalization, batting-order extraction, position
/// detection, and roster player mention extraction.
/// </summary>
public sealed class LineupExplainHelpersTests
{
    /// <summary>
    /// Verifies pitcher-side strings format to display labels (vs LHP / vs RHP).
    /// </summary>
    [Theory]
    [InlineData("lhp", "vs LHP")]
    [InlineData("rhp", "vs RHP")]
    [InlineData("vs LHP", "vs LHP")]
    public void SideLabel_formats_pitcher_side(string input, string expected) =>
        LineupExplainHelpers.SideLabel(input).Should().Be(expected);

    /// <summary>
    /// Verifies <see cref="LineupExplainHelpers.Norm"/> extracts the last name from DMB format.
    /// </summary>
    [Fact]
    public void Norm_extracts_last_name() =>
        LineupExplainHelpers.Norm("Cobb, Ty").Should().Be("Cobb");

    /// <summary>
    /// Verifies batting-order slot numbers are parsed from common phrasing.
    /// </summary>
    [Theory]
    [InlineData("Why bat Ruth at #4?", 4)]
    [InlineData("leadoff hitter", 1)]
    [InlineData("cleanup spot", 4)]
    public void TryExtractBattingOrder_parses_slot(string question, int expected) =>
        LineupExplainHelpers.TryExtractBattingOrder(question.ToLowerInvariant()).Should().Be(expected);

    /// <summary>
    /// Verifies defensive position abbreviations are extracted from questions.
    /// </summary>
    [Theory]
    [InlineData("Why Mackanin at SS?", "SS")]
    [InlineData("DH question", "DH")]
    [InlineData("no position here", null)]
    public void TryExtractPosition_finds_position(string question, string? expected) =>
        LineupExplainHelpers.TryExtractPosition(question).Should().Be(expected);

    /// <summary>
    /// Verifies player names mentioned in questions are matched against context and analysis.
    /// </summary>
    [Fact]
    public void ExtractPlayersMentioned_finds_roster_names()
    {
        var players = LineupExplainHelpers.ExtractPlayersMentioned(
            "cobb and ruth dh",
            LineupTestFixtures.DemoContext,
            LineupTestFixtures.DemoAnalysis);
        players.Should().HaveCountGreaterThanOrEqualTo(2);
    }
}
