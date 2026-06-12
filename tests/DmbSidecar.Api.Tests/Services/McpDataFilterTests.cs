using DmbSidecar.Api.Models;
using DmbSidecar.Api.Services;

namespace DmbSidecar.Api.Tests.Services;

public sealed class McpDataFilterTests
{
    [Fact]
    public void NormalizeSnapshot_returns_null_for_empty()
    {
        McpDataFilter.NormalizeSnapshot(null).Should().BeNull();
        McpDataFilter.NormalizeSnapshot("   ").Should().BeNull();
    }

    [Fact]
    public void NormalizeSnapshot_rejects_stale_cache_message()
    {
        McpDataFilter.NormalizeSnapshot("No cached MCP data for team").Should().BeNull();
    }

    [Fact]
    public void NormalizeSnapshot_rejects_snapshot_missing_page_players()
    {
        var context = new PageContext(
            "lineup",
            "https://imaginesports.com/bball/manage/edit_lineup",
            "Primary vs. LHP",
            "TEAM1",
            [new PageSlot(1, "DH", "Cobb, Ty", "L")],
            null);

        McpDataFilter.NormalizeSnapshot("Unrelated league text only", context).Should().BeNull();
    }

    [Fact]
    public void NormalizeSnapshot_accepts_matching_roster()
    {
        var context = new PageContext(
            "lineup",
            "https://imaginesports.com/bball/manage/edit_lineup",
            "Primary vs. LHP",
            "TEAM1",
            [
                new PageSlot(1, "DH", "Cobb, Ty", "L"),
                new PageSlot(2, "LF", "Ruth, Babe", "L"),
            ],
            null);

        McpDataFilter.NormalizeSnapshot("Roster includes Cobb and Ruth stats", context)
            .Should().Be("Roster includes Cobb and Ruth stats");
    }

    [Fact]
    public void NormalizeSummary_filters_no_cache()
    {
        McpDataFilter.NormalizeSummary("No cached summary").Should().BeNull();
        McpDataFilter.NormalizeSummary("Week 3 standings").Should().Be("Week 3 standings");
    }
}
