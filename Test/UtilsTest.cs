using GiteeCli;

namespace Test
{
    public class UtilsTest
    {
        [Theory]
        [InlineData("git@gitee.com:imyinnan/gitee-cli.git")]
        [InlineData("https://gitee.com/imyinnan/gitee-cli.git")]
        public void TestGetOwnerRepoByUrl(string url)
        {
            var (own, repo) = Utils.GetOwnerRepoByUrl(url);

            Assert.Equal("imyinnan", own);
            Assert.Equal("gitee-cli", repo);
        }
    }
}
