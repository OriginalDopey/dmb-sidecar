using System.Diagnostics;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using DmbSidecar.Api.Configuration;
using Microsoft.Extensions.Options;

namespace DmbSidecar.Api.Services;

/// <summary>
/// HTTP client for Microsoft Foundry agents via the OpenAI-compatible Responses API.
/// <para>
/// Authentication uses the Azure CLI credential (<c>az account get-access-token</c>) to acquire
/// a bearer token with the <c>https://ai.azure.com</c> audience required by the Foundry
/// agent-specific endpoint. This sidesteps known .NET <c>DefaultAzureCredential</c> token-audience
/// mismatches on macOS when targeting Foundry v1 project endpoints.
/// </para>
/// <para>
/// The request body follows the same <c>agent_reference</c> pattern used by the
/// <c>azure-ai-projects</c> Python SDK's <c>openai_client.responses.create()</c>.
/// </para>
/// </summary>
/// <remarks>
/// Callers: <see cref="AdviseService"/>, <see cref="LineupExplainService"/>,
/// and the <c>/foundry/smoke</c> diagnostic probe.
/// </remarks>
public sealed class FoundryAgentService
{
    private readonly HttpClient _http;
    private readonly FoundryOptions _options;
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
        _log = log;
    }

    /// <summary>True when either project or responses endpoint is configured.</summary>
    public bool IsConfigured =>
        !string.IsNullOrWhiteSpace(_options.ProjectEndpoint) ||
        !string.IsNullOrWhiteSpace(_options.ResponsesEndpoint);

    /// <summary>
    /// Sends a single user message to the configured Foundry agent and returns the extracted output text.
    /// </summary>
    /// <param name="userMessage">Natural-language prompt forwarded to the agent.</param>
    /// <param name="ct">Cancellation token.</param>
    /// <returns>Agent response text (may include citation markers like <c>【4:0†source】</c>).</returns>
    /// <exception cref="InvalidOperationException">Foundry endpoint not configured or CLI auth fails.</exception>
    /// <exception cref="HttpRequestException">Foundry returns a non-success HTTP status.</exception>
    public async Task<string> InvokeAsync(string userMessage, CancellationToken ct = default)
    {
        if (!IsConfigured)
            throw new InvalidOperationException(
                "Foundry is not configured. Set Foundry:ProjectEndpoint in appsettings or user secrets.");

        var token = await AcquireTokenAsync();

        var url = _options.BuildResponsesUrl();
        var body = new
        {
            input = new[] { new { role = "user", content = userMessage } },
            stream = false,
            agent_reference = new
            {
                name = _options.AgentName,
                version = _options.AgentVersion,
                type = "agent_reference",
            },
        };

        using var req = new HttpRequestMessage(HttpMethod.Post, url);
        req.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
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

    // ─── Token Acquisition ───────────────────────────────────────────────────────

    /// <summary>
    /// Acquires a bearer token via the Azure CLI. The Foundry agent-specific endpoint
    /// requires the <c>https://ai.azure.com</c> audience, which the CLI produces correctly.
    /// </summary>
    private static async Task<string> AcquireTokenAsync()
    {
        const string resource = "https://ai.azure.com";

        var psi = new ProcessStartInfo("az",
            $"account get-access-token --resource {resource} --query accessToken -o tsv")
        {
            RedirectStandardOutput = true,
            RedirectStandardError = true,
            UseShellExecute = false,
            CreateNoWindow = true,
        };

        using var proc = Process.Start(psi)
            ?? throw new InvalidOperationException("Could not start 'az' CLI process.");

        var token = (await proc.StandardOutput.ReadToEndAsync()).Trim();
        await proc.WaitForExitAsync();

        if (proc.ExitCode != 0 || string.IsNullOrEmpty(token))
            throw new InvalidOperationException(
                "Failed to acquire Azure CLI token. Ensure 'az login' has been run and the Skillsfest subscription is active.");

        return token;
    }

    // ─── Response Parsing ────────────────────────────────────────────────────────

    /// <summary>
    /// Extracts human-readable text from the Foundry Responses API JSON.
    /// Handles both the top-level <c>output_text</c> shortcut and the nested
    /// <c>output[].content[].text</c> structure.
    /// </summary>
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
            // Unparseable response — return raw body as fallback
        }

        return raw;
    }
}
