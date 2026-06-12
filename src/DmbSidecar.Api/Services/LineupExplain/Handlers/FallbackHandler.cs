using System.Text;
using DmbSidecar.Api.Models;

namespace DmbSidecar.Api.Services.LineupExplain.Handlers;

/// <summary>
/// Last-resort handler when classification yields <see cref="LineupQuestionKind.Fallback"/>.
/// Delegates to position assignment when one player is named; otherwise summarizes recommendation delta.
/// </summary>
internal sealed class FallbackHandler : ILineupExplainHandler
{
    /// <inheritdoc />
    public LineupQuestionKind Kind => LineupQuestionKind.Fallback;

    /// <inheritdoc />
    public string Build(LineupExplainContext ctx)
    {
        var sb = new StringBuilder();
        var lineup = ctx.Lineup;

        if (ctx.Intent.Players.Count == 1)
            return new PositionAssignmentHandler().Build(ctx with
            {
                Intent = ctx.Intent with { Kind = LineupQuestionKind.PositionAssignment }
            });

        if (Math.Abs(lineup.Delta) < 3)
        {
            sb.AppendLine($"Lineups are essentially even on RC600+def ({lineup.Delta:+0.0;-0.0} total).");
            return sb.ToString().TrimEnd();
        }

        return new RecommendationSummaryHandler().Build(ctx with
        {
            Intent = ctx.Intent with { Kind = LineupQuestionKind.RecommendationSummary }
        });
    }
}
