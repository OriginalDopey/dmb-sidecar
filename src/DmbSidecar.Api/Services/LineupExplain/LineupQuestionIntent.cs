namespace DmbSidecar.Api.Services.LineupExplain;

public sealed record LineupQuestionIntent(
    LineupQuestionKind Kind,
    string RawQuestion,
    IReadOnlyList<string> Players,
    string? Position,
    int? BattingOrder,
    string? CompareToPlayer);
