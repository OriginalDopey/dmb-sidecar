using System.Text.RegularExpressions;
using DmbSidecar.Api.Models;

namespace DmbSidecar.Api.Services.LineupExplain;

/// <summary>
/// Shared parsing and lookup helpers for lineup explain classification and handlers.
/// Resolves player last names, positions, batting-order slots, and lineup slot lookups.
/// </summary>
internal static partial class LineupExplainHelpers
{
    /// <summary>Returns the surname portion of a "Last, First" display name.</summary>
    public static string Norm(string name) =>
        name.Split(',')[0].Trim();

    /// <summary>Formats pitcher handedness for display (e.g. "vs LHP").</summary>
    public static string SideLabel(string pitcherSide)
    {
        if (pitcherSide.StartsWith("vs ", StringComparison.OrdinalIgnoreCase))
            return pitcherSide;
        return pitcherSide.Equals("lhp", StringComparison.OrdinalIgnoreCase) ? "vs LHP"
            : pitcherSide.Equals("rhp", StringComparison.OrdinalIgnoreCase) ? "vs RHP"
            : $"vs {pitcherSide}";
    }

    /// <summary>Finds a slot by normalized player name, optionally constrained to a position.</summary>
    public static LineupSlotResult? FindSlot(
        IReadOnlyList<LineupSlotResult> slots,
        string normPlayer,
        string? position)
    {
        if (position != null)
        {
            var atPos = slots.FirstOrDefault(s =>
                string.Equals(s.Position, position, StringComparison.OrdinalIgnoreCase));
            if (atPos != null && Norm(atPos.Player) == normPlayer)
                return atPos;
        }
        return slots.FirstOrDefault(s => Norm(s.Player) == normPlayer);
    }

    /// <summary>Matches last names from lineup and roster context against the question text.</summary>
    public static List<string> ExtractPlayersMentioned(
        string question,
        PageContext context,
        LineupAnalyzeResponse lineup)
    {
        var q = question.ToLowerInvariant();
        var names = (context.Slots ?? [])
            .Select(s => s.PlayerName)
            .Concat(lineup.CurrentLineup.Select(s => s.Player))
            .Concat(lineup.RecommendedLineup.Select(s => s.Player))
            .Where(n => !string.IsNullOrWhiteSpace(n))
            .Distinct(StringComparer.OrdinalIgnoreCase);

        var found = new List<string>();
        foreach (var name in names)
        {
            var last = name!.Split(',')[0].Trim().ToLowerInvariant();
            if (last.Length >= 3 && q.Contains(last))
                found.Add(name);
        }
        return found;
    }

    /// <summary>Extracts a formal "Last, First" name when present in the question.</summary>
    public static string? ExtractPlayerFromFormalName(string question) =>
        FormalPlayerName().Match(question) is { Success: true } m ? m.Groups[1].Value.Trim() : null;

    /// <summary>Returns the first defensive position token found in the question.</summary>
    public static string? TryExtractPosition(string question)
    {
        foreach (var pos in new[] { "DH", "1B", "2B", "3B", "SS", "LF", "CF", "RF" })
        {
            if (Regex.IsMatch(question, $@"\b{Regex.Escape(pos)}\b", RegexOptions.IgnoreCase))
                return pos;
        }
        if (Regex.IsMatch(question, @"\bC\b", RegexOptions.IgnoreCase))
            return "C";
        return null;
    }

    /// <summary>Infers batting-order slot 1–9 from hash notation or role keywords (leadoff, cleanup).</summary>
    public static int? TryExtractBattingOrder(string q)
    {
        var lower = q.ToLowerInvariant();
        if (OrderHash().Match(q) is { Success: true } m && int.TryParse(m.Groups[1].Value, out var n) && n is >= 1 and <= 9)
            return n;
        if (lower.Contains("leadoff") || lower.Contains("lead off") || lower.Contains("leading off"))
            return 1;
        if (lower.Contains("cleanup") || lower.Contains("clean up"))
            return 4;
        if (lower.Contains("second spot") || lower.Contains("#2 spot"))
            return 2;
        return null;
    }

    /// <summary>True when the question references DH or designated hitter.</summary>
    public static bool IsDhQuestion(string question) =>
        Regex.IsMatch(question, @"\bDH\b", RegexOptions.IgnoreCase)
        || question.Contains("designated hitter", StringComparison.OrdinalIgnoreCase);

    [GeneratedRegex(@"#\s*([1-9])\b")]
    private static partial Regex OrderHash();

    [GeneratedRegex(@"\b([A-Z][a-z]+,\s*[A-Z][a-z]+)\b", RegexOptions.IgnoreCase)]
    private static partial Regex FormalPlayerName();
}
