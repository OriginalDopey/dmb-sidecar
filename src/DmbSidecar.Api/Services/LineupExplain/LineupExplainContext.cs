using DmbSidecar.Api.Models;

namespace DmbSidecar.Api.Services.LineupExplain;

/// <summary>
/// Immutable input bundle passed to offline lineup explain handlers.
/// Carries the classified intent, precomputed analyze response, and IQ snippets for optional citation.
/// </summary>
/// <param name="Question">Original user question from the side panel.</param>
/// <param name="Intent">Classifier output: kind, extracted players, position, and batting-order slot.</param>
/// <param name="PageContext">Browser-scraped Edit Lineup page context.</param>
/// <param name="Lineup">Current vs recommended lineup comparison from the MCP engine.</param>
/// <param name="IqSnippets">Local IQ markdown snippets relevant to lineup construction.</param>
public sealed record LineupExplainContext(
    string Question,
    LineupQuestionIntent Intent,
    PageContext PageContext,
    LineupAnalyzeResponse Lineup,
    IReadOnlyList<string> IqSnippets);
