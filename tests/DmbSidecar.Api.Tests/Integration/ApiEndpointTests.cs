using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using DmbSidecar.Api.Models;
using DmbSidecar.Api.Tests.Fixtures;

namespace DmbSidecar.Api.Tests.Integration;

public sealed class ApiEndpointTests : IClassFixture<SidecarWebApplicationFactory>
{
    private readonly HttpClient _client;

    public ApiEndpointTests(SidecarWebApplicationFactory factory) =>
        _client = factory.CreateClient();

    [Fact]
    public async Task Health_allows_anonymous_access()
    {
        var response = await _client.GetAsync("/health");
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var body = await response.Content.ReadFromJsonAsync<HealthResponse>();
        body!.Status.Should().Be("ok");
    }

    [Fact]
    public async Task Advise_requires_api_key()
    {
        var response = await _client.PostAsJsonAsync(
            "/advise",
            new AdviseRequest("What is RC/600?", LineupTestFixtures.DemoContext));
        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

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
