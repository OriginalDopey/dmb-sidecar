using DmbSidecar.Api.Services;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Logging.Abstractions;

namespace DmbSidecar.Api.Tests.Services;

/// <summary>
/// Unit tests for <see cref="LocalIqService"/> keyword search over bundled IQ markdown.
/// Uses a temporary content root with a synthetic rules file.
/// </summary>
public sealed class LocalIqServiceTests : IDisposable
{
    private readonly string _tempRoot;
    private readonly LocalIqService _service;

    /// <summary>
    /// Creates a temp IQ source tree and a service instance pointed at it.
    /// </summary>
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

    /// <summary>
    /// Verifies keyword search returns a matching paragraph from local markdown.
    /// </summary>
    [Fact]
    public void Search_finds_matching_paragraph()
    {
        var hits = _service.Search("designated hitter defense lineup");
        hits.Should().NotBeEmpty();
        hits[0].Should().Contain("bat-only");
    }

    /// <summary>
    /// Verifies blank or whitespace questions yield no search hits.
    /// </summary>
    [Fact]
    public void Search_returns_empty_for_blank_question() =>
        _service.Search("   ").Should().BeEmpty();

    /// <summary>
    /// Removes the temporary IQ source directory after each test class instance.
    /// </summary>
    public void Dispose()
    {
        if (Directory.Exists(_tempRoot))
            Directory.Delete(_tempRoot, recursive: true);
    }

    /// <summary>
    /// Minimal <see cref="IWebHostEnvironment"/> stub for temp content-root tests.
    /// </summary>
    private sealed class TestWebHostEnvironment : IWebHostEnvironment
    {
        /// <summary>
        /// Initializes content root under the temp directory.
        /// </summary>
        public TestWebHostEnvironment(string contentRoot)
        {
            ContentRootPath = Path.Combine(contentRoot, "src", "DmbSidecar.Api");
            Directory.CreateDirectory(ContentRootPath);
        }

        /// <inheritdoc />
        public string ApplicationName { get; set; } = "DmbSidecar.Api.Tests";

        /// <inheritdoc />
        public IFileProvider ContentRootFileProvider { get; set; } = null!;

        /// <inheritdoc />
        public string ContentRootPath { get; set; }

        /// <inheritdoc />
        public string EnvironmentName { get; set; } = "Test";

        /// <inheritdoc />
        public string WebRootPath { get; set; } = "";

        /// <inheritdoc />
        public IFileProvider WebRootFileProvider { get; set; } = null!;
    }
}
