using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CliWrap;
using GiteeCli.Models;
using Spectre.Console;

namespace GiteeCli
{
    internal class CommandHandlers
    {
        private readonly GiteeApi api;

        public CommandHandlers(GiteeApi api)
        {
            this.api = api;
        }

        public async Task GistsListHandler()
        {
            try
            {
                var result = await api.GetGists();
                if (result.Code != 0)
                {
                    AnsiConsole.WriteLine(result.Message);
                    return;
                }

                var gists = result.Data;
                Table table = BuildGistsTable(gists);
                AnsiConsole.Write(table);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"{ex.Message}");
            }
        }

        public async Task StarListHandler()
        {
            try
            {
                var result = await api.GetStarRepos();
                if (result.Code != 0)
                {
                    AnsiConsole.WriteLine(result.Message);
                    return;
                }

                var starRepos = result.Data;
                Table table = BuildRepoTable(starRepos);
                AnsiConsole.Write(table);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"{ex.Message}");
            }
        }

        public async Task DeleteRepoHandler(string name)
        {
            try
            {
                var repos = Utils.LoadRepo();
                if (!repos.Any(r => r.Name == name))
                {
                    AnsiConsole.WriteLine($"仓库 [green]{name}[/] 不存在");
                    return;
                }

                await api.DeleteRepo(name);
                AnsiConsole.WriteLine($"已删除仓库：{name}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"{ex.Message}");
            }
        }

        public async Task CloneRepoHandler(string name)
        {
            try
            {
                var repos = Utils.LoadRepo();

                if (repos.Count < 1 || !repos.Any(r => r.Name == name))
                {
                    //获取所有仓库
                    await RepoListHandler();
                    repos = Utils.LoadRepo();
                }

                var urls = new List<string>();

                if (string.IsNullOrEmpty(name))
                { //clone所有仓库
                    urls.AddRange(repos.Select(r => r.SshUrl));
                }
                else
                {
                    if (!repos.Any(r => r.Name == name))
                    {
                        AnsiConsole.MarkupLine($"仓库 : [red]{name}[/] 不存在");
                        return;
                    }
                    urls.Add(repos.First(r => r.Name == name).SshUrl);
                }

                foreach (var url in urls)
                {
                    AnsiConsole.WriteLine($"克隆仓库：{url}");

                    var cmd = Cli.Wrap("git")
                        .WithArguments(args => args.Add("clone").Add(url).Add("--depth").Add(20));

                    await cmd.ExecuteAsync();
                }

                AnsiConsole.WriteLine("克隆结束");
            }
            catch (Exception ex)
            {
                AnsiConsole.WriteException(ex);
            }
        }

        public async Task RepoListHandler()
        {
            try
            {
                var result = await api.GetRepos();
                if (result.Code != 0)
                {
                    AnsiConsole.WriteLine(result.Message);
                    return;
                }
                Utils.SaveRepo(result.Data);
                var table = BuildRepoTable(result.Data);
                AnsiConsole.Write(table);
            }
            catch (Exception ex)
            {
                AnsiConsole.WriteException(ex);
            }
        }

        public async Task CreateGistHandler(string title, string file)
        {
            try
            {
                var result = await api.CreateGist(title, file);
                if (result.Code != 0)
                {
                    AnsiConsole.WriteLine(result.Message);
                    return;
                }
                AnsiConsole.WriteLine("创建成功");
            }
            catch (Exception ex)
            {
                AnsiConsole.WriteException(ex);
            }
        }

        public async Task DownloadGistHandler(string id)
        {
            try
            {
                var result = await api.GetGist(id);
                if (result.Code != 0)
                {
                    AnsiConsole.WriteLine(result.Message);
                    return;
                }

                var files = result.Data.Files;

                foreach (var file in files)
                {
                    var path = Path.Combine(Environment.CurrentDirectory, file.Key);
                    await File.WriteAllTextAsync(path, file.Value.Content);
                }

                AnsiConsole.WriteLine($"下载成功, ID = {result.Data.Id}");
            }
            catch (Exception ex)
            {
                AnsiConsole.WriteException(ex);
            }
        }

        public async Task DeleteGistHandler(string id)
        {
            try
            {
                var result = await api.DeleteGist(id);
                if (result.Code != 0)
                {
                    AnsiConsole.WriteLine(result.Message);
                    return;
                }

                AnsiConsole.WriteLine($"删除代码片段, ID = {id}");
            }
            catch (Exception ex)
            {
                AnsiConsole.WriteException(ex);
            }
        }

        public async Task UpdateGistHandler(string id, string title, string file)
        {
            try
            {
                var result = await api.UpdateGist(id, title, file);
                if (result.Code != 0)
                {
                    AnsiConsole.WriteLine(result.Message);
                    return;
                }

                AnsiConsole.WriteLine($"更新代码片段, ID = {id}");
            }
            catch (Exception ex)
            {
                AnsiConsole.WriteException(ex);
            }
        }

        private static Table BuildRepoTable(List<Repo> starRepos)
        {
            var table = new Table();
            table.Border = TableBorder.Rounded;

            table.AddColumn("序号");
            table.AddColumn("名称");
            table.AddColumn("地址");
            table.AddColumn("星标");
            table.AddColumn("状态");
            table.AddColumn("语言");

            foreach (var repo in starRepos)
            {
                table.AddRow(
                    repo.Index.ToString().PadLeft(3, ' '),
                    $"{repo.Name}",
                    $"{repo.HttpUrl}",
                    $"{repo.StargazersCount}",
                    repo.IsPublic ? "[green]公开[/]" : "私有",
                    repo.Language
                );
            }

            return table;
        }

        private static Table BuildGistsTable(List<Gists> gists)
        {
            var table = new Table();

            table.AddColumn("序号");
            table.AddColumn("ID");
            table.AddColumn("描述");
            table.AddColumn("文件");

            foreach (var gist in gists)
            {
                var files = gist.Files;

                foreach (var file in files)
                {
                    table.AddRow(
                        gist.Index.ToString().PadLeft(3, ' '),
                        $"{gist.Id}",
                        $"{gist.Description}",
                        $"{file.Key}"
                    );
                }
            }

            return table;
        }
    }
}
