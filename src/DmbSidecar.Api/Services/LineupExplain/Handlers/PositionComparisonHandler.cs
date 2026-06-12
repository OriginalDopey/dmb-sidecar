using System.Text;
using DmbSidecar.Api.Models;

namespace DmbSidecar.Api.Services.LineupExplain.Handlers;

/// <summary>
/// Compares two players at the same position (defense and bat side-by-side).
/// </summary>
internal sealed class PositionComparisonHandler : ILineupExplainHandler
{
    public LineupQuestionKind Kind => LineupQuestionKind.PositionComparison;

    public string Build(LineupExplainContext ctx)
    {
        var sb = new StringBuilder();
        var lineup = ctx.Lineup;
        var intent = ctx.Intent;
        var players = intent.Players;

        if (players.Count < 2)
        {
            sb.AppendLine("Name two players — e.g. why Knight over Mackanin at SS?");
            return sb.ToString().TrimEnd();
        }

        var favored = players[0];
        var other = intent.CompareToPlayer ?? players[1];
        var position = intent.Position
            ?? LineupExplainHelpers.FindSlot(lineup.RecommendedLineup, LineupExplainHelpers.Norm(favored), null)?.Position
            ?? LineupExplainHelpers.FindSlot(lineup.CurrentLineup, LineupExplainHelpers.Norm(favored), null)?.Position
            ?? "SS";

        var recFavored = LineupExplainHelpers.FindSlot(lineup.RecommendedLineup, LineupExplainHelpers.Norm(favored), position)
            ?? LineupExplainHelpers.FindSlot(lineup.RecommendedLineup, LineupExplainHelpers.Norm(favored), null);
        var recOther = LineupExplainHelpers.FindSlot(lineup.RecommendedLineup, LineupExplainHelpers.Norm(other), position)
            ?? LineupExplainHelpers.FindSlot(lineup.RecommendedLineup, LineupExplainHelpers.Norm(other), null);
        var curFavored = LineupExplainHelpers.FindSlot(lineup.CurrentLineup, LineupExplainHelpers.Norm(favored), null);
        var curOther = LineupExplainHelpers.FindSlot(lineup.CurrentLineup, LineupExplainHelpers.Norm(other), null);

        sb.AppendLine($"**Position comparison** — {position}: {favored} vs {other}.");

        void Line(string label, LineupSlotResult? slot)
        {
            if (slot == null) return;
            sb.AppendLine(
                $"{label}: {slot.Player} at {slot.Position} (#{slot.Order}, RC{slot.Rc600:F0}, def {slot.Def:+0.0;-0.0}, total {slot.Total:F1}).");
        }

        sb.AppendLine();
        Line("Recommended", recFavored);
        Line("Recommended", recOther);
        if (curFavored != null || curOther != null)
        {
            sb.AppendLine();
            Line("Your lineup", curFavored);
            Line("Your lineup", curOther);
        }

        if (recFavored != null && recOther != null)
        {
            sb.AppendLine();
            sb.AppendLine(
                $"Model prefers **{recFavored.Player}** at {recFavored.Position} " +
                $"(Δdef {recFavored.Def - recOther.Def:+0.0;-0.0}, ΔRC {recFavored.Rc600 - recOther.Rc600:+0.0;-0.0} {LineupExplainHelpers.SideLabel(lineup.PitcherSide)}).");
        }

        return sb.ToString().TrimEnd();
    }
}
