using System.Globalization;
using System.Text;
using System.Text.RegularExpressions;
using DmbSidecar.Api.Models;

namespace DmbSidecar.Api.Services;

/// <summary>
/// Deterministic roster-screen analysis for offline advise when Foundry is down.
/// Parses scraped roster DOM into salary balance, position coverage, IR usage, and pre-season checks.
/// Triggered when the user question matches review keywords via <see cref="WantsReview"/>.
/// </summary>
internal static partial class OfflineRosterReview
{
    /// <summary>True when the question asks for a structured roster review or screen explanation.</summary>
    public static bool WantsReview(string question) =>
        question.Contains("review", StringComparison.OrdinalIgnoreCase) ||
        question.Contains("salary balance", StringComparison.OrdinalIgnoreCase) ||
        question.Contains("position coverage", StringComparison.OrdinalIgnoreCase) ||
        question.Contains("ir usage", StringComparison.OrdinalIgnoreCase) ||
        question.Contains("explain this screen", StringComparison.OrdinalIgnoreCase);

    /// <summary>
    /// Builds a markdown roster review from scraped page context, or null when not on a roster page.
    /// </summary>
    public static string? Build(PageContext context)
    {
        if (context.PageType != "roster" || context.Slots is not { Count: > 0 })
            return null;

        var hitters = Slots(context, "batter");
        var pitchers = Slots(context, "pitcher");
        var ir = Slots(context, "ir");
        if (hitters.Count + pitchers.Count == 0)
            return null;

        var hitterPay = Sum(hitters);
        var pitcherPay = Sum(pitchers);
        var irPay = Sum(ir);
        var total = hitterPay + pitcherPay + irPay;

        var teamName = context.Extra?.GetValueOrDefault("teamName");
        var cash = context.Extra?.GetValueOrDefault("cashBalance");
        var park = context.Extra?.GetValueOrDefault("stadium");
        var cap = context.Extra?.GetValueOrDefault("totalValue");

        var sb = new StringBuilder();
        sb.AppendLine($"**Roster review — {teamName ?? "your team"}** *(offline analysis; Foundry agent will synthesize this once wired)*");
        sb.AppendLine();

        // --- Salary balance ---

        sb.AppendLine("**Salary balance**");
        if (total > 0)
        {
            sb.AppendLine($"- Hitters: ${hitterPay:N0} ({Pct(hitterPay, total)}) · Pitchers: ${pitcherPay:N0} ({Pct(pitcherPay, total)}) · IR: ${irPay:N0} ({Pct(irPay, total)})");
        }
        if (!string.IsNullOrEmpty(cap) || !string.IsNullOrEmpty(cash))
            sb.AppendLine($"- Cap check: {cap ?? "?"} committed · {cash ?? "?"} cash remaining");
        var cashNum = ParseMoney(cash);
        if (cashNum is < 500_000 and >= 0)
            sb.AppendLine("- ⚠️ Almost no cash left — in-season moves need a loan or a release/sign pair with tight math (75% recovery on drops).");
        else if (cashNum >= 2_000_000)
            sb.AppendLine($"- ${cashNum:N0} cash gives you in-season flexibility (still budget ~125% of new salary if you drop someone in-season).");

        var stars = AllActive(context).OrderByDescending(SlotSalary).Take(6).ToList();
        if (stars.Count > 0)
        {
            sb.AppendLine($"- Top spend: {string.Join(", ", stars.Select(s => $"{s.PlayerName} ({FormatMoney(SlotSalary(s))})"))}");
        }

        var sp = pitchers.Where(p => HasPosition(p, "SP")).ToList();
        var spPay = Sum(sp);
        if (sp.Count >= 4 && total > 0 && spPay > total * 0.55m)
            sb.AppendLine($"- ⚠️ Rotation-heavy: {sp.Count} SP account for ${spPay:N0} ({Pct(spPay, total)}) — star-starter build; bullpen is mostly cheap innings.");
        else if (total > 0 && hitterPay > total * 0.55m)
            sb.AppendLine($"- ⚠️ Offense-heavy: position players are {Pct(hitterPay, total)} of payroll — stars are in the lineup; pitching depth must come cheap.");

        // --- Position coverage ---

        sb.AppendLine();
        sb.AppendLine("**Position coverage**");
        var catchers = CountPositions(hitters, "C");
        sb.AppendLine($"- Active: {hitters.Count} position players, {pitchers.Count} pitchers ({sp.Count} SP), {ir.Count}/3 IR slots used.");
        sb.AppendLine(catchers switch
        {
            0 => "- Catchers: none listed — add a backup before Opening Day.",
            1 => "- Catchers: 1 active — workable if you defensive-sub every game; a $500K backup catcher is cheap insurance.",
            _ => $"- Catchers: {catchers} active — good for fatigue rotation.",
        });
        sb.AppendLine($"- Middle IF: {CountPositions(hitters, "SS")} SS-eligible, {CountPositions(hitters, "2B")} 2B, {CountPositions(hitters, "3B")} 3B.");
        sb.AppendLine($"- OF depth: {CountPositions(hitters, "CF")} CF, {CountPositions(hitters, "LF")} LF, {CountPositions(hitters, "RF")} RF (multi-position flags count in each).");
        var minSalaryHitters = hitters.Count(p => SlotSalary(p) <= 500_000);
        if (minSalaryHitters >= 6)
            sb.AppendLine($"- {minSalaryHitters} hitters at or near $500K minimum — fine for platoon/IR stubs, but bench offense is thin.");

        // --- IR usage ---

        sb.AppendLine();
        sb.AppendLine("**IR usage**");
        if (ir.Count == 0)
            sb.AppendLine("- IR empty — stash a catcher or platoon bat before Opening Day if you want injury insulation.");
        else if (ir.All(p => SlotSalary(p) <= 500_000))
            sb.AppendLine("- All three IR slots are minimum-salary depth — correct Classic Standard pattern (cap relief without wasting active spots).");
        else
            sb.AppendLine("- IR includes non-minimum salaries — verify those players are worth inactive cap vs. an active upgrade.");

        if (!string.IsNullOrEmpty(park))
        {
            sb.AppendLine();
            sb.AppendLine("**Park note**");
            sb.AppendLine($"- Home: {park}. Ask Foundry IQ (once live) how this park fits your power/contact mix and whether roster construction matches the park.");
        }

        // --- Pre-season checklist ---

        sb.AppendLine();
        sb.AppendLine("**Quick checks before Opening Day**");
        sb.AppendLine("- Confirm 4 SP + 8+ pitchers in active 25; lineups vs LHP/RHP filled; bench PH/utility set per lineup.");
        if (cashNum < 500_000)
            sb.AppendLine("- With little cash left, treat every in-season add as ~125% of the incoming salary (25% release penalty) unless it's a straight swap.");
        else
            sb.AppendLine("- You have cash for a targeted upgrade — still model release recovery (75%) before pulling the trigger.");

        return sb.ToString().TrimEnd();
    }

