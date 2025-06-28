using Microsoft.AspNetCore.Mvc;

namespace BlazorApp1.Controllers
{
    [ApiController]
    [Route("api/[controller].json")]
    public class AgentController : ControllerBase
    {
        [HttpGet]
        public IActionResult GetAgentInfo()
        {
            var request = HttpContext.Request;
            var baseUrl = $"{request.Scheme}://{request.Host}";
            var sseStreamUrl = $"{baseUrl}/api/sse/stream";

            var agentInfo = new
            {
                name = "Azure Easy Auth Sample Agent",
                description = "A Blazor Server application demonstrating Azure App Service Easy Auth integration",
                version = "1.0.0",
                author = "Travis Vu",
                homepage = "https://github.com/travisvu/easyauthsample",
                license = "MIT",
                avatar = "/logo.png",
                url = sseStreamUrl,
                capabilities = new
                {
                    streaming = true,
                    pushNotifications = false,
                    stateTransitionHistory = true
                },
                defaultInputModes = new[] { "text" },
                defaultOutputModes = new[] { "text", "file" },
                actions = new[]
                {
                    new
                    {
                        name = "View Authentication Status",
                        description = "Check current authentication status and user information",
                        url = "/"
                    },
                    new
                    {
                        name = "API Examples",
                        description = "See examples of using auth tokens with APIs",
                        url = "/api-example"
                    },
                    new
                    {
                        name = "Test Fetch",
                        description = "Test fetching external URLs",
                        url = "/test-fetch.html"
                    }
                },
                tags = new[]
                {
                    "azure",
                    "authentication",
                    "blazor",
                    "easy-auth",
                    "app-service"
                }
            };

            return Ok(agentInfo);
        }
    }
}