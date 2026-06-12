namespace DmbSidecar.Api.Models;

public sealed record PageSlot(
    int Order,
    string? Position,
    string? PlayerName,
    string? Bats
);

public sealed record PageContext(
    string PageType,
    string Url,
    string? LineupName,
    string? CurTeam,
    IReadOnlyList<PageSlot>? Slots,
    IReadOnlyDictionary<string, string>? Extra
);

public sealed record AdviseRequest(
    string Question,
    PageContext Context
);

public sealed record Citation(
    string Source,
    string Label,
    string? Snippet
);

public sealed record AdviseResponse(
    string Answer,
    IReadOnlyList<Citation> Citations,
    long ElapsedMs,
    string? Warning
);

public sealed record HealthResponse(
    string Status,
    bool FoundryConfigured,
    bool McpBridgeReachable,
    string Version
);
