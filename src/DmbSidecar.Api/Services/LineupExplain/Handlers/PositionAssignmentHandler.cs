using System.Text;
using DmbSidecar.Api.Models;

namespace DmbSidecar.Api.Services.LineupExplain.Handlers;

/// <summary>
/// Explains why a player is assigned to a defensive position (RC+def at spot).
/// Delegates to <see cref="DhAssignmentHandler"/> when the question is really about DH parking.
/// </summary>
internal sealed class PositionAssignmentHandler : ILineupExplainHandler
{
    /// <inheritdoc />
    public LineupQuestionKind Kind => LineupQuestionKind.PositionAssignment;

    /// <inheritdoc />
    public string Build(LineupExplainContext ctx)
    {
        var sb = new StringBuilder();
        var lineup = ctx.Lineup;
        var intent = ctx.Intent;

        var player = intent.Players.Count > 0 ? intent.Players[0] : null;
        if (player == null)
        {
            sb.AppendLine("Name a player (last name is enough) and position — e.g. why Mackanin at SS?");
            return sb.ToString().TrimEnd();
        }

        var norm = LineupExplainHelpers.Norm(player);
        var position = intent.Position;
        var cur = LineupExplainHelpers.FindSlot(lineup.CurrentLineup, norm, position)
            ?? LineupExplainHelpers.FindSlot(lineup.CurrentLineup, norm, null);
        if (cur == null)
        {
            sb.AppendLine($"Could not find {player} in the scraped lineup.");
            return sb.ToString().TrimEnd();
        }

        position = cur.Position;
        var recAtPos = lineup.RecommendedLineup.FirstOrDefault(s =>
            string.Equals(s.Position, position, StringComparison.OrdinalIgnoreCase));
        var recPlayer = LineupExplainHelpers.FindSlot(lineup.RecommendedLineup, norm, null);

        sb.AppendLine($"**Position assignment** — {cur.Player} at {position}.");

        if (recAtPos != null && LineupExplainHelpers.Norm(recAtPos.Player) == norm)
        {
            sb.AppendLine($"Model agrees: keeps {cur.Player} at {position} (+{recAtPos.Def:F0} def, RC{recAtPos.Rc600:F0}).");
            if (recPlayer != null && recPlayer.Order != cur.Order)
                sb.AppendLine($"Only order changes: #{cur.Order} → #{recPlayer.Order}.");
            return sb.ToString().TrimEnd();
        }

        if (string.Equals(position, "DH", StringComparison.OrdinalIgnoreCase)
            && recPlayer != null
            && !string.Equals(recPlayer.Position, "DH", StringComparison.OrdinalIgnoreCase))
        {
            return new DhAssignmentHandler().Build(ctx with { Intent = ctx.Intent with { Kind = LineupQuestionKind.DhAssignment } });
        }

        if (recAtPos != null && recPlayer != null)
        {
            sb.AppendLine();
            sb.AppendLine(
                $"You: {cur.Player} at {position} (#{cur.Order}, RC{cur.Rc600:F0}, def {cur.Def:+0.0;-0.0}).");
            sb.AppendLine(
                $"Model: {recAtPos.Player} at {position} (RC{recAtPos.Rc600:F0}, def {recAtPos.Def:+0.0;-0.0}), " +
                $"{recPlayer.Player} at {recPlayer.Position} (#{recPlayer.Order}, def {recPlayer.Def:+0.0;-0.0}).");
            if (recPlayer.Def > cur.Def + 1)
                sb.AppendLine($"Defense swing: +{recPlayer.Def - cur.Def:F0} runs with {recPlayer.Player} at {recPlayer.Position}.");
            if (recAtPos.Rc600 > cur.Rc600 + 3)
                sb.AppendLine($"Bat {LineupExplainHelpers.SideLabel(lineup.PitcherSide)}: {recAtPos.Player} +{recAtPos.Rc600 - cur.Rc600:F0} RC in that slot.");
        }
        else
        {
            sb.AppendLine($"Small tweak at {position} — lineups within {Math.Abs(lineup.Delta):F1} RC+def total.");
        }

        return sb.ToString().TrimEnd();
    }
}
