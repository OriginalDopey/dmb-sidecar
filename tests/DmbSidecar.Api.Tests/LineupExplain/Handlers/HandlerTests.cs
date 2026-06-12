using DmbSidecar.Api.Services.LineupExplain;
using DmbSidecar.Api.Services.LineupExplain.Handlers;
using DmbSidecar.Api.Tests.Fixtures;

namespace DmbSidecar.Api.Tests.LineupExplain.Handlers;

/// <summary>
/// Unit tests for primary lineup explain handlers:
/// <see cref="DhAssignmentHandler"/>, <see cref="PositionAssignmentHandler"/>,
/// and <see cref="RecommendationSummaryHandler"/>.
/// </summary>
public sealed class HandlerTests
{
    /// <summary>
    /// Builds a <see cref="LineupExplainContext"/> for handler unit tests.
    /// </summary>
    private static LineupExplainContext Ctx(string question, LineupQuestionIntent intent) =>
        new(question, intent, LineupTestFixtures.DemoContext, LineupTestFixtures.DemoAnalysis, []);

    /// <summary>
    /// Verifies DH assignment answers mention bat-only role and the named player.
    /// </summary>
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

    /// <summary>
    /// Verifies position assignment answers reference the player and position.
    /// </summary>
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

    /// <summary>
    /// Verifies recommendation summary answers include delta and swap positions.
    /// </summary>
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
