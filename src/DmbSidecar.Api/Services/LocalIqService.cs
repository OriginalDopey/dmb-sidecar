using System.Text.RegularExpressions;

namespace DmbSidecar.Api.Services;

/// <summary>
/// Offline keyword search over iq-sources/ markdown when Foundry is unavailable.
/// Demo-grade substitute until portal KB is wired.
/// </summary>
public sealed partial class LocalIqService
{
    private readonly string _iqRoot;
    private readonly ILogger<LocalIqService> _log;

    public LocalIqService(IWebHostEnvironment env, ILogger<LocalIqService> log)
    {
        _log = log;
        var repoRoot = Path.GetFullPath(Path.Combine(env.ContentRootPath, "..", ".."));
        _iqRoot = Path.Combine(repoRoot, "iq-sources");
    }

    public bool IsAvailable => Directory.Exists(_iqRoot);

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
