using System;
using System.Collections.Generic;

using Newtonsoft.Json;

namespace Subway.Data
{
    public class SubwayStation
    {
        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("x")]
        public int X { get; set; }

        [JsonProperty("y")]
        public int Y { get; set; }

        [JsonConstructor]
        public SubwayStation()
        {
            Name = string.Empty;
            X = Y = 0;
        }
    }
}
