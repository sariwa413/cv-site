using GithubService;
using Microsoft.Extensions.Caching.Memory;
using Octokit;

namespace GitHubApis.CachedServices
{
    public class CachedGithubService : IGitHubService
    {
        private readonly IGitHubService _gitHubService;
        private readonly IMemoryCache _memoryCache;

        private const string userPortfolioKey = "userPortfolioKey";
        public CachedGithubService(IGitHubService gitHubService,IMemoryCache memoryCache)
        {
            _gitHubService = gitHubService;
            _memoryCache = memoryCache;
        }
        public async Task<List<object>> GetPortfolio(string token)
        {
            if (_memoryCache.TryGetValue(userPortfolioKey, out var portfolio))
                return (List<object>)portfolio;
            var cacheOptions=new MemoryCacheEntryOptions()
                .SetAbsoluteExpiration(TimeSpan.FromSeconds(30))
                .SetSlidingExpiration(TimeSpan.FromSeconds(10));
            portfolio=await _gitHubService.GetPortfolio(token);
            _memoryCache.Set(userPortfolioKey, portfolio,cacheOptions);
            return (List<object>)portfolio;
        }

        public Task<List<Repository>> SearchRepositories(string? repoName, string? lang, string? userName)
        {
            return _gitHubService.SearchRepositories(repoName, lang, userName);
        }
    }
}