    // --- Slot helpers ---

    private static List<PageSlot> Slots(PageContext ctx, string section) =>
        ctx.Slots!
            .Where(s => string.Equals(s.Section, section, StringComparison.OrdinalIgnoreCase))
            .ToList();

    private static IEnumerable<PageSlot> AllActive(PageContext ctx) =>
        Slots(ctx, "batter").Concat(Slots(ctx, "pitcher"));

    private static bool HasPosition(PageSlot player, string pos)
    {
        if (string.IsNullOrWhiteSpace(player.Position)) return false;
        return player.Position
            .Split(',', StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries)
            .Any(t => string.Equals(t, pos, StringComparison.OrdinalIgnoreCase));
    }

    private static int CountPositions(IEnumerable<PageSlot> players, string pos) =>
        players.Count(p => HasPosition(p, pos));

    private static decimal Sum(IEnumerable<PageSlot> players) =>
        players.Sum(SlotSalary);

    private static decimal SlotSalary(PageSlot slot) => ParseMoney(slot.Salary);

    private static decimal ParseMoney(string? raw)
    {
        if (string.IsNullOrWhiteSpace(raw)) return 0;
        var digits = MoneyDigits().Replace(raw, "");
        return decimal.TryParse(digits, NumberStyles.Integer, CultureInfo.InvariantCulture, out var n) ? n : 0;
    }

    private static string FormatMoney(decimal n) => n >= 1_000_000 ? $"${n / 1_000_000m:0.#}M" : $"${n:N0}";

    private static string Pct(decimal part, decimal whole) =>
        whole <= 0 ? "0%" : $"{part / whole * 100:0}%";

    [GeneratedRegex(@"[^\d]")]
    private static partial Regex MoneyDigits();
}
