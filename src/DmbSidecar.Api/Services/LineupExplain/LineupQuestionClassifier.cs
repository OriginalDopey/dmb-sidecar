using DmbSidecar.Api.Models;

namespace DmbSidecar.Api.Services.LineupExplain;

/// <summary>
/// Maps free-text lineup questions to a handler kind + extracted entities.
/// Add patterns here; tune narrative in the matching handler.
/// </summary>
internal static class LineupQuestionClassifier
{
    public static LineupQuestionIntent Classify(
        string question,
        PageContext context,
        LineupAnalyzeResponse lineup)
    {
        var q = question.ToLowerInvariant();
        var players = LineupExplainHelpers.ExtractPlayersMentioned(question, context, lineup);
        var formal = LineupExplainHelpers.ExtractPlayerFromFormalName(question);
        if (formal != null && !players.Contains(formal, StringComparer.OrdinalIgnoreCase))
            players.Insert(0, formal);

        var position = LineupExplainHelpers.TryExtractPosition(question);
        var order = LineupExplainHelpers.TryExtractBattingOrder(q);
        var compareTo = TryExtractCompareTarget(q, players);

        if (IsRecommendationSummary(q, players))
            return new LineupQuestionIntent(LineupQuestionKind.RecommendationSummary, question, players, position, order, null);

        if (LineupExplainHelpers.IsDhQuestion(question))
            return new LineupQuestionIntent(LineupQuestionKind.DhAssignment, question, players, position ?? "DH", order, compareTo);

        if (IsPositionComparison(q, players, compareTo))
            return new LineupQuestionIntent(LineupQuestionKind.PositionComparison, question, players, position, order, compareTo);

        if (IsBattingOrder(q, players, order))
            return new LineupQuestionIntent(LineupQuestionKind.BattingOrder, question, players, position, order, compareTo);

        if (IsPositionAssignment(q, players, position))
            return new LineupQuestionIntent(LineupQuestionKind.PositionAssignment, question, players, position, order, compareTo);

        return new LineupQuestionIntent(LineupQuestionKind.Fallback, question, players, position, order, compareTo);
    }

    private static bool IsRecommendationSummary(string q, IReadOnlyList<string> players) =>
        q.Contains("explain") && (q.Contains("difference") || q.Contains("recommend") || q.Contains("main") || q.Contains("change"))
        || q.Contains("what changed") || q.Contains("why this lineup")
        || (q.Contains("explain") && players.Count == 0);

    private static bool IsPositionComparison(string q, IReadOnlyList<string> players, string? compareTo) =>
        players.Count >= 2
        && (compareTo != null
            || q.Contains(" over ")
            || q.Contains(" instead of ")
            || q.Contains(" rather than ")
            || q.Contains(" vs ")
            || q.Contains(" versus "));

    private static bool IsBattingOrder(string q, IReadOnlyList<string> players, int? order) =>
        !LineupExplainHelpers.IsDhQuestion(q)
        && (order != null
            || q.Contains("batting order")
            || q.Contains("bat ") && (q.Contains(" at ") || q.Contains(" #"))
            || q.Contains("why bat ")
            || q.Contains("hit ") && q.Contains(" #")
            || q.Contains("maximize ab")
            || q.Contains("maximize at-bat")
            || (players.Count > 0 && (q.Contains("lead") || q.Contains("cleanup") || q.Contains("#"))));

    private static bool IsPositionAssignment(string q, IReadOnlyList<string> players, string? position) =>
        players.Count > 0
        && (position != null
            || q.Contains("play ")
            || q.Contains(" at ")
            || q.Contains("why ") && q.Contains(" ss")
            || q.Contains("why ") && q.Contains(" 2b")
            || q.Contains("field"));

    private static string? TryExtractCompareTarget(string q, IReadOnlyList<string> players)
    {
        if (players.Count < 2)
            return null;
        foreach (var phrase in new[] { " over ", " instead of ", " rather than ", " vs ", " versus " })
        {
            var idx = q.IndexOf(phrase, StringComparison.Ordinal);
            if (idx < 0) continue;
            var tail = q[(idx + phrase.Length)..];
            foreach (var p in players)
            {
                var last = LineupExplainHelpers.Norm(p).ToLowerInvariant();
                if (tail.Contains(last))
                    return p;
            }
        }
        return players.Count >= 2 ? players[1] : null;
    }
}
