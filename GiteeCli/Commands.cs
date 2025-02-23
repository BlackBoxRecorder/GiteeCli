using System;
using CliWrap;
using ConsoleAppFramework;
using GiteeCli.Models;
using Spectre.Console;

namespace GiteeCli
{
    internal class Commands
    {
        private readonly CommandHandlers _handlers;

        public Commands()
        {
            var token = Utils.GetToken();
            var api = new GiteeApi(token);

            _handlers = new CommandHandlers(api);
        }

        /// <summary>
        /// 设置 Gitee API Token
        /// </summary>
        /// <param name="token">token</param>
        [Command("set token")]
        public void SetToken(string token)
        {
            Utils.SetToken(token);
            AnsiConsole.MarkupLine("[green]设置成功[/]");
        }

        /// <summary>
        /// 获取 Gitee API Token
        /// </summary>
        [Command("get token")]
        public void GetToken()
        {
            var token = Utils.GetToken();
            AnsiConsole.MarkupLine($"[green]{token}[/]");
        }

        /// <summary>
        /// 列出所有仓库
        /// </summary>
        /// <returns></returns>
        [Command("list")]
        public async Task ListRepo()
        {
            await AnsiConsole
                .Status()
                .StartAsync(
                    "Working...",
                    async ctx =>
                    {
                        await _handlers.RepoListHandler();
                    }
                );
        }

        /// <summary>
        /// 克隆仓库，如果参数为空则克隆所有仓库到当前目录
        /// </summary>
        /// <param name="name">仓库名称</param>
        /// <returns></returns>
        [Command("clone")]
        public async Task CloneById(string name = "")
        {
            await AnsiConsole
                .Status()
                .StartAsync(
                    "Working...",
                    async ctx =>
                    {
                        await _handlers.RepoCloneHandler(name);
                    }
                );
        }

        /// <summary>
        /// 删除一个仓库
        /// </summary>
        /// <param name="name">仓库名称</param>
        /// <returns></returns>
        [Command("delete")]
        public async Task DeleteRepo(string name)
        {
            await AnsiConsole
                .Status()
                .StartAsync(
                    "Working...",
                    async ctx =>
                    {
                        await _handlers.RepoDeleteHandler(name);
                    }
                );
        }

        /// <summary>
        /// 列出 Star 的仓库
        /// </summary>
        /// <returns></returns>
        [Command("star list")]
        public async Task StarList()
        {
            await AnsiConsole
                .Status()
                .StartAsync(
                    "Working...",
                    async ctx =>
                    {
                        await _handlers.StarListHandler();
                    }
                );
        }

        /// <summary>
        /// 列出用户的代码片段
        /// </summary>
        /// <returns></returns>
        [Command("gists list")]
        public async Task GistsList()
        {
            await AnsiConsole
                .Status()
                .StartAsync(
                    "Working...",
                    async ctx =>
                    {
                        await _handlers.GistsListHandler();
                    }
                );
        }

        /// <summary>
        /// 创建代码片段
        /// </summary>
        /// <param name="title">代码片段名称</param>
        /// <param name="file">代码片段文件的文件名，仅支持当前目录下的文件</param>
        /// <returns></returns>
        [Command("gists create")]
        public async Task GistsList(string title, string file)
        {
            await AnsiConsole
                .Status()
                .StartAsync(
                    "Working...",
                    async ctx =>
                    {
                        await _handlers.GistsCreateHandler(title, file);
                    }
                );
        }

        /// <summary>
        /// 下载一个代码片段
        /// </summary>
        /// <param name="id">代码片段的ID</param>
        /// <returns></returns>
        [Command("gists download")]
        public async Task GistsDownload(string id)
        {
            await AnsiConsole
                .Status()
                .StartAsync(
                    "Working...",
                    async ctx =>
                    {
                        await _handlers.GistsDownloadHandler(id);
                    }
                );
        }

        /// <summary>
        /// 删除一个代码片段
        /// </summary>
        /// <param name="id">代码片段的ID</param>
        /// <returns></returns>
        [Command("gists delete")]
        public async Task GistsDelete(string id)
        {
            await AnsiConsole
                .Status()
                .StartAsync(
                    "Working...",
                    async ctx =>
                    {
                        await _handlers.GistsDeleteHandler(id);
                    }
                );
        }

        /// <summary>
        /// 更新一个代码片段
        /// </summary>
        /// <param name="id">代码片段的ID</param>
        /// <param name="title">代码片段名称</param>
        /// <param name="file">代码片段文件的文件名，仅支持当前目录下的文件</param>
        /// <returns></returns>
        [Command("gists update")]
        public async Task GistsDelete(string id, string title, string file)
        {
            await AnsiConsole
                .Status()
                .StartAsync(
                    "Working...",
                    async ctx =>
                    {
                        await _handlers.GistsUpdateHandler(id, title, file);
                    }
                );
        }
    }
}
