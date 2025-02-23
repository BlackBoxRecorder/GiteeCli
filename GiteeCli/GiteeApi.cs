using Flurl;
using Flurl.Http;
using GiteeCli.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Spectre.Console;

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

        public async Task<ApiResult<List<Repo>>> GetRepos()
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

        public async Task<ApiResult<string>> DeleteRepo(string name)
        {
            var param = new { access_token = _token };

            try
            {
                var own = Utils.GetUserName();
                var repo = name;

                var resp = await HOST.AppendPathSegments("repos", own, repo)
                    .SetQueryParams(param)
                    .DeleteAsync();

                if (resp.StatusCode == 204)
                {
                    return new ApiResult<string>(0, "删除仓库") { Data = "" };
                }

                var data = await resp.ResponseMessage.Content.ReadAsStringAsync();
                return new ApiResult<string>(resp.StatusCode, "请求失败") { Data = data };
            }
            catch (Exception ex)
            {
                return new ApiResult<string>(-1, ex.Message) { Data = ex.ToString() };
            }
        }

        public async Task<ApiResult<string>> ClearRepo(string name)
        {
            var param = new { access_token = _token };

            try
            {
                var own = Utils.GetUserName();
                var repo = name;

                var resp = await HOST.AppendPathSegments("repos", own, repo, "clear")
                    .SetQueryParams(param)
                    .PutAsync();

                if (resp.StatusCode == 204)
                {
                    return new ApiResult<string>(0, "清空仓库") { Data = "" };
                }

                var data = await resp.ResponseMessage.Content.ReadAsStringAsync();
                return new ApiResult<string>(resp.StatusCode, "请求失败") { Data = data };
            }
            catch (Exception ex)
            {
                return new ApiResult<string>(-1, ex.Message) { Data = ex.ToString() };
            }
        }

        public async Task<ApiResult<List<Repo>>> GetStarRepos()
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

                return new ApiResult<List<Repo>>(0, "请求用户星标仓库成功") { Data = repos };
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

                return new ApiResult<string>(0, "取消星标仓库成功") { Data = "" };
            }
            catch (Exception ex)
            {
                return new ApiResult<string>(-1, ex.Message) { Data = "" };
            }
        }

        public async Task<ApiResult<List<Gists>>> GetGists()
        {
            try
            {
                var param = new { access_token = _token };
                var resp = await HOST.AppendPathSegments("gists").SetQueryParams(param).GetAsync();

                if (resp.StatusCode != 200)
                {
                    return new ApiResult<List<Gists>>(resp.StatusCode, "请求失败") { Data = [] };
                }

                var json = await resp.ResponseMessage.Content.ReadAsStringAsync();

                var gists = JsonToGists(json);

                return new ApiResult<List<Gists>>(0, "请求成功") { Data = gists };
            }
            catch (Exception ex)
            {
                return new ApiResult<List<Gists>>(-1, ex.Message) { Data = [] };
            }
        }

        public async Task<ApiResult<string>> DeleteGist(string id)
        {
            try
            {
                var param = new { access_token = _token };
                var resp = await HOST.AppendPathSegments("gists", id)
                    .SetQueryParams(param)
                    .DeleteAsync();

                if (resp.StatusCode != 204)
                {
                    return new ApiResult<string>(resp.StatusCode, "请求失败") { Data = "" };
                }
                return new ApiResult<string>(0, "删除成功") { Data = "" };
            }
            catch (Exception ex)
            {
                return new ApiResult<string>(-1, ex.Message) { Data = "" };
            }
        }

        public async Task<ApiResult<string>> CreateGist(string title, string file)
        {
            try
            {
                if (file.StartsWith(".\\"))
                { //当前目录
                    file = file.Replace(".\\", "");
                }

                var fullpath = Path.Combine(Environment.CurrentDirectory, file);

                if (!File.Exists(fullpath))
                {
                    throw new FileNotFoundException(fullpath);
                }

                var content = await File.ReadAllTextAsync(fullpath);

                var files = new Dictionary<string, Dictionary<string, string>>
                {
                    {
                        file,
                        new Dictionary<string, string> { { "content", content } }
                    },
                };

                var param = new
                {
                    access_token = _token,
                    description = title,
                    files = files,
                };

                Console.WriteLine(JsonConvert.SerializeObject(param));
                var resp = await HOST.AppendPathSegments("gists").PostJsonAsync(param);

                if (resp.StatusCode != 201)
                {
                    return new ApiResult<string>(resp.StatusCode, "请求失败") { Data = "" };
                }
                return new ApiResult<string>(0, "删除成功") { Data = "" };
            }
            catch (Exception ex)
            {
                AnsiConsole.WriteException(ex);
                return new ApiResult<string>(-1, ex.Message) { Data = ex.ToString() };
            }
        }

        public async Task<ApiResult<Gists>> GetGist(string id)
        {
            try
            {
                var param = new { access_token = _token };

                var resp = await HOST.AppendPathSegments("gists", id)
                    .SetQueryParams(param)
                    .GetAsync();

                if (resp.StatusCode != 200)
                {
                    return new ApiResult<Gists>(resp.StatusCode, "请求失败");
                }

                var json = await resp.GetStringAsync();

                var obj = JObject.Parse(json);

                var gists = new Gists
                {
                    Id =
                        obj["id"]?.ToString() ?? throw new InvalidCastException("gists id解析失败"),
                    Description =
                        obj["description"]?.ToString()
                        ?? throw new InvalidCastException("gists description解析失败"),
                };

                var filesJson =
                    obj["files"]?.ToString()
                    ?? throw new InvalidCastException("gists files json解析失败");

                var files =
                    JsonConvert.DeserializeObject<Dictionary<string, GistsFile>>(filesJson)
                    ?? throw new InvalidCastException("gists files json 反序列化失败");
                gists.Files = files;

                return new ApiResult<Gists>(0, "获取Gists成功") { Data = gists };
            }
            catch (Exception ex)
            {
                AnsiConsole.WriteException(ex);
                return new ApiResult<Gists>(-1, ex.Message);
            }
        }

        public async Task<ApiResult<string>> UpdateGist(string id, string title, string file)
        {
            try
            {
                if (file.StartsWith(".\\"))
                {
                    file = file.Replace(".\\", "");
                }

                var fullpath = Path.Combine(Environment.CurrentDirectory, file);

                if (!File.Exists(fullpath))
                {
                    throw new FileNotFoundException(fullpath);
                }

                var content = await File.ReadAllTextAsync(fullpath);

                var files = new Dictionary<string, Dictionary<string, string>>
                {
                    {
                        file,
                        new Dictionary<string, string> { { "content", content } }
                    },
                };

                var param = new
                {
                    access_token = _token,
                    description = title,
                    files = files,
                };

                var resp = await HOST.AppendPathSegments("gists", id).PatchJsonAsync(param);

                if (resp.StatusCode != 200)
                {
                    return new ApiResult<string>(resp.StatusCode, "请求失败");
                }
                return new ApiResult<string>(0, "更新成功");
            }
            catch (Exception ex)
            {
                AnsiConsole.WriteException(ex);
                return new ApiResult<string>(-1, ex.Message);
            }
        }

        public async Task<ApiResult<string>> GetUserInfo()
        {
            try
            {
                var param = new { access_token = _token };
                var resp = await HOST.AppendPathSegments("user").SetQueryParams(param).GetAsync();
                if (resp.StatusCode != 200)
                {
                    return new ApiResult<string>(resp.StatusCode, "请求失败") { Data = "" };
                }

                var json = await resp.ResponseMessage.Content.ReadAsStringAsync();
                var obj = JObject.Parse(json);
                var name = obj["login"] ?? throw new InvalidDataException("获取用户信息失败");

                return new ApiResult<string>(0, "获取用户成功") { Data = name.ToString() };
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
            int index = 1;
            foreach (var item in items)
            {
                var name = item["path"]?.ToString();
                var description = item["description"]?.ToString();
                var html_url = item["html_url"]?.ToString();
                var ssh_url = item["ssh_url"]?.ToString();
                var isPublic = item["public"]?.ToString();
                var language = item["language"]?.ToString();
                var stargazers_count = item["stargazers_count"]?.ToString();

#pragma warning disable CS8601 // 引用类型赋值可能为 null。
                var repo = new Repo()
                {
                    Index = index,
                    Name = name,
                    Description = description,
                    HttpUrl = html_url,
                    SshUrl = ssh_url,
                    IsPublic = isPublic?.ToLower() == "true",
                    Language = language,
                    StargazersCount = int.Parse(stargazers_count ?? "0"),
                };
#pragma warning restore CS8601 // 引用类型赋值可能为 null。
                index++;
                repos.Add(repo);
            }

            return repos;
        }

        public static List<Gists> JsonToGists(string json)
        {
            var items = JArray.Parse(json);
            var gists = new List<Gists>();

            int index = 1;
            foreach (var item in items)
            {
                var id = item["id"]?.ToString();
                var description = item["description"]?.ToString();
                var filesJson =
                    item["files"]?.ToString()
                    ?? throw new InvalidCastException("gists files json 序列化异常");

                var files = JsonConvert.DeserializeObject<Dictionary<string, GistsFile>>(filesJson);

#pragma warning disable CS8601 // 引用类型赋值可能为 null。
                var gist = new Gists()
                {
                    Index = index,
                    Id = id,
                    Description = description,
                    Files = files,
                };
#pragma warning restore CS8601 // 引用类型赋值可能为 null。
                gists.Add(gist);
                index++;
            }

            return gists;
        }
    }
}
