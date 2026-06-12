using System.Net.Http.Json;
using DmbSidecar.Api.Configuration;
using Microsoft.Extensions.Options;

namespace DmbSidecar.Api.Services;

/// <summary>
/// Typed HTTP client for the local Python MCP bridge.
/// Supplies cached team/league reports and delegates lineup optimization to the bridge's Python engine.
/// All read methods fail soft (null/false) so callers can degrade gracefully when the bridge is down.
/// </summary>
public sealed class McpBridgeClient
{
    private readonly HttpClient _http;
    private readonly ILogger<McpBridgeClient> _log;

    /// <summary>Configures base address from <see cref="McpBridgeOptions.BaseUrl"/>.</summary>
    public McpBridgeClient(HttpClient http, IOptions<McpBridgeOptions> options, ILogger<McpBridgeClient> log)
    {
        _http = http;
        _log = log;
        _http.BaseAddress = new Uri(options.Value.BaseUrl.TrimEnd('/') + "/");
    }

    // --- Health ---

    /// <summary>True when the bridge responds successfully to <c>GET /health</c>.</summary>
    public async Task<bool> IsHealthyAsync(CancellationToken ct = default)
    {
        try
        {
            var res = await _http.GetAsync("health", ct);
            return res.IsSuccessStatusCode;
        }
        catch (Exception ex)
        {
            _log.LogWarning(ex, "MCP bridge health check failed");
            return false;
        }
    }

    // --- Cached reports ---

    /// <summary>Text snapshot for the scoped team, or null on failure or missing cache.</summary>
    public async Task<string?> GetTeamSnapshotAsync(string? entryTeamId = null, CancellationToken ct = default)
    {
        try
        {
            var path = BuildScopedPath("report/team_snapshot", entryTeamId);
            var doc = await _http.GetFromJsonAsync<SnapshotDto>(path, ct);
            return doc?.Text;
        }
        catch (Exception ex)
        {
            _log.LogWarning(ex, "team_snapshot failed");
            return null;
        }
    }

    /// <summary>League summary text for the scoped team, or null on failure or missing cache.</summary>
    public async Task<string?> GetLeagueSummaryAsync(string? entryTeamId = null, CancellationToken ct = default)
    {
        try
        {
            var path = BuildScopedPath("report/league_summary", entryTeamId);
            var doc = await _http.GetFromJsonAsync<SnapshotDto>(path, ct);
            return doc?.Text;
        }
        catch (Exception ex)
        {
            _log.LogWarning(ex, "league_summary failed");
            return null;
        }
    }

    private static string BuildScopedPath(string endpoint, string? entryTeamId) =>
        string.IsNullOrWhiteSpace(entryTeamId)
            ? endpoint
            : $"{endpoint}?team_id={Uri.EscapeDataString(entryTeamId)}";

    // --- Lineup engine ---

    /// <summary>Posts lineup context to the bridge optimizer; returns null when the call fails.</summary>
    public async Task<LineupAnalyzeBridgeResult?> AnalyzeLineupAsync(
        LineupAnalyzeBridgeRequest body,
        CancellationToken ct = default)
    {
        try
        {
            var res = await _http.PostAsJsonAsync("lineup/analyze", body, ct);
            res.EnsureSuccessStatusCode();
            return await res.Content.ReadFromJsonAsync<LineupAnalyzeBridgeResult>(cancellationToken: ct);
        }
        catch (Exception ex)
        {
            _log.LogWarning(ex, "lineup/analyze failed");
            return null;
        }
    }

    // --- Bridge DTOs ---

    private sealed record SnapshotDto(string Text);

    /// <summary>Request body shape expected by the MCP bridge lineup endpoint.</summary>
    public sealed record LineupAnalyzeBridgeRequest(
        string PitcherSide,
        List<LineupSlotBridge> CurrentLineup,
        List<string> RosterNames,
        string LineupName,
        Dictionary<string, List<string>>? PositionEligibility = null);

    /// <summary>Single slot in a bridge lineup analyze request.</summary>
    public sealed record LineupSlotBridge(int Order, string? PlayerName, string? Position);

