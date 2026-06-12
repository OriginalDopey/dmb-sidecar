namespace DmbSidecar.Api.Services.LineupExplain.Handlers;

internal interface ILineupExplainHandler
{
    LineupQuestionKind Kind { get; }
    string Build(LineupExplainContext ctx);
}
