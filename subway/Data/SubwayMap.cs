using System;
using System.Collections.Generic;

using Newtonsoft.Json;

namespace Subway.Data
{
    public class SubwayMap
    {
        [JsonProperty("title")]
        public string Title { get; set; }

        [JsonProperty("stations")]
        public IList<SubwayStation> Stations { get; set; }

        [JsonProperty("lines")]
        public IList<SubwayLine> Lines { get; set; }

        [JsonProperty("transforms")]
        public IList<IList<int>> Transforms { get; set; }

        [JsonConstructor]
        public SubwayMap()
        {
            Title = "";
            Lines = new List<SubwayLine>();
            Stations = new List<SubwayStation>();
            Transforms = new List<IList<int>>();
        }
    }
}
