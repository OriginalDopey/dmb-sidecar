namespace DmbSidecar.Api.Models;

/// <summary>
/// Request/response contracts for general front-office advice and health probes.
/// Scraped page context travels with every call from the Chrome extension;
/// <see cref="AdviseResponse"/> is the shared answer envelope for both <c>/advise</c> and <c>/lineup/explain</c>.
/// </summary>

/// <summary>One scraped row from an ImagineSports page (lineup slot or roster line).</summary>
public sealed record PageSlot(
    int Order,
    string? Position,
    string? PlayerName,
    string? Bats,
    string? Salary = null,
    string? Section = null
);

/// <summary>Structured snapshot of the active browser tab sent with every API request.</summary>
public sealed record PageContext(
    string PageType,
    string Url,
    string? LineupName,
    string? CurTeam,
    IReadOnlyList<PageSlot>? Slots,
    IReadOnlyDictionary<string, string>? Extra
);

/// <summary>General front-office question with page context from the extension.</summary>
public sealed record AdviseRequest(
    string Question,
    PageContext Context
);

/// <summary>Provenance line shown in the side panel citations list.</summary>
public sealed record Citation(
    string Source,
    string Label,
    string? Snippet
);

/// <summary>
/// Answer payload returned to the Chrome side panel.
/// <see cref="QuestionKind"/> is set for Lineup Lab explain (e.g. DhAssignment, PositionComparison).
/// </summary>
public sealed record AdviseResponse(
    string Answer,
    IReadOnlyList<Citation> Citations,
    long ElapsedMs,
    string? Warning,
    string? QuestionKind = null
);

/// <summary>Aggregate readiness report for <c>GET /health</c>.</summary>
public sealed record HealthResponse(
    string Status,
    bool FoundryConfigured,
    bool McpBridgeReachable,
    string Version
);
