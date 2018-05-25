using System;
using System.IO;
using System.Collections.Generic;
using System.Windows.Forms;

using Newtonsoft.Json;
using WindowsFormsApp1;

namespace Subway
{
    using Data;

    public static class Program
    {
        static SubwayMap map;
        static Dictionary<string, int> stationIds;
        static Dictionary<string, int> lineIds;
        static UndirectedGraph graph;

        public static SubwayMap LoadMap(string fileName)
        {
            string content = File.ReadAllText(fileName);
            return JsonConvert.DeserializeObject<SubwayMap>(content);
        }

        public static void InitMap()
        {
            map = LoadMap("BeijingSubwayMap.json");
            stationIds = new Dictionary<string, int>();
            lineIds = new Dictionary<string, int>();
            graph = new UndirectedGraph();

            for (int i = 0; i < map.Stations.Count; i++)
                stationIds[map.Stations[i].Name] = i;

            for (int i = 0; i < map.Lines.Count; i++)
                lineIds[map.Lines[i].Name] = i;

            foreach (SubwayLine line in map.Lines)
            {
                for (int i = 1; i < line.Path.Count; i++)
                {
                    int from = stationIds[line.Path[i - 1]];
                    int to = stationIds[line.Path[i]];
                    graph.AddEdge(from, to, lineIds[line.Name]);
                }
            }
        }

        [STAThread]
        public static void Main(string[] args)
        {
            if (args[0] == "/g")
            {
                Application.EnableVisualStyles();
                Application.SetCompatibleTextRenderingDefault(false);
                Application.Run(new FormMain());
            }
            else
            {
                InitMap();
                if (!stationIds.ContainsKey(args[1]) || !stationIds.ContainsKey(args[2]))
                {
                    Console.WriteLine("站名错误！");
                    return;
                }

                int source = stationIds[args[1]];
                int target = stationIds[args[2]];

                KeyValuePair<int, List<KeyValuePair<int, int>>> sp = graph.ShortestPath(target, source);

                if (sp.Key == 0x3f3f3f3f)
                {
                    Console.WriteLine("不可到达！");
                    return;
                }

                Console.WriteLine(sp.Key);
                Console.Write(map.Stations[source].Name);
                for (int i = 0; i < sp.Value.Count; i++)
                {
                    if (i != 0 && sp.Value[i].Value != sp.Value[i - 1].Value)
                        Console.Write(" 换乘" + map.Lines[sp.Value[i].Value].Name);
                    Console.Write("\n" + map.Stations[sp.Value[i].Key].Name);
                }
                Console.WriteLine();
            }
        }
    }
}
