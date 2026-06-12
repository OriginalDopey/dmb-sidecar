using System.Diagnostics;
using System.Text;
using System.Text.Json;
using DmbSidecar.Api.Models;
using DmbSidecar.Api.Utilities;

namespace DmbSidecar.Api.Services;

/// <summary>
/// Orchestrates general front-office advice for <c>POST /advise</c>.
/// Enriches the user question with MCP team/league snapshots (when fresh), builds a Foundry prompt,
/// and falls back to local IQ + <see cref="OfflineAdviseHelper"/> when the agent is unavailable.
/// Callers: Chrome extension side panel via minimal API in <c>Program.cs</c>.
/// </summary>
public sealed class AdviseService
{
    private readonly FoundryAgentService _foundry;
    private readonly McpBridgeClient _mcp;
    private readonly LocalIqService _localIq;
    private readonly ILogger<AdviseService> _log;

    /// <summary>Creates the service with Foundry, MCP, and offline IQ dependencies.</summary>
    public AdviseService(
        FoundryAgentService foundry,
        McpBridgeClient mcp,
        LocalIqService localIq,
        ILogger<AdviseService> log)
    {
        _foundry = foundry;
        _mcp = mcp;
        _localIq = localIq;
        _log = log;
    }

    /// <summary>
    /// Produces an answer with citations and optional warnings.
    /// MCP data is filtered against the browser DOM to avoid stale-cache advice.
    /// </summary>
    public async Task<AdviseResponse> AdviseAsync(AdviseRequest request, CancellationToken ct = default)
    {
        var sw = Stopwatch.StartNew();
        var citations = new List<Citation>();
        var warnings = new List<string>();

        string? teamSnapshot = null;
        string? leagueSummary = null;

        var entryTeam = string.IsNullOrWhiteSpace(request.Context.CurTeam) ? null : request.Context.CurTeam;

        // --- MCP enrichment ---

        if (await _mcp.IsHealthyAsync(ct))
        {
            var rawSnapshot = await _mcp.GetTeamSnapshotAsync(entryTeam, ct);
            teamSnapshot = McpDataFilter.NormalizeSnapshot(rawSnapshot, request.Context);
            leagueSummary = McpDataFilter.NormalizeSummary(await _mcp.GetLeagueSummaryAsync(entryTeam, ct));
            if (teamSnapshot != null)
                citations.Add(new Citation("mcp", "Team snapshot (cached DB)", TextHelper.Truncate(teamSnapshot, 200)));
            if (leagueSummary != null)
                citations.Add(new Citation("mcp", "League summary (cached DB)", TextHelper.Truncate(leagueSummary, 200)));
            if (rawSnapshot != null && teamSnapshot == null && request.Context.Slots is { Count: > 0 })
                warnings.Add("MCP roster cache looks stale vs. this page — using browser roster.");
            else if (entryTeam != null && teamSnapshot == null && leagueSummary == null)
                warnings.Add($"No MCP cache for curTeam {entryTeam} — using on-page roster only.");
        }
        else
        {
            warnings.Add("MCP bridge unreachable — advice will use page context and IQ only.");
        }

        var prompt = BuildPrompt(request, teamSnapshot, leagueSummary);
        string answer;

        // --- Foundry invoke with offline fallback ---

        try
        {
            answer = await _foundry.InvokeAsync(prompt, ct);
            citations.Add(new Citation("foundry", $"Agent {_foundry.GetType().Name}", "Foundry IQ + instructions"));
        }
        catch (Exception ex)
        {
            _log.LogError(ex, "Foundry invoke failed");
            warnings.Add($"Foundry error: {ex.Message}");
            var iqSnippets = _localIq.Search(request.Question);
            if (iqSnippets.Count > 0)
                citations.Add(new Citation("local-iq", "iq-sources/ (offline)", TextHelper.Truncate(string.Join(" ", iqSnippets), 200)));
            answer = OfflineAdviseHelper.Build(request, teamSnapshot, leagueSummary, iqSnippets, ex.Message);
        }

        sw.Stop();
        return new AdviseResponse(
            answer,
            citations,
            sw.ElapsedMilliseconds,
            warnings.Count > 0 ? string.Join(" ", warnings) : null);
    }

    // --- Prompt construction ---

    private static string BuildPrompt(AdviseRequest request, string? teamSnapshot, string? leagueSummary)
    {
        var sb = new StringBuilder();
        sb.AppendLine("You are DMB Sidecar — a Classic Standard Diamond Mind Baseball front-office advisor.");
        sb.AppendLine("Ground mechanics in your knowledge base. Use live data blocks below when present.");
        sb.AppendLine("Cite whether advice comes from rules vs live league data. Never invent stats.");
        sb.AppendLine();
        sb.AppendLine($"## User question\n{request.Question}");
        sb.AppendLine();
        sb.AppendLine("## Page context (from browser)");
        sb.AppendLine(JsonSerializer.Serialize(request.Context, new JsonSerializerOptions { WriteIndented = true }));
        AppendPageRosterSummary(sb, request.Context);
        if (teamSnapshot != null)
        {
            sb.AppendLine();
            sb.AppendLine("## Live team snapshot (MCP)");
            sb.AppendLine(teamSnapshot);
        }
        if (leagueSummary != null)
        {
            sb.AppendLine();
            sb.AppendLine("## League summary (MCP)");
            sb.AppendLine(leagueSummary);
        }
        return sb.ToString();
    }

    private static void AppendPageRosterSummary(StringBuilder sb, PageContext context)
    {
        if (context.PageType != "roster" || context.Slots is not { Count: > 0 })
            return;

        sb.AppendLine();
        sb.AppendLine("## Roster on screen (browser DOM)");
        if (context.Extra != null)
        {
            if (context.Extra.TryGetValue("teamName", out var team)) sb.AppendLine($"Team: {team}");
            if (context.Extra.TryGetValue("totalValue", out var cap)) sb.AppendLine($"Total value: {cap}");
            if (context.Extra.TryGetValue("cashBalance", out var cash)) sb.AppendLine($"Cash: {cash}");
            if (context.Extra.TryGetValue("stadium", out var park)) sb.AppendLine($"Park: {park}");
        }

        foreach (var group in new[] { ("Hitters", "batter"), ("Pitchers", "pitcher"), ("IR", "ir") })
        {
            var rows = context.Slots
                .Where(s => string.Equals(s.Section, group.Item2, StringComparison.OrdinalIgnoreCase))
                .Take(30)
                .ToList();
            if (rows.Count == 0) continue;
            sb.AppendLine($"{group.Item1}:");
            foreach (var p in rows)
                sb.AppendLine($"  {p.Position} {p.PlayerName} {p.Salary}".Trim());
        }
    }

}
