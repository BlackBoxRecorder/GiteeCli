using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GiteeCli.Models
{
    public class Gists
    {
        public required string Id { get; set; }

        public string Description { get; set; } = "";

        public Dictionary<string, GistsFile> Files { get; set; } = [];
    }

    public class GistsFile
    {
        public int Size { get; set; }
        public required string RawUrl { get; set; }

        public string Type { get; set; } = "";

        public bool Truncated { get; set; }

        public string Content { get; set; } = "";
    }
}
