using GithubService;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Octokit;

namespace GitHubApis.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class GithubController : ControllerBase
    {
        private readonly IGitHubService _githubService;
        private readonly string _githubToken;
        public GithubController(IConfiguration configuration,IGitHubService githubService)
        {
            _githubService = githubService;
            _githubToken = configuration["GithubToken"];
        }

        [Route("repositories")]
        [HttpGet]
        public async Task<ActionResult<List<Repository>>> SearchRepositories( [FromQuery] string? repoName, [FromQuery] string? lang,[FromQuery] string? userName)
        {
            var repos = await _githubService.SearchRepositories(repoName, lang, userName);

            if (repos == null || repos.Count == 0)
            {
                return NotFound("No repositories found matching the criteria.");
            }

            return Ok(repos);
        }


        [Route("portfolio")]
        [HttpGet]
        public async Task<IActionResult> GetPortfolio()
        {
            var repos = await _githubService.GetPortfolio(_githubToken);
            if (repos == null)
            {
                return NotFound();
            }
            return Ok(repos);
        }
    }
}

