using System.Text;
using DmbSidecar.Api.Models;

namespace DmbSidecar.Api.Services;

/// <summary>
/// Composes a structured offline answer when Foundry invocation fails in <see cref="AdviseService"/>.
/// Prefers a full roster review when the question asks for one; otherwise emits compact roster,
/// league standings snippets, and a single local IQ rules cite.
/// </summary>
internal static class OfflineAdviseHelper
{
    /// <summary>
    /// Builds markdown answer text from page context, optional MCP blocks, IQ snippets, and the Foundry error.
    /// Does not include stale MCP roster text (filtered upstream).
    /// </summary>
    public static string Build(
        AdviseRequest request,
        string? teamSnapshot,
        string? leagueSummary,
        IReadOnlyList<string> iqSnippets,
        string foundryError)
    {
        var sb = new StringBuilder();
        var q = request.Question.ToLowerInvariant();

        var rosterReview = OfflineRosterReview.Build(request.Context);
        if (rosterReview != null && OfflineRosterReview.WantsReview(request.Question))
        {
            sb.AppendLine(rosterReview);
            sb.AppendLine();
        }
        else if (request.Context.PageType == "roster" && request.Context.Slots is { Count: > 0 })
        {
            sb.AppendLine("**Roster on screen (summary):**");
            AppendCompactRoster(sb, request.Context);
            sb.AppendLine();
        }

        if (leagueSummary != null &&
            !leagueSummary.Contains("No cached", StringComparison.Ordinal) &&
            (q.Contains("division") || q.Contains("standings") || q.Contains("lead")))
        {
            sb.AppendLine("**League (MCP cache):**");
            sb.AppendLine(leagueSummary);
            sb.AppendLine();
        }

        // MCP roster omitted when stale vs. page (filtered in AdviseService)

        // One rules cite max — only when we didn't produce a roster review
        if (rosterReview == null && iqSnippets.Count > 0)
        {
            sb.AppendLine("**Rules (local IQ):**");
            sb.AppendLine($"- {iqSnippets[0]}");
            sb.AppendLine();
        }

        if (sb.Length == 0)
        {
            sb.AppendLine("**Foundry agent unavailable** — no page context to analyze.");
            sb.AppendLine($"Foundry: {foundryError}");
        }

        return sb.ToString().TrimEnd();
    }

    private static void AppendCompactRoster(StringBuilder sb, PageContext context)
    {
        if (context.Extra != null)
        {
            if (context.Extra.TryGetValue("teamName", out var team))
                sb.AppendLine($"{team} · {context.Extra.GetValueOrDefault("totalValue")} · cash {context.Extra.GetValueOrDefault("cashBalance")}");
        }

        foreach (var group in new[] { ("Hitters", "batter"), ("Pitchers", "pitcher"), ("IR", "ir") })
        {
            var rows = context.Slots!
                .Where(s => string.Equals(s.Section, group.Item2, StringComparison.OrdinalIgnoreCase))
                .Take(5)
                .ToList();
            if (rows.Count == 0) continue;
            sb.AppendLine($"{group.Item1} ({context.Slots!.Count(s => s.Section == group.Item2)}): " +
                string.Join(", ", rows.Select(p => $"{p.PlayerName} {p.Salary}")));
        }
    }
}
