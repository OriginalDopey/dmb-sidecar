namespace DmbSidecar.Api.Models;

/// <summary>Bridge request to optimize a nine-man lineup vs LHP or RHP.</summary>
public sealed record LineupAnalyzeRequest(
    string PitcherSide,
    IReadOnlyList<LineupSlotInput> CurrentLineup,
    IReadOnlyList<string> RosterNames,
    string LineupName
);

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

public sealed record LineupSwap(string Position, string From, string To, double Gain);

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
