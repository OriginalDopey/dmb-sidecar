using System.Diagnostics;
using System.Text;
using DmbSidecar.Api.Models;
using DmbSidecar.Api.Services.LineupExplain;

namespace DmbSidecar.Api.Services;

/// <summary>
/// Lineup Lab explain orchestrator for <c>POST /lineup/explain</c>.
/// Ensures analyze data exists, classifies the question via <see cref="LineupExplainRouter"/>,
/// prefers Foundry when configured, and routes to typed offline handlers on failure.
/// Returns <see cref="AdviseResponse"/> with <see cref="AdviseResponse.QuestionKind"/> set for UI routing.
/// </summary>
public sealed class LineupExplainService
{
    private readonly LineupAnalyzeService _lineup;
    private readonly LocalIqService _localIq;
    private readonly FoundryAgentService _foundry;
    private readonly LineupExplainRouter _router;
    private readonly ILogger<LineupExplainService> _log;

    /// <summary>Creates the service with lineup analysis, IQ, Foundry, and router dependencies.</summary>
    public LineupExplainService(
        LineupAnalyzeService lineup,
        LocalIqService localIq,
        FoundryAgentService foundry,
        LineupExplainRouter router,
        ILogger<LineupExplainService> log)
    {
        _lineup = lineup;
        _localIq = localIq;
        _foundry = foundry;
        _router = router;
        _log = log;
    }

    /// <summary>
    /// Answers a lineup-specific question using analysis context from the side panel.
    /// Re-runs analyze when <see cref="LineupExplainRequest.Lineup"/> is not supplied.
    /// </summary>
    public async Task<AdviseResponse> ExplainAsync(LineupExplainRequest request, CancellationToken ct = default)
    {
        var sw = Stopwatch.StartNew();
        string? warning = null;

        // --- Ensure lineup analysis ---

        var analysis = request.Lineup;
        if (analysis == null)
        {
            analysis = await _lineup.AnalyzeAsync(request.Context, ct);
            if (analysis == null)
            {
                return new AdviseResponse(
                    "Could not analyze lineup — check dev stack and player pool CSV.",
                    [],
                    sw.ElapsedMilliseconds,
                    "Run ./scripts/start-dev.sh");
            }
        }

        var iqQuery =
            $"designated hitter DH lineup construction defense fielding batting order {request.Question}";
        var iqSnippets = _localIq.Search(iqQuery, maxSnippets: 6);

        var intent = _router.Classify(request.Question, request.Context, analysis);

        // --- Foundry path ---

        if (_foundry.IsConfigured)
        {
            try
            {
                var answer = await _foundry.InvokeAsync(
                    BuildFoundryPrompt(request, analysis, iqSnippets, intent), ct);
                sw.Stop();
                return new AdviseResponse(answer, [], sw.ElapsedMilliseconds, null, intent.Kind.ToString());
            }
            catch (Exception ex)
            {
                _log.LogWarning(ex, "Foundry lineup explain failed — offline handler fallback");
                warning = "Foundry unavailable — local handler answer below.";
            }
        }

        // --- Offline handler path ---

        var explainContext = new LineupExplainContext(
            request.Question, intent, request.Context, analysis, iqSnippets);
        var offlineAnswer = _router.Answer(explainContext);
        sw.Stop();

        return new AdviseResponse(
            offlineAnswer, [], sw.ElapsedMilliseconds, warning, intent.Kind.ToString());
    }

    private static string BuildFoundryPrompt(
        LineupExplainRequest request,
        LineupAnalyzeResponse lineup,
        IReadOnlyList<string> iqSnippets,
        LineupQuestionIntent intent)
    {
        var sb = new StringBuilder();
        sb.AppendLine("You are DMB Sidecar Lineup Lab. Answer ONLY the user's lineup question.");
        sb.AppendLine("Use the comparison data below. Be concise (3-6 sentences). No roster dump.");
        sb.AppendLine("Ground DH/defense/batting-order logic in KB snippets when relevant.");
        sb.AppendLine();
        sb.AppendLine($"## Question kind\n{intent.Kind}");
        if (intent.Players.Count > 0)
            sb.AppendLine($"## Players\n{string.Join(", ", intent.Players)}");
        if (intent.Position != null)
            sb.AppendLine($"## Position\n{intent.Position}");
        if (intent.BattingOrder != null)
            sb.AppendLine($"## Batting order slot\n#{intent.BattingOrder}");
        sb.AppendLine();
        sb.AppendLine($"## Question\n{request.Question}");
        sb.AppendLine($"## Handedness\n{lineup.PitcherSide}");
        sb.AppendLine($"## Totals\nCurrent {lineup.CurrentTotal:F1} · Recommended {lineup.RecommendedTotal:F1} · Δ{lineup.Delta:+0.0;-0.0} RC+def");
        sb.AppendLine("## Current (order, pos, player, RC, def)");
        foreach (var s in lineup.CurrentLineup.OrderBy(x => x.Order))
            sb.AppendLine($"{s.Order}. {s.Position} {s.Player} RC{s.Rc600:F0} +{s.Def:F0}def");
        sb.AppendLine("## Recommended");
        foreach (var s in lineup.RecommendedLineup.OrderBy(x => x.Order))
            sb.AppendLine($"{s.Order}. {s.Position} {s.Player} RC{s.Rc600:F0} +{s.Def:F0}def");
        if (iqSnippets.Count > 0)
        {
            sb.AppendLine("## KB snippets");
            foreach (var snip in iqSnippets.Take(3))
                sb.AppendLine(snip);
        }
        return sb.ToString();
    }
}
