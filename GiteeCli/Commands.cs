using CliWrap;
using ConsoleAppFramework;

namespace GiteeCli
{
    internal class Commands
    {
        private static readonly string giteeDir = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
            "GiteeCli"
        );
        private static readonly string repoFile = Path.Combine(giteeDir, "repos.txt");
        private static readonly string tokenFile = Path.Combine(giteeDir, "token.txt");

        public Commands()
        {
            if (!Directory.Exists(giteeDir))
            {
                Directory.CreateDirectory(giteeDir);
            }
        }

        [Command("")]
        public async Task SetToken(string token)
        {
            try
            {
                await File.WriteAllTextAsync(tokenFile, token);
                Console.WriteLine("Token 设置成功");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"异常：{ex.Message}");
            }
        }

        [Command("list")]
        public async Task List()
        {
            try
            {
                var token = await File.ReadAllTextAsync(tokenFile);
                var repos = await GiteeApi.GetRepoUrls(token);
                int index = 1;
                foreach (var repo in repos)
                {
                    Console.WriteLine($"{index,3}   {repo}");
                    index++;
                }
                Console.WriteLine();

                await File.WriteAllLinesAsync(repoFile, repos);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"异常：{ex.Message}");
            }
        }

        [Command("clone")]
        public async Task CloneById(int id = 0)
        {
            try
            {
                if (!File.Exists(repoFile))
                {
                    Console.WriteLine("请先执行：GiteeCli list");
                    return;
                }

                var repos = await File.ReadAllLinesAsync("repos.txt");

                var urls = new List<string>();
                if (id > 0)
                {
                    urls.Add(repos[id - 1]);
                }
                else
                {
                    urls.AddRange(repos);
                }

                foreach (var url in urls)
                {
                    Console.WriteLine($"克隆仓库：{url}");

                    var cmd = Cli.Wrap("git")
                        .WithArguments(args => args.Add("clone").Add(url).Add("--depth").Add(20));

                    await cmd.ExecuteAsync();
                }

                Console.WriteLine("克隆结束");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"{ex.Message}");
            }
        }
    }
}
