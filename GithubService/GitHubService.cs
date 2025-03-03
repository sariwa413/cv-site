using Octokit;
using System;
using System.Xml.Linq;

namespace GithubService
{
    public class GitHubService : IGitHubService
    {
        private readonly GitHubClient _client;
        public GitHubService()
        {
            _client=new GitHubClient(new ProductHeaderValue("cv-github-app"));
        }
        public async Task<List<object>> GetPortfolio(string token)
        {
            _client.Credentials = new Credentials(token);

            var repos = await _client.Repository.GetAllForCurrent();

            var portfolio = await Task.WhenAll(repos.Select(async repo => new
            {
                repo.Name,
                repo.HtmlUrl,
                repo.StargazersCount,
                LastCommit = (await _client.Repository.Commit.GetAll(repo.Owner.Login, repo.Name)).FirstOrDefault()?.Commit.Author.Date,
                Languages = await _client.Repository.GetAllLanguages(repo.Owner.Login, repo.Name),
                PullRequests = (await _client.PullRequest.GetAllForRepository(repo.Owner.Login, repo.Name)).Count
            }));

            return portfolio.ToList<object>();
        }
        public async Task<List<Repository>> SearchRepositories(string? repoName, string? lang, string? userName)
        {
            var queryParts = new List<string>();
            if (!string.IsNullOrWhiteSpace(repoName))
                queryParts.Add(repoName);
            if (!string.IsNullOrWhiteSpace(userName))
                queryParts.Add($"user:{userName}");
            if (!string.IsNullOrWhiteSpace(lang))
                queryParts.Add($"language:{lang}");
            string query = string.Join(" ", queryParts);
            if (string.IsNullOrWhiteSpace(query))
                throw new ArgumentException("At least one search parameter must be provided.");

            var req = new SearchRepositoriesRequest(query);
            var res = await _client.Search.SearchRepo(req);
            return res.Items.Take(5).ToList();

        }



    }
}
