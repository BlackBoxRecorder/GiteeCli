using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace GiteeCli.Models
{
    public class Gists
    {
        public int Index { get; set; }
        public required string Id { get; set; }

        public string Description { get; set; } = "";

        public Dictionary<string, GistsFile> Files { get; set; } = [];
    }

    public class GistsFile
    {
        public int Size { get; set; }

        [JsonProperty("raw_url")]
        public required string RawUrl { get; set; }

        public string Type { get; set; } = "";

        public bool Truncated { get; set; }

        public string Content { get; set; } = "";
    }
}
