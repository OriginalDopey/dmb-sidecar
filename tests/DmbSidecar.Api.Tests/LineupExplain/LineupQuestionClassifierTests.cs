using DmbSidecar.Api.Models;
using DmbSidecar.Api.Services.LineupExplain;
using DmbSidecar.Api.Tests.Fixtures;

namespace DmbSidecar.Api.Tests.LineupExplain;

/// <summary>
/// Unit tests for <see cref="LineupQuestionClassifier"/> natural-language
/// intent detection (DH, batting order, position assignment, comparisons).
/// </summary>
public sealed class LineupQuestionClassifierTests
{
    private readonly PageContext _context = LineupTestFixtures.DemoContext;
    private readonly LineupAnalyzeResponse _lineup = LineupTestFixtures.DemoAnalysis;

    /// <summary>
    /// Verifies recommendation-summary phrasing maps to <see cref="LineupQuestionKind.RecommendationSummary"/>.
    /// </summary>
    [Theory]
    [InlineData("Explain the main differences between my lineup and the recommendation.", LineupQuestionKind.RecommendationSummary)]
    [InlineData("What changed in the recommendation?", LineupQuestionKind.RecommendationSummary)]
    public void Classify_recommendation_summary(string question, LineupQuestionKind expected)
    {
        var intent = LineupQuestionClassifier.Classify(question, _context, _lineup);
        intent.Kind.Should().Be(expected);
    }

    /// <summary>
    /// Verifies DH-related questions classify as <see cref="LineupQuestionKind.DhAssignment"/>.
    /// </summary>
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

    /// <summary>
    /// Verifies batting-order questions extract the target slot number.
    /// </summary>
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

    /// <summary>
    /// Verifies single-player position questions extract player and position.
    /// </summary>
    [Fact]
    public void Classify_position_assignment()
    {
        var intent = LineupQuestionClassifier.Classify("Why Mackanin at SS?", _context, _lineup);
        intent.Kind.Should().Be(LineupQuestionKind.PositionAssignment);
        intent.Players.Should().Contain(p => p.Contains("Mackanin", StringComparison.OrdinalIgnoreCase));
        intent.Position.Should().Be("SS");
    }

    /// <summary>
    /// Verifies A-over-B position comparisons extract both players and position.
    /// </summary>
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
