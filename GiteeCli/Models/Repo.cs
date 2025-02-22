using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GiteeCli.Models
{
    public class Repo
    {
        /// <summary>
        /// ssh地址
        /// </summary>
        public string SshUrl { get; set; } = "";

        /// <summary>
        /// http地址
        /// </summary>
        public string HttpUrl { get; set; } = "";

        /// <summary>
        /// 仓库全名
        /// </summary>
        public string FullName { get; set; } = "";

        /// <summary>
        /// 仓库描述
        /// </summary>
        public string Description { get; set; } = "";

        /// <summary>
        /// 是否是私有仓库
        /// </summary>
        public bool IsPublic { get; set; }

        /// <summary>
        /// star数量
        /// </summary>
        public int StargazersCount { get; set; }

        /// <summary>
        /// 编程语言
        /// </summary>
        public string Language { get; set; } = "";
    }
}
