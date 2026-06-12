using DmbSidecar.Api.Services.LineupExplain;
using DmbSidecar.Api.Services.LineupExplain.Handlers;
using DmbSidecar.Api.Tests.Fixtures;

namespace DmbSidecar.Api.Tests.LineupExplain.Handlers;

public sealed class RemainingHandlerTests
{
    private static LineupExplainContext Ctx(LineupQuestionIntent intent, string question) =>
        new(question, intent, LineupTestFixtures.DemoContext, LineupTestFixtures.DemoAnalysis, []);

    [Fact]
    public void BattingOrderHandler_returns_framework()
    {
        var intent = new LineupQuestionIntent(
            LineupQuestionKind.BattingOrder, "Why bat Ruth at #4?", ["Ruth, Babe"], null, 4, null);
        var answer = new BattingOrderHandler().Build(Ctx(intent, "Why bat Ruth at #4?"));
        answer.Should().Contain("Batting order");
        answer.Should().Contain("Ruth");
    }

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

    [Fact]
    public void FallbackHandler_delegates_single_player_to_position()
    {
        var intent = new LineupQuestionIntent(
            LineupQuestionKind.Fallback, "Mackanin", ["Mackanin, Jim"], "SS", null, null);
        var answer = new FallbackHandler().Build(Ctx(intent, "Mackanin"));
        answer.Should().Contain("Mackanin");
    }
}