    /// <summary>Raw lineup optimization result deserialized from the MCP bridge.</summary>
    public sealed class LineupAnalyzeBridgeResult
    {
        /// <summary>Lineup name from the bridge or request.</summary>
        public string? LineupName { get; set; }

        /// <summary>Opposing pitcher handedness (e.g. lhp, rhp).</summary>
        public string? PitcherSide { get; set; }

        /// <summary>Current lineup slots with RC600 and defensive values.</summary>
        public List<SlotDto>? CurrentLineup { get; set; }

        /// <summary>Optimizer-recommended lineup slots.</summary>
        public List<SlotDto>? RecommendedLineup { get; set; }

        /// <summary>Sum of RC600+def for the current lineup.</summary>
        public double CurrentTotal { get; set; }

        /// <summary>Sum of RC600+def for the recommended lineup.</summary>
        public double RecommendedTotal { get; set; }

        /// <summary>Recommended minus current total.</summary>
        public double Delta { get; set; }

        /// <summary>Position-level player changes with gain estimates.</summary>
        public List<SwapDto>? Swaps { get; set; }

        /// <summary>Human-readable optimizer notes.</summary>
        public List<string>? Notes { get; set; }

        /// <summary>Platoon split suggestions from the engine.</summary>
        public List<string>? PlatoonHints { get; set; }

        /// <summary>Chart series for side-by-side slot totals.</summary>
        public ChartDto? Chart { get; set; }

        /// <summary>Count of eligible players in the optimization pool.</summary>
        public int PoolSize { get; set; }

        /// <summary>Engine identifier string from the bridge.</summary>
        public string? Engine { get; set; }
    }

    /// <summary>Per-slot metrics returned by the lineup engine.</summary>
    public sealed class SlotDto
    {
        /// <summary>Batting order 1–9.</summary>
        public int Order { get; set; }

        /// <summary>Defensive position abbreviation.</summary>
        public string? Position { get; set; }

        /// <summary>Player display name (Last, First).</summary>
        public string? Player { get; set; }

        /// <summary>Runs created per 600 PA for the platoon split.</summary>
        public double Rc600 { get; set; }

        /// <summary>Defensive runs value at assigned position.</summary>
        public double Def { get; set; }

        /// <summary>Combined RC600+def score used by the optimizer.</summary>
        public double Total { get; set; }

        /// <summary>Player salary in dollars.</summary>
        public int Salary { get; set; }

        /// <summary>True when the player was in the eligible roster pool.</summary>
        public bool InPool { get; set; }

        /// <summary>OPS for the platoon split.</summary>
        public double Ops { get; set; }

        /// <summary>OBP for the platoon split.</summary>
        public double Obp { get; set; }

        /// <summary>Home run factor for the platoon split.</summary>
        public double Hrf { get; set; }

        /// <summary>Platoon differential indicator from the pool CSV.</summary>
        public int BatPlat { get; set; }

        /// <summary>Run rating string from the pool.</summary>
        public string? Run { get; set; }

        /// <summary>Injury rating string from the pool.</summary>
        public string? Injury { get; set; }

        /// <summary>Range grade at assigned position.</summary>
        public string? RangeGrade { get; set; }

        /// <summary>Error rating at assigned position.</summary>
        public int? Err { get; set; }
    }

    /// <summary>Single position swap between current and recommended lineups.</summary>
    public sealed class SwapDto
    {
        /// <summary>Position where the swap occurs.</summary>
        public string? Position { get; set; }

        /// <summary>Player removed from the slot.</summary>
        public string? From { get; set; }

        /// <summary>Player inserted into the slot.</summary>
        public string? To { get; set; }

        /// <summary>Estimated RC+def gain from the swap.</summary>
        public double Gain { get; set; }
    }

    /// <summary>Chart payload for current vs recommended slot totals.</summary>
    public sealed class ChartDto
    {
        /// <summary>Slot or player labels for the chart axis.</summary>
        public List<string>? Labels { get; set; }

        /// <summary>Current lineup total per label.</summary>
        public List<double>? Current { get; set; }

        /// <summary>Recommended lineup total per label.</summary>
        public List<double>? Recommended { get; set; }
    }
}
