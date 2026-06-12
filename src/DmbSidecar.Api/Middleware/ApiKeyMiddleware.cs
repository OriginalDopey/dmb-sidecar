using DmbSidecar.Api.Configuration;
using Microsoft.Extensions.Options;

namespace DmbSidecar.Api.Middleware;

/// <summary>
/// Shared-secret gate for all API routes except health and Swagger.
/// The Chrome extension sends <c>X-Api-Key</c>; value is bound from <see cref="ApiSecurityOptions"/>.
/// Health and Swagger remain anonymous so local dev and probes work without the key.
/// </summary>
public sealed class ApiKeyMiddleware
{
    private const string HeaderName = "X-Api-Key";
    private readonly RequestDelegate _next;
    private readonly string _apiKey;

    /// <summary>Creates middleware with the configured API key from options.</summary>
    public ApiKeyMiddleware(RequestDelegate next, IOptions<ApiSecurityOptions> options)
    {
        _next = next;
        _apiKey = options.Value.ApiKey;
    }

    /// <summary>
    /// Validates the API key header or returns 401 JSON.
    /// Bypasses authentication for <c>/health</c> and <c>/swagger</c> paths.
    /// </summary>
    public async Task InvokeAsync(HttpContext context)
    {
        var path = context.Request.Path.Value ?? "";
        if (path.StartsWith("/health", StringComparison.OrdinalIgnoreCase) ||
            path.StartsWith("/swagger", StringComparison.OrdinalIgnoreCase))
        {
            await _next(context);
            return;
        }

        if (!context.Request.Headers.TryGetValue(HeaderName, out var provided) ||
            provided != _apiKey)
        {
            context.Response.StatusCode = StatusCodes.Status401Unauthorized;
            await context.Response.WriteAsJsonAsync(new { error = "Invalid or missing X-Api-Key header." });
            return;
        }

        await _next(context);
    }
}
