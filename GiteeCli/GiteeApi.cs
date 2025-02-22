using Flurl;
using Flurl.Http;
using GiteeCli.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace GiteeCli
{
    public class GiteeApi
    {
        private const string HOST = $"https://gitee.com/api/v5";
        private readonly string _token;

        public GiteeApi(string token)
        {
            _token = token;
        }

        public async Task<ApiResult<List<Repo>>> GetRepoUrls()
        {
            var param = new
            {
                access_token = _token,
                visibility = "all",
                sort = "full_name",
                page = 1,
                per_page = 100,
            };

            var resp = await HOST.AppendPathSegments("user", "repos")
                .SetQueryParams(param)
                .GetAsync();

            if (resp.StatusCode != 200)
            {
                return new ApiResult<List<Repo>>(resp.StatusCode, "请求失败") { Data = [] };
            }

            var json = await resp.ResponseMessage.Content.ReadAsStringAsync();
            var repos = JsonToRepos(json);
            return new ApiResult<List<Repo>>(0, "请求成功") { Data = repos };
        }

        public async Task<ApiResult<string>> DeleteRepo(string url)
        {
            var param = new { access_token = _token };

            try
            {
                var (own, repo) = Utils.GetOwnerRepoByUrl(url);

                var resp = await HOST.AppendPathSegments("repos", own, repo)
                    .SetQueryParams(param)
                    .DeleteAsync();

                if (resp.StatusCode == 204)
                {
                    return new ApiResult<string>(0, "") { Data = "" };
                }

                var data = await resp.ResponseMessage.Content.ReadAsStringAsync();
                return new ApiResult<string>(resp.StatusCode, "请求失败") { Data = data };
            }
            catch (Exception ex)
            {
                return new ApiResult<string>(-1, ex.Message) { Data = ex.ToString() };
            }
        }

        public async Task<ApiResult<string>> ClearRepo(string url)
        {
            var param = new { access_token = _token };

            try
            {
                var (own, repo) = Utils.GetOwnerRepoByUrl(url);

                var resp = await HOST.AppendPathSegments("repos", own, repo, "clear")
                    .SetQueryParams(param)
                    .PutAsync();

                if (resp.StatusCode == 204)
                {
                    return new ApiResult<string>(0, "") { Data = "" };
                }

                var data = await resp.ResponseMessage.Content.ReadAsStringAsync();
                return new ApiResult<string>(resp.StatusCode, "请求失败") { Data = data };
            }
            catch (Exception ex)
            {
                return new ApiResult<string>(-1, ex.Message) { Data = ex.ToString() };
            }
        }

        public async Task<ApiResult<List<Repo>>> GetStarRepo()
        {
            try
            {
                var param = new
                {
                    access_token = _token,
                    page = 1,
                    per_page = 100,
                };
                var resp = await HOST.AppendPathSegments("user", "starred")
                    .SetQueryParams(param)
                    .GetAsync();

                if (resp.StatusCode != 200)
                {
                    return new ApiResult<List<Repo>>(resp.StatusCode, "请求失败") { Data = [] };
                }

                var json = await resp.GetStringAsync();
                var repos = JsonToRepos(json);

                return new ApiResult<List<Repo>>(0, "请求成功") { Data = repos };
            }
            catch (Exception ex)
            {
                return new ApiResult<List<Repo>>(-1, ex.Message) { Data = [] };
            }
        }

        public async Task<ApiResult<string>> CancelStarRepo(string owner, string repo)
        {
            try
            {
                var param = new { access_token = _token };
                var resp = await HOST.AppendPathSegments("starred", owner, repo)
                    .SetQueryParams(param)
                    .DeleteAsync();

                if (resp.StatusCode != 204)
                {
                    return new ApiResult<string>(resp.StatusCode, "请求失败") { Data = "" };
                }

                return new ApiResult<string>(0, "请求成功") { Data = "" };
            }
            catch (Exception ex)
            {
                return new ApiResult<string>(-1, ex.Message) { Data = "" };
            }
        }

        public async Task<ApiResult<string>> GetGists()
        {
            try
            {
                var param = new { access_token = _token };
                var resp = await HOST.AppendPathSegments("gists").SetQueryParams(param).GetAsync();

                if (resp.StatusCode != 200)
                {
                    return new ApiResult<string>(resp.StatusCode, "请求失败") { Data = "" };
                }

                return new ApiResult<string>(0, "请求成功") { Data = "" };
            }
            catch (Exception ex)
            {
                return new ApiResult<string>(-1, ex.Message) { Data = "" };
            }
        }

        private static List<Repo> JsonToRepos(string json)
        {
            var items = JArray.Parse(json);

            var repos = new List<Repo>();

            foreach (var item in items)
            {
                var full_name = item["full_name"]?.ToString();
                var description = item["description"]?.ToString();
                var html_url = item["html_url"]?.ToString();
                var ssh_url = item["ssh_url"]?.ToString();
                var isPublic = item["public"]?.ToString();
                var language = item["language"]?.ToString();
                var stargazers_count = item["stargazers_count"]?.ToString();

#pragma warning disable CS8601 // 引用类型赋值可能为 null。
                var repo = new Repo()
                {
                    FullName = full_name,
                    Description = description,
                    HttpUrl = html_url,
                    SshUrl = ssh_url,
                    IsPublic = isPublic == "true",
                    Language = language,
                    StargazersCount = int.Parse(stargazers_count ?? "0"),
                };
#pragma warning restore CS8601 // 引用类型赋值可能为 null。


                repos.Add(repo);
            }

            return repos;
        }
    }
}
