using System.Net.Http.Json;
using DmbSidecar.Api.Configuration;
using Microsoft.Extensions.Options;

namespace DmbSidecar.Api.Services;

public sealed class McpBridgeClient
{
    private readonly HttpClient _http;
    private readonly ILogger<McpBridgeClient> _log;

    public McpBridgeClient(HttpClient http, IOptions<McpBridgeOptions> options, ILogger<McpBridgeClient> log)
    {
        _http = http;
        _log = log;
        _http.BaseAddress = new Uri(options.Value.BaseUrl.TrimEnd('/') + "/");
    }

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

    private sealed record SnapshotDto(string Text);

    public sealed record LineupAnalyzeBridgeRequest(
        string PitcherSide,
        List<LineupSlotBridge> CurrentLineup,
        List<string> RosterNames,
        string LineupName,
        Dictionary<string, List<string>>? PositionEligibility = null);

    public sealed record LineupSlotBridge(int Order, string? PlayerName, string? Position);

    public sealed class LineupAnalyzeBridgeResult
    {
        public string? LineupName { get; set; }
        public string? PitcherSide { get; set; }
        public List<SlotDto>? CurrentLineup { get; set; }
        public List<SlotDto>? RecommendedLineup { get; set; }
        public double CurrentTotal { get; set; }
        public double RecommendedTotal { get; set; }
        public double Delta { get; set; }
        public List<SwapDto>? Swaps { get; set; }
        public List<string>? Notes { get; set; }
        public List<string>? PlatoonHints { get; set; }
        public ChartDto? Chart { get; set; }
        public int PoolSize { get; set; }
        public string? Engine { get; set; }
    }

    public sealed class SlotDto
    {
        public int Order { get; set; }
        public string? Position { get; set; }
        public string? Player { get; set; }
        public double Rc600 { get; set; }
        public double Def { get; set; }
        public double Total { get; set; }
        public int Salary { get; set; }
        public bool InPool { get; set; }
        public double Ops { get; set; }
        public double Obp { get; set; }
        public double Hrf { get; set; }
        public int BatPlat { get; set; }
        public string? Run { get; set; }
        public string? Injury { get; set; }
        public string? RangeGrade { get; set; }
        public int? Err { get; set; }
    }

    public sealed class SwapDto
    {
        public string? Position { get; set; }
        public string? From { get; set; }
        public string? To { get; set; }
        public double Gain { get; set; }
    }

    public sealed class ChartDto
    {
        public List<string>? Labels { get; set; }
        public List<double>? Current { get; set; }
        public List<double>? Recommended { get; set; }
    }
}
