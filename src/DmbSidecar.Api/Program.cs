using DmbSidecar.Api.Configuration;
using DmbSidecar.Api.Middleware;
using DmbSidecar.Api.Models;
using DmbSidecar.Api.Services;
using DmbSidecar.Api.Services.LineupExplain;

var builder = WebApplication.CreateBuilder(args);

builder.Services.Configure<FoundryOptions>(builder.Configuration.GetSection(FoundryOptions.SectionName));
builder.Services.Configure<McpBridgeOptions>(builder.Configuration.GetSection(McpBridgeOptions.SectionName));
builder.Services.Configure<ApiSecurityOptions>(builder.Configuration.GetSection(ApiSecurityOptions.SectionName));

builder.Services.AddHttpClient<FoundryAgentService>();
builder.Services.AddHttpClient<McpBridgeClient>();
builder.Services.AddSingleton<LocalIqService>();
builder.Services.AddSingleton<AdviseService>();
builder.Services.AddSingleton<LineupAnalyzeService>();
builder.Services.AddSingleton<LineupExplainRouter>();
builder.Services.AddSingleton<LineupExplainService>();

builder.Services.AddCors(options =>
{
    options.AddPolicy("Extension", policy =>
    {
        policy.SetIsOriginAllowed(origin =>
                origin.StartsWith("chrome-extension://", StringComparison.OrdinalIgnoreCase)
                || origin.StartsWith("http://127.0.0.1:", StringComparison.OrdinalIgnoreCase)
                || origin.StartsWith("http://localhost:", StringComparison.OrdinalIgnoreCase))
            .AllowAnyHeader()
            .AllowAnyMethod();
    });
});

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c => c.SwaggerDoc("v1", new() { Title = "DMB Sidecar API", Version = "0.1.0" }));

var app = builder.Build();

app.UseCors("Extension");
app.UseMiddleware<ApiKeyMiddleware>();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.MapGet("/health", async (FoundryAgentService foundry, McpBridgeClient mcp) =>
{
    var mcpOk = await mcp.IsHealthyAsync();
    return Results.Ok(new HealthResponse(
        Status: "ok",
        FoundryConfigured: foundry.IsConfigured,
        McpBridgeReachable: mcpOk,
        Version: "0.1.0"));
}).WithTags("Health").AllowAnonymous();

app.MapPost("/foundry/smoke", async (FoundryAgentService foundry, CancellationToken ct) =>
{
  try
  {
    var text = await foundry.InvokeAsync(
        "In one sentence: what is in-season player release salary recovery in Classic Standard?",
        ct);
    return Results.Ok(new { answer = text });
  }
  catch (Exception ex)
  {
    return Results.Problem(ex.Message, statusCode: 502);
  }
}).WithTags("Foundry");

app.MapPost("/advise", async (AdviseRequest request, AdviseService advise, CancellationToken ct) =>
{
    if (string.IsNullOrWhiteSpace(request.Question))
        return Results.BadRequest(new { error = "Question is required." });
    var response = await advise.AdviseAsync(request, ct);
    return Results.Ok(response);
}).WithTags("Advise");

app.MapPost("/lineup/analyze", async (PageContext context, LineupAnalyzeService lineup, CancellationToken ct) =>
{
    if (context.PageType != "lineup" || context.Slots is not { Count: > 0 })
        return Results.BadRequest(new { error = "Open Edit Lineup with 9 slots filled." });
    var result = await lineup.AnalyzeAsync(context, ct);
    if (result == null)
        return Results.Problem("Lineup analysis failed — check player pool CSV.", statusCode: 502);
    return Results.Ok(result);
}).WithTags("Lineup");

app.MapPost("/lineup/explain", async (LineupExplainRequest request, LineupExplainService explain, CancellationToken ct) =>
{
    if (string.IsNullOrWhiteSpace(request.Question))
        return Results.BadRequest(new { error = "Question is required." });
    if (request.Context.PageType != "lineup")
        return Results.BadRequest(new { error = "Open Edit Lineup first." });
    var response = await explain.ExplainAsync(request, ct);
    return Results.Ok(response);
}).WithTags("Lineup");

app.Run();

/// <summary>Entry point type for ASP.NET Core integration tests.</summary>
public partial class Program { }
