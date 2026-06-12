using DmbSidecar.Api.Services.LineupExplain;
using DmbSidecar.Api.Tests.Fixtures;

namespace DmbSidecar.Api.Tests.LineupExplain;

public sealed class LineupExplainRouterTests
{
    private readonly LineupExplainRouter _router = new();

    [Theory]
    [InlineData("Why not Cobb at DH?", LineupQuestionKind.DhAssignment)]
    [InlineData("Why Mackanin at SS?", LineupQuestionKind.PositionAssignment)]
    [InlineData("Why Knight over Mackanin at SS?", LineupQuestionKind.PositionComparison)]
    [InlineData("Explain the main differences between my lineup and the recommendation.", LineupQuestionKind.RecommendationSummary)]
    public void Route_returns_non_empty_answer_for_known_patterns(string question, LineupQuestionKind kind)
    {
        var (intent, answer) = _router.Route(
            question,
            LineupTestFixtures.DemoContext,
            LineupTestFixtures.DemoAnalysis,
            []);

        intent.Kind.Should().Be(kind);
        answer.Should().NotBeNullOrWhiteSpace();
    }

    [Fact]
    public void Answer_uses_same_handler_as_route()
    {
        var intent = _router.Classify(
            "Why not Cobb at DH?",
            LineupTestFixtures.DemoContext,
            LineupTestFixtures.DemoAnalysis);
        var ctx = new LineupExplainContext(
            "Why not Cobb at DH?",
            intent,
            LineupTestFixtures.DemoContext,
            LineupTestFixtures.DemoAnalysis,
            []);

        var direct = _router.Answer(ctx);
        var (_, routed) = _router.Route(
            "Why not Cobb at DH?",
            LineupTestFixtures.DemoContext,
            LineupTestFixtures.DemoAnalysis,
            []);

        direct.Should().Be(routed);
    }
}
