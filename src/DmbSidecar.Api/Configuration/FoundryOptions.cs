namespace DmbSidecar.Api.Configuration;

/// <summary>
/// Strongly typed configuration for Microsoft Foundry agent invocation.
/// Bound from the <c>Foundry</c> appsettings section; consumed by <see cref="Services.FoundryAgentService"/>.
/// </summary>
public sealed class FoundryOptions
{
    /// <summary>Configuration section name in appsettings.</summary>
    public const string SectionName = "Foundry";

    /// <summary>Project endpoint, e.g. https://....services.ai.azure.com/api/projects/skillfestFoundry</summary>
    public string ProjectEndpoint { get; set; } = "";

    /// <summary>OpenAI-compatible responses URL for the agent. If empty, built from ProjectEndpoint + AgentName.</summary>
    public string? ResponsesEndpoint { get; set; }

    /// <summary>Foundry agent resource name passed in the request body.</summary>
    public string AgentName { get; set; } = "dmb-front-office";

    /// <summary>Agent version string for the Foundry agent reference payload.</summary>
    public string AgentVersion { get; set; } = "1";

    /// <summary>Azure API version appended to the agent-specific endpoint URL.</summary>
    public string ApiVersion { get; set; } = "2025-11-15-preview";

    /// <summary>
    /// Resolves the agent-specific Responses API URL.
    /// Prefers explicit <see cref="ResponsesEndpoint"/>; otherwise derives from project endpoint + agent name.
    /// </summary>
    public string BuildResponsesUrl()
    {
        if (!string.IsNullOrWhiteSpace(ResponsesEndpoint))
            return ResponsesEndpoint.TrimEnd('/');

        var baseUrl = ProjectEndpoint.TrimEnd('/');
        return $"{baseUrl}/agents/{AgentName}/endpoint/protocols/openai/responses?api-version={ApiVersion}";
    }
}

/// <summary>
/// HTTP client settings for the local Python MCP bridge (cached ImagineSports data, lineup engine).
/// Bound from the <c>McpBridge</c> section; consumed by <see cref="Services.McpBridgeClient"/>.
/// </summary>
public sealed class McpBridgeOptions
{
    /// <summary>Configuration section name in appsettings.</summary>
    public const string SectionName = "McpBridge";

    /// <summary>Base URL of the MCP bridge process (default local dev port).</summary>
    public string BaseUrl { get; set; } = "http://127.0.0.1:8765";
}

/// <summary>
/// API key shared between the Chrome extension and this host.
/// Bound from the <c>Security</c> section; enforced by <see cref="Middleware.ApiKeyMiddleware"/>.
/// </summary>
public sealed class ApiSecurityOptions
{
    /// <summary>Configuration section name in appsettings.</summary>
    public const string SectionName = "Security";

    /// <summary>Expected value of the <c>X-Api-Key</c> request header.</summary>
    public string ApiKey { get; set; } = "dev-key-change-me";
}
