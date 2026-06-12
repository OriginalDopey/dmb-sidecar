using System.Text;
using DmbSidecar.Api.Models;

namespace DmbSidecar.Api.Services;

/// <summary>
/// Adapts browser-scraped lineup context to the MCP lineup engine for <c>POST /lineup/analyze</c>.
/// Parses roster pool and position eligibility from extension extras, maps bridge DTOs to API models,
/// and builds the markdown summary shown in the Lineup Lab grid.
/// </summary>
public sealed class LineupAnalyzeService
{
    private readonly McpBridgeClient _mcp;

    /// <summary>Creates the service with MCP bridge client dependency.</summary>
    public LineupAnalyzeService(McpBridgeClient mcp) => _mcp = mcp;

    /// <summary>
    /// Runs lineup optimization for the active Edit Lineup page.
    /// Returns null when slots are empty or the MCP bridge call fails.
    /// </summary>
    public async Task<LineupAnalyzeResponse?> AnalyzeAsync(PageContext context, CancellationToken ct = default)
    {
        if (context.Slots is not { Count: > 0 })
            return null;

        var pitcherSide = context.Extra?.GetValueOrDefault("pitcherSide") ?? "rhp";
        var roster = ParseRosterPool(context.Extra?.GetValueOrDefault("rosterPool"));
        if (roster.Count == 0)
            roster = context.Slots.Select(s => s.PlayerName).Where(n => n != null).Cast<string>().ToList();

        var eligibility = ParseEligibility(context.Extra?.GetValueOrDefault("positionEligibility"));

        var bridgeReq = new McpBridgeClient.LineupAnalyzeBridgeRequest(
            PitcherSide: pitcherSide,
            CurrentLineup: context.Slots
                .Select(s => new McpBridgeClient.LineupSlotBridge(s.Order, s.PlayerName, s.Position))
                .ToList(),
            RosterNames: roster,
            LineupName: context.LineupName ?? "",
            PositionEligibility: eligibility);

        var raw = await _mcp.AnalyzeLineupAsync(bridgeReq, ct);
        if (raw == null) return null;

        var notes = raw.Notes ?? [];
        var summary = BuildSummary(raw, context.LineupName ?? "Lineup");

        return new LineupAnalyzeResponse(
            raw.LineupName ?? context.LineupName ?? "",
            raw.PitcherSide ?? pitcherSide,
            MapSlots(raw.CurrentLineup),
            MapSlots(raw.RecommendedLineup),
            raw.CurrentTotal,
            raw.RecommendedTotal,
            raw.Delta,
            (raw.Swaps ?? []).Select(s => new LineupSwap(s.Position ?? "", s.From ?? "", s.To ?? "", s.Gain)).ToList(),
            notes,
            raw.PlatoonHints ?? [],
            new LineupChart(
                raw.Chart?.Labels ?? [],
                raw.Chart?.Current ?? [],
                raw.Chart?.Recommended ?? []),
            raw.PoolSize,
            summary,
            raw.Engine);
    }

    // --- Extension extra parsing ---

    private static Dictionary<string, List<string>> ParseEligibility(string? json)
    {
        if (string.IsNullOrWhiteSpace(json)) return [];
        try
        {
            return System.Text.Json.JsonSerializer.Deserialize<Dictionary<string, List<string>>>(json) ?? [];
        }
        catch
        {
            return [];
        }
    }

    private static List<string> ParseRosterPool(string? json)
    {
        if (string.IsNullOrWhiteSpace(json)) return [];
        try
        {
            return System.Text.Json.JsonSerializer.Deserialize<List<string>>(json) ?? [];
        }
        catch
        {
            return [];
        }
    }

    // --- DTO mapping and summary ---

    private static IReadOnlyList<LineupSlotResult> MapSlots(List<McpBridgeClient.SlotDto>? slots) =>
        (slots ?? []).Select(s => new LineupSlotResult(
            s.Order,
            s.Position ?? "",
            s.Player ?? "",
            s.Rc600,
            s.Def,
            s.Total,
            s.Salary,
            s.InPool,
            s.Ops,
            s.Obp,
            s.Hrf,
            s.BatPlat,
            s.Run ?? "",
            s.Injury ?? "",
            s.RangeGrade ?? "",
            s.Err)).ToList();

    private static string BuildSummary(McpBridgeClient.LineupAnalyzeBridgeResult raw, string name)
    {
        var sb = new StringBuilder();
        sb.AppendLine($"**Lineup Lab — {name}** ({raw.PitcherSide})");
        sb.AppendLine();
        sb.AppendLine($"| | Current | Recommended |");
        sb.AppendLine($"|--|---------|-------------|");
        sb.AppendLine($"| **RC600+Def total** | {raw.CurrentTotal:F1} | {raw.RecommendedTotal:F1} ({raw.Delta:+0.0;-0.0}) |");
        sb.AppendLine();
        foreach (var n in raw.Notes ?? [])
            sb.AppendLine($"- {n}");
        if (raw.PlatoonHints?.Count > 0)
        {
            sb.AppendLine();
            sb.AppendLine("**Platoon:**");
            foreach (var h in raw.PlatoonHints)
                sb.AppendLine($"- {h}");
        }
        sb.AppendLine();
        sb.AppendLine("**Recommended order:**");
        foreach (var s in raw.RecommendedLineup ?? [])
            sb.AppendLine($"{s.Order}. {s.Player} ({s.Position}) — RC600 {s.Rc600:F0} +{s.Def:F0} def");
        return sb.ToString().TrimEnd();
    }
}
