using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using DmbSidecar.Api.Models;
using DmbSidecar.Api.Tests.Fixtures;

namespace DmbSidecar.Api.Tests.Integration;

/// <summary>
/// Integration tests for DmbSidecar.Api HTTP endpoints.
/// Exercises authentication, lineup explain/analyze routes, and the anonymous
/// health probe using <see cref="SidecarWebApplicationFactory"/>.
/// </summary>
public sealed class ApiEndpointTests : IClassFixture<SidecarWebApplicationFactory>
{
    private readonly HttpClient _client;

    /// <summary>
    /// Creates an HTTP client bound to the in-memory test host.
    /// </summary>
    public ApiEndpointTests(SidecarWebApplicationFactory factory) =>
        _client = factory.CreateClient();

    /// <summary>
    /// Verifies <c>GET /health</c> is reachable without an API key.
    /// </summary>
    [Fact]
    public async Task Health_allows_anonymous_access()
    {
        var response = await _client.GetAsync("/health");
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var body = await response.Content.ReadFromJsonAsync<HealthResponse>();
        body!.Status.Should().Be("ok");
    }

    /// <summary>
    /// Verifies <c>POST /advise</c> returns 401 when the API key header is missing.
    /// </summary>
    [Fact]
    public async Task Advise_requires_api_key()
    {
        var response = await _client.PostAsJsonAsync(
            "/advise",
            new AdviseRequest("What is RC/600?", LineupTestFixtures.DemoContext));
        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    /// <summary>
    /// Verifies <c>POST /lineup/explain</c> classifies a DH question and returns an answer.
    /// </summary>
    [Fact]
    public async Task Lineup_explain_returns_question_kind()
    {
        using var request = new HttpRequestMessage(HttpMethod.Post, "/lineup/explain");
        request.Headers.Add("X-Api-Key", SidecarWebApplicationFactory.TestApiKey);
        request.Content = JsonContent.Create(new LineupExplainRequest(
            "Why not Cobb at DH?",
            LineupTestFixtures.DemoContext,
            LineupTestFixtures.DemoAnalysis));

        var response = await _client.SendAsync(request);
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var json = await response.Content.ReadAsStringAsync();
        using var doc = JsonDocument.Parse(json);
        doc.RootElement.GetProperty("questionKind").GetString().Should().Be("DhAssignment");
        doc.RootElement.GetProperty("answer").GetString().Should().NotBeNullOrWhiteSpace();
    }

    /// <summary>
    /// Verifies <c>POST /lineup/analyze</c> returns 401 without an API key.
    /// </summary>
    [Fact]
    public async Task Lineup_analyze_requires_api_key()
    {
        var response = await _client.PostAsJsonAsync(
            "/lineup/analyze",
            LineupTestFixtures.DemoContext);
        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    /// <summary>
    /// Verifies <c>POST /lineup/analyze</c> returns lineup comparison when the MCP bridge is up.
    /// Accepts 502 when the bridge is unavailable (typical in CI).
    /// </summary>
    [Fact]
    public async Task Lineup_analyze_returns_comparison()
    {
        using var request = new HttpRequestMessage(HttpMethod.Post, "/lineup/analyze");
        request.Headers.Add("X-Api-Key", SidecarWebApplicationFactory.TestApiKey);
        request.Content = JsonContent.Create(LineupTestFixtures.DemoContext);

        var response = await _client.SendAsync(request);
        // Bridge may be down in CI — accept 502 or 200
        response.StatusCode.Should().BeOneOf(HttpStatusCode.OK, HttpStatusCode.BadGateway);
        if (response.StatusCode == HttpStatusCode.OK)
        {
            var body = await response.Content.ReadFromJsonAsync<LineupAnalyzeResponse>();
            body!.CurrentLineup.Should().HaveCount(9);
            body.RecommendedLineup.Should().NotBeEmpty();
        }
    }

    /// <summary>
    /// Verifies <c>POST /lineup/explain</c> rejects blank questions with 400.
    /// </summary>
    [Fact]
    public async Task Lineup_explain_rejects_empty_question()
    {
        using var request = new HttpRequestMessage(HttpMethod.Post, "/lineup/explain");
        request.Headers.Add("X-Api-Key", SidecarWebApplicationFactory.TestApiKey);
        request.Content = JsonContent.Create(new LineupExplainRequest(
            "  ",
            LineupTestFixtures.DemoContext,
            LineupTestFixtures.DemoAnalysis));

        var response = await _client.SendAsync(request);
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }
}
