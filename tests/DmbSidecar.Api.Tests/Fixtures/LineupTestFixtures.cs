using DmbSidecar.Api.Models;

namespace DmbSidecar.Api.Tests.Fixtures;

/// <summary>Shared lineup analysis fixtures for explain and integration tests.</summary>
public static class LineupTestFixtures
{
    public static PageContext DemoContext => new(
        PageType: "lineup",
        Url: "https://imaginesports.com/bball/manage/edit_lineup",
        LineupName: "Primary vs. LHP",
        CurTeam: "FMWAWCUMRLYLNGGBENRQ",
        Slots: DemoCurrentLineup.Select(s => new PageSlot(s.Order, s.Position, s.Player, "L")).ToList(),
        Extra: new Dictionary<string, string> { ["pitcherSide"] = "lhp" });

    public static IReadOnlyList<LineupSlotResult> DemoCurrentLineup =>
    [
        Slot(1, "DH", "Cobb, Ty", 128, 0),
        Slot(2, "LF", "Ruth, Babe", 142, 4),
        Slot(3, "1B", "Greenberg, Hank", 118, 2),
        Slot(4, "RF", "Rizzo, Anthony", 102, 1),
        Slot(5, "CF", "Agee, Tommie", 88, 3),
        Slot(6, "3B", "Higgins, Bobby", 95, 2),
        Slot(7, "2B", "Knight, Ray", 78, 0),
        Slot(8, "SS", "Mackanin, Jim", 87, -0.5m),
        Slot(9, "C", "Nieves, Charlie", 72, 1),
    ];

    public static IReadOnlyList<LineupSlotResult> DemoRecommendedLineup =>
    [
        Slot(1, "CF", "Agee, Tommie", 88, 3),
        Slot(2, "2B", "Knight, Ray", 78, 0),
        Slot(3, "1B", "Greenberg, Hank", 118, 2),
        Slot(4, "LF", "Ruth, Babe", 142, 4),
        Slot(5, "3B", "Higgins, Bobby", 95, 2),
        Slot(6, "RF", "Cobb, Ty", 128, 7),
        Slot(7, "C", "Nieves, Charlie", 72, 1),
        Slot(8, "SS", "Knight, Ray", 78, 0.2m),
        Slot(9, "DH", "Rizzo, Anthony", 102, 0),
    ];

    public static LineupAnalyzeResponse DemoAnalysis => new(
        LineupName: "Primary vs. LHP",
        PitcherSide: "lhp",
        CurrentLineup: DemoCurrentLineup,
        RecommendedLineup: DemoRecommendedLineup,
        CurrentTotal: DemoCurrentLineup.Sum(s => s.Total),
        RecommendedTotal: DemoRecommendedLineup.Sum(s => s.Total),
        Delta: 8.2,
        Swaps:
        [
            new LineupSwap("DH", "Cobb, Ty", "Rizzo, Anthony", 2.1),
            new LineupSwap("SS", "Mackanin, Jim", "Knight, Ray", 3.2),
            new LineupSwap("RF", "Rizzo, Anthony", "Cobb, Ty", 2.9),
        ],
        Notes: [],
        PlatoonHints: [],
        Chart: new LineupChart([], [], []),
        PoolSize: 9,
        Summary: "Δ +8.2 RC+def",
        Engine: "dmb-config");

    private static LineupSlotResult Slot(int order, string pos, string player, double rc, decimal def) =>
        new(order, pos, player, rc, (double)def, rc + (double)def, 5_000_000, true);
}
