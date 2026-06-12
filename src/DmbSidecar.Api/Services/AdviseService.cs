using System.Diagnostics;
using System.Text;
using System.Text.Json;
using DmbSidecar.Api.Models;

namespace DmbSidecar.Api.Services;

public sealed class AdviseService
{
    private readonly FoundryAgentService _foundry;
    private readonly McpBridgeClient _mcp;
    private readonly ILogger<AdviseService> _log;

    public AdviseService(FoundryAgentService foundry, McpBridgeClient mcp, ILogger<AdviseService> log)
    {
        _foundry = foundry;
        _mcp = mcp;
        _log = log;
    }

    public async Task<AdviseResponse> AdviseAsync(AdviseRequest request, CancellationToken ct = default)
    {
        var sw = Stopwatch.StartNew();
        var citations = new List<Citation>();
        var warnings = new List<string>();

        string? teamSnapshot = null;
        string? leagueSummary = null;

        if (await _mcp.IsHealthyAsync(ct))
        {
            teamSnapshot = await _mcp.GetTeamSnapshotAsync(ct);
            leagueSummary = await _mcp.GetLeagueSummaryAsync(ct);
            if (teamSnapshot != null)
                citations.Add(new Citation("mcp", "Team snapshot (cached DB)", Truncate(teamSnapshot, 200)));
            if (leagueSummary != null)
                citations.Add(new Citation("mcp", "League summary (cached DB)", Truncate(leagueSummary, 200)));
        }
        else
        {
            warnings.Add("MCP bridge unreachable — advice will use page context and IQ only.");
        }

        var prompt = BuildPrompt(request, teamSnapshot, leagueSummary);
        string answer;

        try
        {
            answer = await _foundry.InvokeAsync(prompt, ct);
            citations.Add(new Citation("foundry", $"Agent {_foundry.GetType().Name}", "Foundry IQ + instructions"));
        }
        catch (Exception ex)
        {
            _log.LogError(ex, "Foundry invoke failed");
            warnings.Add($"Foundry error: {ex.Message}");
            answer = BuildOfflineFallback(request, teamSnapshot, leagueSummary, ex.Message);
        }

        sw.Stop();
        return new AdviseResponse(
            answer,
            citations,
            sw.ElapsedMilliseconds,
            warnings.Count > 0 ? string.Join(" ", warnings) : null);
    }

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

    private static string BuildOfflineFallback(
        AdviseRequest request,
        string? teamSnapshot,
        string? leagueSummary,
        string error)
    {
        var sb = new StringBuilder();
        sb.AppendLine("**Foundry agent unavailable** — offline scaffold response.");
        sb.AppendLine($"Error: {error}");
        sb.AppendLine();
        sb.AppendLine("Complete manual step: create agent `dmb-front-office` in Foundry portal (see docs/manual-steps/FOUNDRY_IQ_PORTAL.md).");
        sb.AppendLine();
        sb.AppendLine($"**Your question:** {request.Question}");
        sb.AppendLine($"**Page:** {request.Context.PageType} — {request.Context.Url}");
        if (request.Context.Slots?.Count > 0)
        {
            sb.AppendLine("**Lineup on screen:**");
            foreach (var s in request.Context.Slots)
                sb.AppendLine($"  {s.Order}. {s.PlayerName} ({s.Position})");
        }
        if (teamSnapshot != null)
        {
            sb.AppendLine();
            sb.AppendLine("**Cached roster/finance:**");
            sb.AppendLine(teamSnapshot);
        }
        if (leagueSummary != null)
        {
            sb.AppendLine();
            sb.AppendLine(leagueSummary);
        }
        return sb.ToString();
    }

    private static string Truncate(string s, int max) =>
        s.Length <= max ? s : s[..max] + "…";
}
