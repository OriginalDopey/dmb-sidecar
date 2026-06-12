using DmbSidecar.Api.Models;

namespace DmbSidecar.Api.Services.LineupExplain;

public sealed record LineupExplainContext(
    string Question,
    LineupQuestionIntent Intent,
    PageContext PageContext,
    LineupAnalyzeResponse Lineup,
    IReadOnlyList<string> IqSnippets);
