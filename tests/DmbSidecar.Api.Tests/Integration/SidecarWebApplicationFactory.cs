using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.Configuration;

namespace DmbSidecar.Api.Tests.Integration;

/// <summary>Test host for ASP.NET Core integration tests with deterministic API key.</summary>
public sealed class SidecarWebApplicationFactory : WebApplicationFactory<Program>
{
    public const string TestApiKey = "test-api-key-ci";

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
