using DmbSidecar.Api.Models;
using DmbSidecar.Api.Services.LineupExplain;
using DmbSidecar.Api.Tests.Fixtures;

namespace DmbSidecar.Api.Tests.LineupExplain;

public sealed class LineupQuestionClassifierTests
{
    private readonly PageContext _context = LineupTestFixtures.DemoContext;
    private readonly LineupAnalyzeResponse _lineup = LineupTestFixtures.DemoAnalysis;

    [Theory]
    [InlineData("Explain the main differences between my lineup and the recommendation.", LineupQuestionKind.RecommendationSummary)]
    [InlineData("What changed in the recommendation?", LineupQuestionKind.RecommendationSummary)]
    public void Classify_recommendation_summary(string question, LineupQuestionKind expected)
    {
        var intent = LineupQuestionClassifier.Classify(question, _context, _lineup);
        intent.Kind.Should().Be(expected);
    }

    [Theory]
    [InlineData("Why not Cobb at DH?")]
    [InlineData("Why not Cobb or Ruth at DH?")]
    [InlineData("Should Ruth be the designated hitter?")]
    public void Classify_dh_assignment(string question)
    {
        var intent = LineupQuestionClassifier.Classify(question, _context, _lineup);
        intent.Kind.Should().Be(LineupQuestionKind.DhAssignment);
        intent.Players.Should().NotBeEmpty();
    }

    [Theory]
    [InlineData("Why bat Ruth at #4?", 4)]
    [InlineData("Why is Cobb leadoff?", 1)]
    [InlineData("Should Ruth hit cleanup?", 4)]
    public void Classify_batting_order(string question, int expectedSlot)
    {
        var intent = LineupQuestionClassifier.Classify(question, _context, _lineup);
        intent.Kind.Should().Be(LineupQuestionKind.BattingOrder);
        intent.BattingOrder.Should().Be(expectedSlot);
    }

    [Fact]
    public void Classify_position_assignment()
    {
        var intent = LineupQuestionClassifier.Classify("Why Mackanin at SS?", _context, _lineup);
        intent.Kind.Should().Be(LineupQuestionKind.PositionAssignment);
        intent.Players.Should().Contain(p => p.Contains("Mackanin", StringComparison.OrdinalIgnoreCase));
        intent.Position.Should().Be("SS");
    }

    [Fact]
    public void Classify_position_comparison()
    {
        var intent = LineupQuestionClassifier.Classify(
            "Why Knight over Mackanin at SS?", _context, _lineup);
        intent.Kind.Should().Be(LineupQuestionKind.PositionComparison);
        intent.Players.Should().HaveCountGreaterThanOrEqualTo(2);
        intent.Position.Should().Be("SS");
    }
}
