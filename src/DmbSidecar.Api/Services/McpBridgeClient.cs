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

    public async Task<string?> GetTeamSnapshotAsync(CancellationToken ct = default)
    {
        try
        {
            var doc = await _http.GetFromJsonAsync<SnapshotDto>("report/team_snapshot", ct);
            return doc?.Text;
        }
        catch (Exception ex)
        {
            _log.LogWarning(ex, "team_snapshot failed");
            return null;
        }
    }

    public async Task<string?> GetLeagueSummaryAsync(CancellationToken ct = default)
    {
        try
        {
            var doc = await _http.GetFromJsonAsync<SnapshotDto>("report/league_summary", ct);
            return doc?.Text;
        }
        catch (Exception ex)
        {
            _log.LogWarning(ex, "league_summary failed");
            return null;
        }
    }

    private sealed record SnapshotDto(string Text);
}
