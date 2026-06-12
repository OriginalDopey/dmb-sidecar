using DmbSidecar.Api.Models;
using DmbSidecar.Api.Services.LineupExplain.Handlers;

namespace DmbSidecar.Api.Services.LineupExplain;

/// <summary>
/// Routes lineup explain questions to typed handlers after entity extraction.
/// Single entry point for offline explain; Foundry uses <see cref="Classify"/> for prompt context.
/// Handlers are registered by <see cref="LineupQuestionKind"/> in the constructor.
/// </summary>
public sealed class LineupExplainRouter
{
    private readonly IReadOnlyDictionary<LineupQuestionKind, ILineupExplainHandler> _handlers;

    /// <summary>Registers all lineup explain handlers keyed by question kind.</summary>
    public LineupExplainRouter()
    {
        ILineupExplainHandler[] all =
        [
            new RecommendationSummaryHandler(),
            new DhAssignmentHandler(),
            new BattingOrderHandler(),
            new PositionComparisonHandler(),
            new PositionAssignmentHandler(),
            new FallbackHandler(),
        ];
        _handlers = all.ToDictionary(h => h.Kind);
    }

    /// <summary>Classifies a free-text question and extracts players, position, and batting-order slot.</summary>
    public LineupQuestionIntent Classify(
        string question,
        PageContext context,
        LineupAnalyzeResponse lineup) =>
        LineupQuestionClassifier.Classify(question, context, lineup);

    /// <summary>Builds an offline answer for a pre-classified intent using the matching handler.</summary>
    public string Answer(LineupExplainContext context)
    {
        var handler = _handlers.TryGetValue(context.Intent.Kind, out var h)
            ? h
            : _handlers[LineupQuestionKind.Fallback];
        return handler.Build(context);
    }

    /// <summary>Classifies and answers in one call (convenience for offline explain path).</summary>
    public (LineupQuestionIntent Intent, string Answer) Route(
        string question,
        PageContext context,
        LineupAnalyzeResponse lineup,
        IReadOnlyList<string> iqSnippets)
    {
        var intent = Classify(question, context, lineup);
        var explainContext = new LineupExplainContext(question, intent, context, lineup, iqSnippets);
        return (intent, Answer(explainContext));
    }
}
