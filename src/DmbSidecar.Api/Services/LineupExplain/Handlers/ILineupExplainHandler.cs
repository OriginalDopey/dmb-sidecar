namespace DmbSidecar.Api.Services.LineupExplain.Handlers;

/// <summary>
/// Strategy contract for offline lineup explain answers.
/// Each implementation handles one <see cref="LineupQuestionKind"/> and produces markdown for the side panel.
/// </summary>
internal interface ILineupExplainHandler
{
    /// <summary>Question family this handler serves.</summary>
    LineupQuestionKind Kind { get; }

    /// <summary>Builds the markdown answer from classified context and lineup analysis data.</summary>
    string Build(LineupExplainContext ctx);
}
