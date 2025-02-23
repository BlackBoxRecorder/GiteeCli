using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GiteeCli;

namespace Test
{
    public class GiteeApiTest
    {
        [Fact(Skip = "跳过")]
        public async Task TestRepoWorkFlow()
        {
            //先创建一个test仓库
            //列出所有仓库
            //清空一个仓库
            //删除一个仓库

            var token = Utils.GetToken();
            Assert.Equal(32, token.Length);

            var api = new GiteeApi(token);

            var result = await api.GetAllRepos();
            Assert.Equal(0, result.Code);
            Assert.True(result.Data.Count > 10);

            var repo = result.Data.First(r => r.Name == "test");
            Assert.NotNull(repo);

            var clearResult = await api.ClearRepo(repo.Name);
            Assert.Equal(0, clearResult.Code);

            var deleteResult = await api.DeleteRepo(repo.Name);
            Assert.Equal(0, deleteResult.Code);
        }

        [Fact()]
        public async Task TestGistsWorkflow()
        {
            //列出所有Gists
            //创建一个Gist
            //更新一个Gist
            //删除一个Gist

            var token = Utils.GetToken();
            Assert.Equal(32, token.Length);

            var api = new GiteeApi(token);
            var title = "自动单元测试流程";
            var fileContent = "自动测试文件内容";
            var newFileContent = fileContent + $" update:{DateTime.Now}";

            var filename = DateTime.Now.ToString("yyyy-MM-dd-HH-mm-ss");
            var fullpath = Path.Combine(Environment.CurrentDirectory, filename);
            await File.WriteAllTextAsync(fullpath, fileContent);

            var createResult = await api.CreateGists(title, filename);
            Assert.Equal(0, createResult.Code);

            var result = await api.GetAllGists();
            Assert.Equal(0, result.Code);
            Assert.True(result.Data.Count > 0);

            var gists = result.Data.First(g => g.Description == title);
            Assert.NotNull(gists);

            await File.WriteAllTextAsync(fullpath, newFileContent);

            var updateResult = await api.UpdateGists(gists.Id, title, filename);
            Assert.Equal(0, updateResult.Code);
            File.Delete(fullpath);

            var downloadResult = await api.GetGistsById(gists.Id);
            Assert.Equal(0, downloadResult.Code);
            Assert.NotNull(downloadResult.Data);

            Assert.Equal(downloadResult.Data.Files[filename].Content, newFileContent);

            var deleteResult = await api.DeleteGists(gists.Id);
            Assert.Equal(0, deleteResult.Code);
        }

        [Theory(Skip = "暂时禁用此测试")]
        [InlineData("git@gitee.com:imyinnan/test.git")]
        public async Task TestDeleteRepo(string url)
        {
            var token = Utils.GetToken();
            Assert.Equal(32, token.Length);

            var api = new GiteeApi(token);
            var result = await api.DeleteRepo(url);
            Assert.Equal(0, result.Code);
        }

        [Theory(Skip = "暂时禁用此测试")]
        [InlineData("test")]
        public async Task TestClearRepo(string name)
        {
            var token = Utils.GetToken();
            Assert.Equal(32, token.Length);

            var api = new GiteeApi(token);
            var result = await api.ClearRepo(name);
            Assert.Equal(0, result.Code);
        }

        [Fact]
        public async Task TestGetStarRepo()
        {
            var token = Utils.GetToken();
            Assert.Equal(32, token.Length);

            var api = new GiteeApi(token);
            var result = await api.GetStarRepos();

            Assert.Equal(0, result.Code);
        }

        [Theory(Skip = "暂时禁用此测试")]
        [InlineData("leo_song/bnuoge")]
        public async Task TestCancelStarRepo(string fullName)
        {
            var token = Utils.GetToken();
            Assert.Equal(32, token.Length);

            var api = new GiteeApi(token);

            var owner = fullName.Split('/')[0];
            var repo = fullName.Split("/")[1];

            var result = await api.CancelStarRepo(owner, repo);

            Assert.Equal(0, result.Code);
        }

        [Fact]
        public async Task TestGetRepo()
        {
            var token = Utils.GetToken();
            Assert.Equal(32, token.Length);

            var api = new GiteeApi(token);
            var result = await api.GetAllRepos();

            Assert.Equal(0, result.Code);
        }

        [Fact]
        public async Task TestGetGists()
        {
            var token = Utils.GetToken();
            Assert.Equal(32, token.Length);

            var api = new GiteeApi(token);
            var result = await api.GetAllGists();

            Assert.Equal(0, result.Code);
        }

        [Theory(Skip = "暂时禁用此测试")]
        [InlineData("测试片段", "test.txt")]
        public async Task TestCreateGist(string title, string file)
        {
            var token = Utils.GetToken();
            Assert.Equal(32, token.Length);

            var api = new GiteeApi(token);

            var result = await api.CreateGists(title, file);

            Assert.Equal(0, result.Code);
        }
    }
}
