using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GiteeCli.Models;
using Newtonsoft.Json;

namespace GiteeCli
{
    public static class Utils
    {
        private static readonly string giteeDir = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
            "GiteeCli"
        );
        private static readonly string tokenFile = Path.Combine(giteeDir, "token.txt");
        private static readonly string reposFile = Path.Combine(giteeDir, "repos.json");
        private static readonly string userFile = Path.Combine(giteeDir, "user.txt");

        /// <summary>
        ///
        /// https://gitee.com/imyinnan/ob-timetickme.git
        /// git@gitee.com:imyinnan/ob-timetickme.git
        /// </summary>
        /// <param name="url"></param>
        /// <returns></returns>
        /// <exception cref="InvalidDataException"></exception>
        public static (string, string) GetOwnerRepoByUrl(string url)
        {
            if (url.StartsWith("git"))
            {
                var items = url.Split(':');

                var parts = items[1].Split('/');
                var owner = parts[0];
                var repo = parts[1].Split('.')[0];

                return (owner, repo);
            }
            else if (url.StartsWith("https"))
            {
                var path = url.Replace("https://gitee.com/", "");

                var parts = path.Split('/');
                var owner = parts[0];
                var repo = parts[1].Split('.')[0];

                return (owner, repo);
            }
            else
            {
                throw new InvalidDataException("不支持的url");
            }
        }

        public static string GetToken()
        {
            if (!File.Exists(tokenFile))
            {
                var env = Environment.GetEnvironmentVariable("GITEE_TOKEN");
                if (!string.IsNullOrEmpty(env))
                {
                    return env;
                }

                throw new InvalidDataException("Token 不存在");
            }
            else
            {
                return File.ReadAllText(tokenFile);
            }
        }

        public static void SetToken(string token)
        {
            try
            {
                if (!Directory.Exists(giteeDir))
                {
                    Directory.CreateDirectory(giteeDir);
                }
                File.WriteAllText(tokenFile, token);

                var api = new GiteeApi(token);
                var result = api.GetUserInfo().Result;
                if (result.Code == 0)
                {
                    var user = result.Data;
                    File.WriteAllText(userFile, user);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"异常：{ex.Message}");
            }
        }

        public static string GetUserName()
        {
            if (!File.Exists(userFile))
            {
                return "";
            }
            return File.ReadAllText(userFile);
        }

        public static void SaveRepo(List<Repo> repos)
        {
            try
            {
                var json = JsonConvert.SerializeObject(repos);
                File.WriteAllText(reposFile, json);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
        }

        public static List<Repo> LoadRepo()
        {
            try
            {
                if (!File.Exists(reposFile))
                {
                    return [];
                }

                var json = File.ReadAllText(reposFile);

                var repos = JsonConvert.DeserializeObject<List<Repo>>(json);

                return repos ?? [];
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                return [];
            }
        }
    }
}
