namespace DmbSidecar.Api.Utilities;

/// <summary>Shared string utilities for API services.</summary>
internal static class TextHelper
{
    /// <summary>Truncates <paramref name="text"/> to <paramref name="maxLength"/> characters, appending an ellipsis when shortened.</summary>
    public static string Truncate(string text, int maxLength) =>
        text.Length <= maxLength ? text : string.Concat(text.AsSpan(0, maxLength), "…");
}
