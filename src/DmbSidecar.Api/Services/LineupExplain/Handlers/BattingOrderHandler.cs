using System.Text;
using DmbSidecar.Api.Models;

namespace DmbSidecar.Api.Services.LineupExplain.Handlers;

/// <summary>
/// Explains batting-order slots using implementation-plan rules (OBP #1–2, RC heart #3–5).
/// </summary>
internal sealed class BattingOrderHandler : ILineupExplainHandler
{
    public LineupQuestionKind Kind => LineupQuestionKind.BattingOrder;

    public string Build(LineupExplainContext ctx)
    {
        var sb = new StringBuilder();
        var lineup = ctx.Lineup;
        var intent = ctx.Intent;

        var player = intent.Players.Count > 0
            ? intent.Players[0]
            : LineupExplainHelpers.ExtractPlayersMentioned(ctx.Question, ctx.PageContext, lineup).FirstOrDefault();

        if (player == null)
        {
            AppendOrderFramework(sb, lineup, null, intent.BattingOrder);
            return sb.ToString().TrimEnd();
        }

        var norm = LineupExplainHelpers.Norm(player);
        var cur = LineupExplainHelpers.FindSlot(lineup.CurrentLineup, norm, null);
        var rec = LineupExplainHelpers.FindSlot(lineup.RecommendedLineup, norm, null);
        var targetSlot = intent.BattingOrder ?? cur?.Order ?? rec?.Order;

        AppendOrderFramework(sb, lineup, player, targetSlot);

        if (cur != null && rec != null)
        {
            sb.AppendLine();
            sb.AppendLine(
                $"**{cur.Player}** — your #{cur.Order} · model #{rec.Order} " +
                $"(RC{rec.Rc600:F0} {LineupExplainHelpers.SideLabel(lineup.PitcherSide)}, OBP-driven slots vs RC heart).");
            if (cur.Order != rec.Order)
                sb.AppendLine($"Slot change: #{cur.Order} → #{rec.Order}.");
        }

        return sb.ToString().TrimEnd();
    }

    private static void AppendOrderFramework(
        StringBuilder sb,
        LineupAnalyzeResponse lineup,
        string? player,
        int? slot)
    {
        sb.AppendLine("**Batting order** — implementation-plan slots (same as generate_team_config):");
        sb.AppendLine("#1–2 highest OBP · #3–5 top RC600 heart · #4 cleanup · #9 second leadoff.");
        sb.AppendLine($"Totals {LineupExplainHelpers.SideLabel(lineup.PitcherSide)}: current {lineup.CurrentTotal:F1} · recommended {lineup.RecommendedTotal:F1} · Δ{lineup.Delta:+0.0;-0.0}.");
        if (player != null && slot != null)
            sb.AppendLine($"Focus: **{player}** in the #{slot} slot.");
        else if (player != null)
            sb.AppendLine($"Focus: **{player}** batting-order placement.");
    }
}
