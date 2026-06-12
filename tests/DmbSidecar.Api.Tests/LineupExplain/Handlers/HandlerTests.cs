using DmbSidecar.Api.Services.LineupExplain;
using DmbSidecar.Api.Services.LineupExplain.Handlers;
using DmbSidecar.Api.Tests.Fixtures;

namespace DmbSidecar.Api.Tests.LineupExplain.Handlers;

public sealed class HandlerTests
{
    private static LineupExplainContext Ctx(string question, LineupQuestionIntent intent) =>
        new(question, intent, LineupTestFixtures.DemoContext, LineupTestFixtures.DemoAnalysis, []);

    [Fact]
    public void DhAssignmentHandler_mentions_defense_recovery()
    {
        var intent = new LineupQuestionIntent(
            LineupQuestionKind.DhAssignment,
            "Why not Cobb at DH?",
            ["Cobb, Ty"],
            "DH",
            null,
            null);
        var answer = new DhAssignmentHandler().Build(Ctx("Why not Cobb at DH?", intent));
        answer.Should().Contain("bat-only");
        answer.Should().Contain("Cobb");
    }

    [Fact]
    public void PositionAssignmentHandler_compares_mackanin_ss()
    {
        var intent = new LineupQuestionIntent(
            LineupQuestionKind.PositionAssignment,
            "Why Mackanin at SS?",
            ["Mackanin, Jim"],
            "SS",
            null,
            null);
        var answer = new PositionAssignmentHandler().Build(Ctx("Why Mackanin at SS?", intent));
        answer.Should().Contain("Mackanin");
        answer.Should().Contain("SS");
    }

    [Fact]
    public void RecommendationSummaryHandler_lists_swaps()
    {
        var intent = new LineupQuestionIntent(
            LineupQuestionKind.RecommendationSummary,
            "Explain differences",
            [],
            null,
            null,
            null);
        var answer = new RecommendationSummaryHandler().Build(Ctx("Explain differences", intent));
        answer.Should().Contain("Δ");
        answer.Should().Contain("SS");
    }
}
