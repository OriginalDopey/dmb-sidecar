namespace DmbSidecar.Api.Configuration;

public sealed class FoundryOptions
{
    public const string SectionName = "Foundry";

    /// <summary>Project endpoint, e.g. https://....services.ai.azure.com/api/projects/skillfestFoundry</summary>
    public string ProjectEndpoint { get; set; } = "";

    /// <summary>OpenAI-compatible responses URL for the agent. If empty, built from ProjectEndpoint + AgentName.</summary>
    public string? ResponsesEndpoint { get; set; }

    public string AgentName { get; set; } = "dmb-front-office";

    public string AgentVersion { get; set; } = "1";

    /// <summary>Azure AD scope for token.</summary>
    public string Scope { get; set; } = "https://cognitiveservices.azure.com/.default";

    public string BuildResponsesUrl()
    {
        if (!string.IsNullOrWhiteSpace(ResponsesEndpoint))
            return ResponsesEndpoint.TrimEnd('/');

        var baseUrl = ProjectEndpoint.TrimEnd('/');
        return $"{baseUrl}/agents/{AgentName}/endpoint/protocols/openai/responses";
    }
}

public sealed class McpBridgeOptions
{
    public const string SectionName = "McpBridge";
    public string BaseUrl { get; set; } = "http://127.0.0.1:8765";
}

public sealed class ApiSecurityOptions
{
    public const string SectionName = "Security";
    public string ApiKey { get; set; } = "dev-key-change-me";
}
