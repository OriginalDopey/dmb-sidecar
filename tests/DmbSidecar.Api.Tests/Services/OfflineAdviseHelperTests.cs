using DmbSidecar.Api.Models;
using DmbSidecar.Api.Services;

namespace DmbSidecar.Api.Tests.Services;

/// <summary>
/// Unit tests for <see cref="OfflineAdviseHelper.Build"/> fallback answers when
/// Foundry or MCP data is unavailable.
/// </summary>
public sealed class OfflineAdviseHelperTests
{
    /// <summary>
    /// Verifies generic questions include local IQ rules content in the answer.
    /// </summary>
    [Fact]
    public void Build_includes_question_and_page_type()
    {
        var request = new AdviseRequest(
            "What is RC/600?",
            new PageContext("lineup", "https://imaginesports.com/bball/manage/edit_lineup", "Primary vs. LHP", null, [], null));

        var answer = OfflineAdviseHelper.Build(
            request,
            null,
            null,
            ["RC/600 is runs created per 600 plate appearances in Classic Standard."],
            "Foundry down");

        answer.Should().Contain("RC/600");
        answer.Should().Contain("Rules (local IQ)");
    }

    /// <summary>
    /// Verifies roster-review prompts route to the roster review builder.
    /// </summary>
    [Fact]
    public void Build_uses_roster_review_when_question_matches()
    {
        var request = new AdviseRequest(
            "Review this roster for salary balance",
            new PageContext(
                "roster",
                "https://imaginesports.com/bball/team/roster",
                null,
                "T1",
                [new PageSlot(1, "1B", "Gehrig, Lou", "L", "$8,000,000", "batter")],
                new Dictionary<string, string> { ["teamName"] = "Demo" }));

        var answer = OfflineAdviseHelper.Build(request, null, null, [], "offline");
        answer.Should().Contain("Roster review");
    }
}
