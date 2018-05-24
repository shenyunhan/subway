using System;
using System.Collections.Generic;

using Newtonsoft.Json;

namespace Subway.Data
{
    public class SubwayLine
    {
        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("color")]
        public int Color { get; set; }

        [JsonProperty("path")]
        public IList<string> Path { get; set; }

        [JsonConstructor]
        public SubwayLine()
        {
            Name = string.Empty;
            Color = 0;
            Path = new List<string>();
        }
    }
}
