namespace DmbSidecar.Api.Models;

/// <summary>
/// Lineup Lab analyze/explain contracts.
/// <see cref="LineupAnalyzeResponse"/> is produced by the MCP lineup engine and optionally
/// attached to <see cref="LineupExplainRequest"/> so explain handlers can cite precomputed deltas.
/// </summary>

/// <summary>Bridge request to optimize a nine-man lineup vs LHP or RHP.</summary>
public sealed record LineupAnalyzeRequest(
    string PitcherSide,
    IReadOnlyList<LineupSlotInput> CurrentLineup,
    IReadOnlyList<string> RosterNames,
    string LineupName
);

/// <summary>Single batting-order slot sent to the lineup optimizer.</summary>
public sealed record LineupSlotInput(
    int Order,
    string? PlayerName,
    string? Position
);

/// <summary>Side-by-side lineup comparison for the Lineup Lab grid.</summary>
public sealed record LineupAnalyzeResponse(
    string LineupName,
    string PitcherSide,
    IReadOnlyList<LineupSlotResult> CurrentLineup,
    IReadOnlyList<LineupSlotResult> RecommendedLineup,
    double CurrentTotal,
    double RecommendedTotal,
    double Delta,
    IReadOnlyList<LineupSwap> Swaps,
    IReadOnlyList<string> Notes,
    IReadOnlyList<string> PlatoonHints,
    LineupChart Chart,
    int PoolSize,
    string Summary,
    string? Engine = null
);

/// <summary>Per-slot offensive, defensive, and pool metadata from the lineup engine.</summary>
public sealed record LineupSlotResult(
    int Order,
    string Position,
    string Player,
    double Rc600,
    double Def,
    double Total,
    int Salary,
    bool InPool,
    double Ops = 0,
    double Obp = 0,
    double Hrf = 0,
    int BatPlat = 0,
    string Run = "",
    string Injury = "",
    string RangeGrade = "",
    int? Err = null
);

/// <summary>Position-level player change between current and recommended lineups.</summary>
public sealed record LineupSwap(string Position, string From, string To, double Gain);

/// <summary>Per-slot RC+def totals for chart rendering in the side panel.</summary>
public sealed record LineupChart(
    IReadOnlyList<string> Labels,
    IReadOnlyList<double> Current,
    IReadOnlyList<double> Recommended
);

/// <summary>Free-text lineup question with optional precomputed analysis from Optimize.</summary>
public sealed record LineupExplainRequest(
    string Question,
    PageContext Context,
    LineupAnalyzeResponse? Lineup = null
);
