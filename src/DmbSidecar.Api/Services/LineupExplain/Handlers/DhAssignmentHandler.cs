using System.Text;
using DmbSidecar.Api.Models;

namespace DmbSidecar.Api.Services.LineupExplain.Handlers;

/// <summary>
/// Explains DH assignment using defense-recovery logic (weakest glove parked at bat-only).
/// Compares current vs recommended DH and field positions for named players.
/// </summary>
internal sealed class DhAssignmentHandler : ILineupExplainHandler
{
    /// <inheritdoc />
    public LineupQuestionKind Kind => LineupQuestionKind.DhAssignment;

    /// <inheritdoc />
    public string Build(LineupExplainContext ctx)
    {
        var sb = new StringBuilder();
        var lineup = ctx.Lineup;
        var players = ctx.Intent.Players;

        if (players.Count == 0)
            players = LineupExplainHelpers.ExtractPlayersMentioned(ctx.Question, ctx.PageContext, lineup);

        if (players.Count == 0)
        {
            AppendDhOverview(sb, lineup, []);
            return sb.ToString().TrimEnd();
        }

        AppendDhOverview(sb, lineup, players);
        return sb.ToString().TrimEnd();
    }

    private static void AppendDhOverview(
        StringBuilder sb,
        LineupAnalyzeResponse lineup,
        IReadOnlyList<string> players)
    {
        var recDh = lineup.RecommendedLineup.FirstOrDefault(s =>
            string.Equals(s.Position, "DH", StringComparison.OrdinalIgnoreCase));

        sb.AppendLine(
            "DH is bat-only — the model parks whoever has the least defensive value to recover, " +
            "not your best bats or plus gloves.");

        foreach (var player in players)
        {
            var norm = LineupExplainHelpers.Norm(player);
            var cur = LineupExplainHelpers.FindSlot(lineup.CurrentLineup, norm, null);
            var rec = LineupExplainHelpers.FindSlot(lineup.RecommendedLineup, norm, null);
            if (cur == null) continue;

            sb.AppendLine();
            if (string.Equals(cur.Position, "DH", StringComparison.OrdinalIgnoreCase))
            {
                sb.AppendLine(
                    $"**{cur.Player}** — you have him at DH (#{cur.Order}, RC{cur.Rc600:F0} {LineupExplainHelpers.SideLabel(lineup.PitcherSide)}).");
                if (rec != null && !string.Equals(rec.Position, "DH", StringComparison.OrdinalIgnoreCase))
                {
                    sb.AppendLine(
                        $"The model moves him to **{rec.Position}** (#{rec.Order}, +{rec.Def:F0} def). " +
                        $"Every DH inning throws away ~{rec.Def:F0} runs of fielding the optimizer gets back in the OF.");
                }
            }
            else
            {
                sb.AppendLine(
                    $"**{cur.Player}** — you have him at **{cur.Position}** (#{cur.Order}, RC{cur.Rc600:F0}+def {cur.Def:F0}).");
                if (rec != null && rec.Def >= 2)
                {
                    sb.AppendLine(
                        $"Not at DH because he saves **+{rec.Def:F0} defensive runs** at {rec.Position}. " +
                        $"DH would bury that value — his bat is already in the lineup without hiding the glove.");
                }
                else if (rec != null && rec.Rc600 >= 110)
                {
                    sb.AppendLine(
                        $"He's a heart/cleanup bat — the model keeps him at **{rec.Position}** " +
                        $"#{rec.Order} (implementation-plan slots), not a DH parking spot.");
                }
                else
                {
                    sb.AppendLine("His bat is already deployed at a defensive position; DH is for the weakest glove left.");
                }
            }
        }

        if (recDh != null)
        {
            sb.AppendLine();
            sb.AppendLine(
                $"**Who fills DH instead:** {recDh.Player} (RC{recDh.Rc600:F0} {LineupExplainHelpers.SideLabel(lineup.PitcherSide)}, " +
                $"+{recDh.Def:F0} def — no premium fielding to recover elsewhere).");
            var starAtDh = players.Any(p =>
            {
                var c = LineupExplainHelpers.FindSlot(lineup.CurrentLineup, LineupExplainHelpers.Norm(p), null);
                return c != null && string.Equals(c.Position, "DH", StringComparison.OrdinalIgnoreCase);
            });
            if (!starAtDh && players.Count > 0)
            {
                sb.AppendLine(
                    $"Neither star needs DH to get their bat in — both are already starting. " +
                    $"{recDh.Player} is the cheapest bat-only fit.");
            }
        }
    }
}
