using DmbSidecar.Api.Services;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Logging.Abstractions;

namespace DmbSidecar.Api.Tests.Services;

public sealed class LocalIqServiceTests : IDisposable
{
    private readonly string _tempRoot;
    private readonly LocalIqService _service;

    public LocalIqServiceTests()
    {
        _tempRoot = Path.Combine(Path.GetTempPath(), "dmb-sidecar-iq-" + Guid.NewGuid().ToString("N"));
        var iqDir = Path.Combine(_tempRoot, "iq-sources", "rules");
        Directory.CreateDirectory(iqDir);
        File.WriteAllText(
            Path.Combine(iqDir, "dh.md"),
            "# DH rules\n\nDesignated hitter is bat-only. Defense recovery matters for Classic Standard lineups.");

        var env = new TestWebHostEnvironment(_tempRoot);
        _service = new LocalIqService(env, NullLogger<LocalIqService>.Instance);
    }

    [Fact]
    public void Search_finds_matching_paragraph()
    {
        var hits = _service.Search("designated hitter defense lineup");
        hits.Should().NotBeEmpty();
        hits[0].Should().Contain("bat-only");
    }

    [Fact]
    public void Search_returns_empty_for_blank_question() =>
        _service.Search("   ").Should().BeEmpty();

    public void Dispose()
    {
        if (Directory.Exists(_tempRoot))
            Directory.Delete(_tempRoot, recursive: true);
    }

    private sealed class TestWebHostEnvironment : IWebHostEnvironment
    {
        public TestWebHostEnvironment(string contentRoot)
        {
            ContentRootPath = Path.Combine(contentRoot, "src", "DmbSidecar.Api");
            Directory.CreateDirectory(ContentRootPath);
        }

        public string ApplicationName { get; set; } = "DmbSidecar.Api.Tests";
        public IFileProvider ContentRootFileProvider { get; set; } = null!;
        public string ContentRootPath { get; set; }
        public string EnvironmentName { get; set; } = "Test";
        public string WebRootPath { get; set; } = "";
        public IFileProvider WebRootFileProvider { get; set; } = null!;
    }
}
