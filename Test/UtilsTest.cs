using GiteeCli;

namespace Test
{
    public class UtilsTest
    {
        [Theory]
        [InlineData("git@gitee.com:imyinnan/ob-timetickme.git")]
        [InlineData("https://gitee.com/imyinnan/ob-timetickme.git")]
        public void TestGetOwnerRepoByUrl(string url)
        {
            var (own, repo) = Utils.GetOwnerRepoByUrl(url);

            Assert.Equal("imyinnan", own);
            Assert.Equal("ob-timetickme", repo);
        }
    }
}
