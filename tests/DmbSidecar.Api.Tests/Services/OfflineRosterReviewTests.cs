using DmbSidecar.Api.Models;
using DmbSidecar.Api.Services;

namespace DmbSidecar.Api.Tests.Services;

/// <summary>
/// Unit tests for <see cref="OfflineRosterReview"/> prompt detection and
/// roster-page summary generation used in offline advise mode.
/// </summary>
public sealed class OfflineRosterReviewTests
{
    /// <summary>
    /// Verifies common roster-review phrasing is detected by <see cref="OfflineRosterReview.WantsReview"/>.
    /// </summary>
    [Theory]
    [InlineData("Review this roster for salary balance")]
    [InlineData("Explain this screen")]
    [InlineData("Check IR usage")]
    public void WantsReview_detects_roster_prompts(string question) =>
        OfflineRosterReview.WantsReview(question).Should().BeTrue();

    /// <summary>
    /// Verifies non-roster page types do not produce a roster review block.
    /// </summary>
    [Fact]
    public void Build_returns_null_for_non_roster_page()
    {
        var context = new PageContext("lineup", "https://example.com", null, null, [], null);
        OfflineRosterReview.Build(context).Should().BeNull();
    }

    /// <summary>
    /// Verifies roster pages with slots and metadata produce a non-empty summary.
    /// </summary>
    [Fact]
    public void Build_returns_summary_for_roster_page()
    {
        var context = new PageContext(
            "roster",
            "https://imaginesports.com/bball/team/roster",
            null,
            "TEAM1",
            [
                new PageSlot(1, "1B", "Gehrig, Lou", "L", "$8,000,000", "batter"),
                new PageSlot(2, "SP", "Johnson, Walter", "R", "$12,000,000", "pitcher"),
            ],
            new Dictionary<string, string>
            {
                ["teamName"] = "Demo Nine",
                ["totalValue"] = "$98,500,000",
            });

        var text = OfflineRosterReview.Build(context);
        text.Should().NotBeNullOrWhiteSpace();
        text.Should().Contain("Demo Nine");
    }
}
