namespace DmbSidecar.Api.Services.LineupExplain;

/// <summary>
/// Classified lineup question with extracted entities for handler routing.
/// Produced by <see cref="LineupQuestionClassifier"/> and consumed by <see cref="LineupExplainRouter"/>.
/// </summary>
/// <param name="Kind">Handler family selected for this question.</param>
/// <param name="RawQuestion">Original question text (preserves casing).</param>
/// <param name="Players">Player names matched from the question against lineup/roster context.</param>
/// <param name="Position">Defensive position extracted from the question, if any.</param>
/// <param name="BattingOrder">Batting-order slot 1–9 inferred from the question, if any.</param>
/// <param name="CompareToPlayer">Second player in a comparison question (e.g. "Knight over Mackanin").</param>
public sealed record LineupQuestionIntent(
    LineupQuestionKind Kind,
    string RawQuestion,
    IReadOnlyList<string> Players,
    string? Position,
    int? BattingOrder,
    string? CompareToPlayer);
