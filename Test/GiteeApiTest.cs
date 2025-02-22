﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GiteeCli;

namespace Test
{
    public class GiteeApiTest
    {
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
        [InlineData("git@gitee.com:imyinnan/test.git")]
        public async Task TestClearRepo(string url)
        {
            var token = Utils.GetToken();
            Assert.Equal(32, token.Length);

            var api = new GiteeApi(token);
            var result = await api.ClearRepo(url);
            Assert.Equal(0, result.Code);
        }

        [Fact]
        public async Task TestGetStarRepo()
        {
            var token = Utils.GetToken();
            Assert.Equal(32, token.Length);

            var api = new GiteeApi(token);
            var result = await api.GetStarRepo();

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
            var result = await api.GetRepoUrls();

            Assert.Equal(0, result.Code);
        }
    }
}
