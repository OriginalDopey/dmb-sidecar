using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.Configuration;

namespace DmbSidecar.Api.Tests.Integration;

/// <summary>
/// Test host factory for ASP.NET Core integration tests.
/// Configures a deterministic API key, disables Foundry, and points the MCP
/// bridge at an unreachable URL so tests control external dependencies.
/// </summary>
public sealed class SidecarWebApplicationFactory : WebApplicationFactory<Program>
{
    /// <summary>
    /// API key injected into test requests via the <c>X-Api-Key</c> header.
    /// </summary>
    public const string TestApiKey = "test-api-key-ci";

    /// <summary>
    /// Overrides application configuration for isolated, deterministic tests.
    /// </summary>
    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.UseEnvironment("Development");
        builder.ConfigureAppConfiguration((_, config) =>
        {
            config.AddInMemoryCollection(new Dictionary<string, string?>
            {
                ["Security:ApiKey"] = TestApiKey,
                ["McpBridge:BaseUrl"] = "http://127.0.0.1:1",
                ["Foundry:ProjectEndpoint"] = "",
            });
        });
    }
}
