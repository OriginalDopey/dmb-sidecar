using DmbSidecar.Api.Services.LineupExplain;
using DmbSidecar.Api.Services.LineupExplain.Handlers;
using DmbSidecar.Api.Tests.Fixtures;

namespace DmbSidecar.Api.Tests.LineupExplain.Handlers;

/// <summary>
/// Unit tests for secondary lineup explain handlers:
/// <see cref="BattingOrderHandler"/>, <see cref="PositionComparisonHandler"/>,
/// and <see cref="FallbackHandler"/>.
/// </summary>
public sealed class RemainingHandlerTests
{
    /// <summary>
    /// Builds a <see cref="LineupExplainContext"/> for handler unit tests.
    /// </summary>
    private static LineupExplainContext Ctx(LineupQuestionIntent intent, string question) =>
        new(question, intent, LineupTestFixtures.DemoContext, LineupTestFixtures.DemoAnalysis, []);

    /// <summary>
    /// Verifies batting-order handler returns framework text and player name.
    /// </summary>
    [Fact]
    public void BattingOrderHandler_returns_framework()
    {
        var intent = new LineupQuestionIntent(
            LineupQuestionKind.BattingOrder, "Why bat Ruth at #4?", ["Ruth, Babe"], null, 4, null);
        var answer = new BattingOrderHandler().Build(Ctx(intent, "Why bat Ruth at #4?"));
        answer.Should().Contain("Batting order");
        answer.Should().Contain("Ruth");
    }

    /// <summary>
    /// Verifies position comparison handler produces a side-by-side answer at SS.
    /// </summary>
    [Fact]
    public void PositionComparisonHandler_compares_two_players()
    {
        var intent = new LineupQuestionIntent(
            LineupQuestionKind.PositionComparison,
            "Why Knight over Mackanin at SS?",
            ["Knight, Ray", "Mackanin, Jim"],
            "SS",
            null,
            "Mackanin, Jim");
        var answer = new PositionComparisonHandler().Build(Ctx(intent, "Why Knight over Mackanin at SS?"));
        answer.Should().Contain("comparison");
        answer.Should().Contain("SS");
    }

    /// <summary>
    /// Verifies fallback handler delegates single-player questions to position logic.
    /// </summary>
    [Fact]
    public void FallbackHandler_delegates_single_player_to_position()
    {
        var intent = new LineupQuestionIntent(
            LineupQuestionKind.Fallback, "Mackanin", ["Mackanin, Jim"], "SS", null, null);
        var answer = new FallbackHandler().Build(Ctx(intent, "Mackanin"));
        answer.Should().Contain("Mackanin");
    }
}
