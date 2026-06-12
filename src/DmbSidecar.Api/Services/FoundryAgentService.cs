using System.Diagnostics;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using Azure.Core;
using Azure.Identity;
using DmbSidecar.Api.Configuration;
using Microsoft.Extensions.Options;

namespace DmbSidecar.Api.Services;

/// <summary>
/// HTTP client for Microsoft Foundry agents via the OpenAI-compatible Responses API.
/// Uses <see cref="DefaultAzureCredential"/> for bearer tokens; same agent_reference body pattern as the Python reference client.
/// Callers: <see cref="AdviseService"/>, <see cref="LineupExplainService"/>, and the <c>/foundry/smoke</c> probe.
/// </summary>
public sealed class FoundryAgentService
{
    private readonly HttpClient _http;
    private readonly FoundryOptions _options;
    private readonly DefaultAzureCredential _credential;
    private readonly ILogger<FoundryAgentService> _log;

    private static readonly JsonSerializerOptions JsonOpts = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.SnakeCaseLower,
        DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
    };

    /// <summary>Creates the service with typed HTTP client and Foundry configuration.</summary>
    public FoundryAgentService(
        HttpClient http,
        IOptions<FoundryOptions> options,
        ILogger<FoundryAgentService> log)
    {
        _http = http;
        _options = options.Value;
        _credential = new DefaultAzureCredential();
        _log = log;
    }

    /// <summary>True when either project or responses endpoint is configured.</summary>
    public bool IsConfigured =>
        !string.IsNullOrWhiteSpace(_options.ProjectEndpoint) ||
        !string.IsNullOrWhiteSpace(_options.ResponsesEndpoint);

    /// <summary>
    /// Sends a single user message to the configured Foundry agent and returns extracted output text.
    /// Throws when not configured or when the HTTP response is non-success.
    /// </summary>
    public async Task<string> InvokeAsync(string userMessage, CancellationToken ct = default)
    {
        if (!IsConfigured)
            throw new InvalidOperationException(
                "Foundry is not configured. Set Foundry:ProjectEndpoint in appsettings or user secrets.");

        var token = await _credential.GetTokenAsync(
            new TokenRequestContext(new[] { _options.Scope }),
            ct);

        var url = _options.BuildResponsesUrl();
        var body = new
        {
            input = new[]
            {
                new { role = "user", content = userMessage },
            },
            agent_reference = new
            {
                name = _options.AgentName,
                version = _options.AgentVersion,
                type = "agent_reference",
            },
        };

        using var req = new HttpRequestMessage(HttpMethod.Post, url);
        req.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token.Token);
        req.Content = new StringContent(
            JsonSerializer.Serialize(body, JsonOpts),
            Encoding.UTF8,
            "application/json");

        _log.LogInformation("Foundry POST {Url} agent={Agent} v{Version}", url, _options.AgentName, _options.AgentVersion);

        var sw = Stopwatch.StartNew();
        using var res = await _http.SendAsync(req, ct);
        var raw = await res.Content.ReadAsStringAsync(ct);
        sw.Stop();

        if (!res.IsSuccessStatusCode)
        {
            _log.LogError("Foundry error {Status}: {Body}", res.StatusCode, raw[..Math.Min(raw.Length, 500)]);
            throw new HttpRequestException($"Foundry returned {(int)res.StatusCode}: {raw[..Math.Min(raw.Length, 200)]}");
        }

        _log.LogInformation("Foundry response in {Ms}ms", sw.ElapsedMilliseconds);

        return ExtractOutputText(raw);
    }

    // --- Response parsing ---

    private static string ExtractOutputText(string raw)
    {
        try
        {
            using var doc = JsonDocument.Parse(raw);
            var root = doc.RootElement;

            if (root.TryGetProperty("output_text", out var ot))
                return ot.GetString() ?? raw;

            if (root.TryGetProperty("output", out var output) && output.ValueKind == JsonValueKind.Array)
            {
                var sb = new StringBuilder();
                foreach (var item in output.EnumerateArray())
                {
                    if (item.TryGetProperty("content", out var content) && content.ValueKind == JsonValueKind.Array)
                    {
                        foreach (var part in content.EnumerateArray())
                        {
                            if (part.TryGetProperty("text", out var text))
                                sb.Append(text.GetString());
                        }
                    }
                }
                if (sb.Length > 0)
                    return sb.ToString();
            }
        }
        catch (JsonException)
        {
            // fall through
        }

        return raw;
    }
}
