using System.Text.RegularExpressions;

namespace DmbSidecar.Api.Services;

/// <summary>
/// Offline keyword search over <c>iq-sources/</c> markdown when Foundry is unavailable.
/// Scores paragraphs by term overlap with the user question; used by advise and lineup explain fallbacks.
/// Repository root is resolved relative to the API content root (two levels up from the project folder).
/// </summary>
public sealed partial class LocalIqService
{
    private readonly string _iqRoot;
    private readonly ILogger<LocalIqService> _log;

    /// <summary>Locates the iq-sources directory at repo root.</summary>
    public LocalIqService(IWebHostEnvironment env, ILogger<LocalIqService> log)
    {
        _log = log;
        var repoRoot = Path.GetFullPath(Path.Combine(env.ContentRootPath, "..", ".."));
        _iqRoot = Path.Combine(repoRoot, "iq-sources");
    }

    /// <summary>True when the iq-sources folder exists on disk.</summary>
    public bool IsAvailable => Directory.Exists(_iqRoot);

    /// <summary>
    /// Returns up to <paramref name="maxSnippets"/> best-matching paragraphs from IQ markdown files.
    /// Empty when iq-sources is missing or no terms could be extracted from the question.
    /// </summary>
    public IReadOnlyList<string> Search(string question, int maxSnippets = 3)
    {
        if (!IsAvailable)
            return [];

        var terms = ExtractTerms(question);
        if (terms.Count == 0)
            return [];

        var hits = new List<(int Score, string Snippet)>();

        foreach (var file in Directory.EnumerateFiles(_iqRoot, "*.md", SearchOption.AllDirectories))
        {
            var text = File.ReadAllText(file);
            var rel = Path.GetRelativePath(_iqRoot, file);
            foreach (var para in SplitParagraphs(text))
            {
                if (para.Length < 40)
                    continue;
                var score = terms.Count(t => para.Contains(t, StringComparison.OrdinalIgnoreCase));
                if (score > 0)
                    hits.Add((score, $"[{rel}] {para.Trim()}"));
            }
        }

        return hits
            .OrderByDescending(h => h.Score)
            .ThenBy(h => h.Snippet.Length)
            .Take(maxSnippets)
            .Select(h => h.Snippet)
            .ToList();
    }

    // --- Term extraction ---

    private static List<string> ExtractTerms(string question)
    {
        var stop = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
        {
            "a", "an", "the", "is", "are", "was", "were", "what", "who", "how", "when", "where",
            "my", "me", "i", "do", "does", "did", "can", "should", "would", "could", "tell",
            "about", "this", "that", "screen", "explain",
        };

        return WordRegex()
            .Matches(question)
            .Select(m => m.Value.ToLowerInvariant())
            .Where(w => w.Length > 2 && !stop.Contains(w))
            .Distinct()
            .Take(8)
            .ToList();
    }

    private static IEnumerable<string> SplitParagraphs(string text) =>
        text.Split(["\n\n", "\r\n\r\n"], StringSplitOptions.RemoveEmptyEntries);

    [GeneratedRegex(@"\b[a-zA-Z]{3,}\b")]
    private static partial Regex WordRegex();
}
