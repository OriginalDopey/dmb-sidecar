namespace DmbSidecar.Api.Services.LineupExplain;

/// <summary>
/// Routed question families for offline Lineup Lab explain.
/// Each value maps to an <see cref="Handlers.ILineupExplainHandler"/> implementation registered in <see cref="LineupExplainRouter"/>.
/// </summary>
public enum LineupQuestionKind
{
    /// <summary>Explain overall recommendation vs current (Δ, top swaps).</summary>
    RecommendationSummary,

    /// <summary>Why / why not a player at DH (defense recovery).</summary>
    DhAssignment,

    /// <summary>Why bat player at lineup slot N (OBP table-setters, cleanup, etc.).</summary>
    BattingOrder,

    /// <summary>Why player A at position instead of player B.</summary>
    PositionComparison,

    /// <summary>Why player plays position X (defense + bat at spot).</summary>
    PositionAssignment,

    /// <summary>No confident match — swap summary fallback.</summary>
    Fallback,
}
