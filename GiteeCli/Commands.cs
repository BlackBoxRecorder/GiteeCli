using System;
using CliWrap;
using ConsoleAppFramework;
using GiteeCli.Models;
using Spectre.Console;

namespace GiteeCli
{
    internal class Commands
    {
        private readonly GiteeApi api;

        public Commands()
        {
            var token = Utils.GetToken();
            api = new GiteeApi(token);
        }

        [Command("set token")]
        public void SetToken(string token)
        {
            Utils.SetToken(token);
            AnsiConsole.MarkupLine("[green]设置成功[/]");
        }

        [Command("get token")]
        public void GetToken()
        {
            var token = Utils.GetToken();
            AnsiConsole.MarkupLine($"[green]{token}[/]");
        }

        [Command("list")]
        public async Task List()
        {
            await AnsiConsole
                .Status()
                .StartAsync(
                    "Working...",
                    async ctx =>
                    {
                        try
                        {
                            var result = await api.GetRepoUrls();
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
                            Console.WriteLine($"异常：{ex.Message}");
                        }
                    }
                );
        }

        [Command("clone")]
        public async Task CloneById(int id = 0)
        {
            await AnsiConsole
                .Status()
                .StartAsync(
                    "Working...",
                    async ctx =>
                    {
                        try
                        {
                            var repos = Utils.LoadRepo();

                            if (repos.Count < 1)
                            {
                                AnsiConsole.MarkupLine("请先执行: [red] gitee-cli list [/]");
                                return;
                            }

                            var urls = new List<string>();
                            if (id > 0)
                            {
                                urls.Add(repos[id - 1].SshUrl);
                            }
                            else
                            {
                                urls.AddRange(repos.Select(r => r.SshUrl));
                            }

                            foreach (var url in urls)
                            {
                                AnsiConsole.WriteLine($"克隆仓库：{url}");

                                var cmd = Cli.Wrap("git")
                                    .WithArguments(args =>
                                        args.Add("clone").Add(url).Add("--depth").Add(20)
                                    );

                                await cmd.ExecuteAsync();
                            }

                            AnsiConsole.WriteLine("克隆结束");
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine($"{ex.Message}");
                        }
                    }
                );
        }

        [Command("delete")]
        public async Task Delete(int id)
        {
            await AnsiConsole
                .Status()
                .StartAsync(
                    "Working...",
                    async ctx =>
                    {
                        try
                        {
                            var repos = Utils.LoadRepo();
                            var repo = repos[id - 1];
                            await api.DeleteRepo(repo.HttpUrl);
                            AnsiConsole.WriteLine($"已删除仓库：{repo.FullName}");
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine($"{ex.Message}");
                        }
                    }
                );
        }

        [Command("star list")]
        public async Task StarList()
        {
            await AnsiConsole
                .Status()
                .StartAsync(
                    "Working...",
                    async ctx =>
                    {
                        try
                        {
                            var result = await api.GetStarRepo();
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
                );
        }

        private static Table BuildRepoTable(List<Repo> starRepos)
        {
            var table = new Table();
            table.Border = TableBorder.Rounded;

            table.AddColumn("全名").Ascii2Border();
            table.AddColumn("地址").Ascii2Border();
            table.AddColumn("Star").Ascii2Border();
            table.AddColumn("语言").Ascii2Border();
            table.AddColumn("描述").Ascii2Border();

            foreach (var repo in starRepos)
            {
                table.AddRow(
                    $"{repo.FullName}",
                    $"[green]{repo.HttpUrl}[/]",
                    $"{repo.StargazersCount}",
                    repo.Language,
                    repo.Description
                );
            }

            return table;
        }
    }
}
