using System.Text;
using DmbSidecar.Api.Models;

namespace DmbSidecar.Api.Services.LineupExplain.Handlers;

/// <summary>
/// Overall why the recommendation differs — tune high-level story here.
/// </summary>
internal sealed class RecommendationSummaryHandler : ILineupExplainHandler
{
    public LineupQuestionKind Kind => LineupQuestionKind.RecommendationSummary;

    public string Build(LineupExplainContext ctx)
    {
        var sb = new StringBuilder();
        var lineup = ctx.Lineup;

        sb.AppendLine(
            $"**Recommendation summary** {LineupExplainHelpers.SideLabel(lineup.PitcherSide)} — " +
            $"current {lineup.CurrentTotal:F1} · recommended {lineup.RecommendedTotal:F1} · Δ{lineup.Delta:+0.0;-0.0} RC+def.");

        var swaps = lineup.Swaps
            .Where(s => Math.Abs(s.Gain) > 0.5)
            .OrderByDescending(s => Math.Abs(s.Gain))
            .Take(5)
            .ToList();

        if (swaps.Count == 0)
        {
            sb.AppendLine("No material position swaps — mostly batting-order or marginal tweaks.");
            return sb.ToString().TrimEnd();
        }

        sb.AppendLine();
        sb.AppendLine("Top moves:");
        foreach (var s in swaps)
            sb.AppendLine($"• {s.Position}: {s.From} → {s.To} ({s.Gain:+0.0;-0.0})");

        return sb.ToString().TrimEnd();
    }
}
