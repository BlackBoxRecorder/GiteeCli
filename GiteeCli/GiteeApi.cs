using Flurl;
using Flurl.Http;
using Newtonsoft.Json.Linq;

namespace GiteeCli
{
    internal static class GiteeApi
    {
        private const string userRepoUrl = $"https://gitee.com/api/v5/user/repos";

        public static async Task<List<string>> GetRepoUrls(string token)
        {
            var param = new
            {
                access_token = token,
                visibility = "all",
                sort = "full_name",
                page = 1,
                per_page = 100,
            };

            var resp = await userRepoUrl.SetQueryParams(param).GetAsync();

            if (resp.StatusCode != 200)
            {
                return new List<string>();
            }

            var json = await resp.ResponseMessage.Content.ReadAsStringAsync();

            var items = JArray.Parse(json);

            var result = new List<string>();

            foreach (var item in items)
            {
                var url = item["ssh_url"]?.ToString();

                if (url != null)
                {
                    result.Add(url);
                }
            }

            return result;
        }
    }
}
