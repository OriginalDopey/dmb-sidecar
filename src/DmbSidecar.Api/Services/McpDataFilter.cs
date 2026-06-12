using DmbSidecar.Api.Models;

namespace DmbSidecar.Api.Services;

/// <summary>
/// Validates MCP report text before it is injected into Foundry prompts.
/// Drops empty, placeholder, and stale snapshots that disagree with the browser DOM roster.
/// Used by <see cref="AdviseService"/> to prevent advising against an outdated cached team.
/// </summary>
internal static class McpDataFilter
{
    /// <summary>
    /// Returns normalized snapshot text, or null when missing, placeholder, or stale vs. page roster.
    /// </summary>
    public static string? NormalizeSnapshot(string? text, PageContext? context = null)
    {
        if (string.IsNullOrWhiteSpace(text)) return null;
        if (text.Contains("No cached MCP data", StringComparison.OrdinalIgnoreCase))
            return null;
        if (context != null && !SnapshotMatchesPage(text, context))
            return null;
        return text;
    }

    /// <summary>Returns league summary text, or null when missing or placeholder.</summary>
    public static string? NormalizeSummary(string? text) =>
        string.IsNullOrWhiteSpace(text) || text.Contains("No cached", StringComparison.OrdinalIgnoreCase)
            ? null
            : text;

    // --- Stale-cache detection ---

    /// <summary>Reject MCP roster when it clearly disagrees with the browser DOM (stale scrape).</summary>
    private static bool SnapshotMatchesPage(string snapshot, PageContext context)
    {
        if (context.Slots is not { Count: > 0 }) return true;

        var pageLastNames = context.Slots
            .Select(s => PlayerLastName(s.PlayerName))
            .Where(n => n.Length > 1)
            .ToHashSet(StringComparer.OrdinalIgnoreCase);

        if (pageLastNames.Count == 0) return true;

        var hits = 0;
        foreach (var name in pageLastNames)
        {
            if (snapshot.Contains(name, StringComparison.OrdinalIgnoreCase))
                hits++;
        }

        // At least two rostered players should appear in a fresh MCP snapshot.
        return hits >= 2;
    }

    private static string PlayerLastName(string? full)
    {
        if (string.IsNullOrWhiteSpace(full)) return "";
        var comma = full.IndexOf(',');
        return comma > 0 ? full[..comma].Trim() : full.Trim();
    }
}
