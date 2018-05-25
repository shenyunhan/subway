using System;
using System.Collections.Generic;

namespace Subway
{
    public class UndirectedGraph
    {
        private struct Edge
        {
            public int To { get; private set; }
            public int Line { get; private set; }

            public Edge(int to, int line)
            {
                To = to;
                Line = line;
            }
        }

        private Dictionary<int, List<Edge>> adj;

        public UndirectedGraph()
        {
            adj = new Dictionary<int, List<Edge>>();
        }

        public void AddEdge(int from, int to, int line)
        {
            if (!adj.ContainsKey(from))
                adj[from] = new List<Edge>();
            if (!adj.ContainsKey(to))
                adj[to] = new List<Edge>();

            adj[from].Add(new Edge(to, line));
            adj[to].Add(new Edge(from, line));
        }

        public KeyValuePair<int, List<KeyValuePair<int, int>>> ShortestPath(int source, int target)
        {
            Dictionary<int, int> dist = new Dictionary<int, int>();
            Dictionary<int, KeyValuePair<int, int>> pre = new Dictionary<int, KeyValuePair<int, int>>();
            foreach (int id in adj.Keys)
            {
                dist[id] = 0x3f3f3f3f;
            }
            dist[source] = 0;

            Queue<Edge> q = new Queue<Edge>();
            HashSet<Edge> vis = new HashSet<Edge>();
            q.Enqueue(new Edge(source, -1));
            vis.Add(new Edge(source, -1));

            while (q.Count != 0)
            {
                Edge x = q.Dequeue();
                foreach (Edge e in adj[x.To])
                {
                    int temp = dist[x.To] + 1;
                    if (x.Line != -1 && e.Line != x.Line) temp += 3;
                    if (dist[e.To] > temp)
                    {
                        dist[e.To] = temp;
                        pre[e.To] = new KeyValuePair<int, int>(x.To, e.Line);
                        if (!vis.Contains(e))
                        {
                            q.Enqueue(e);
                            vis.Add(e);
                        }
                    }
                }
                vis.Remove(x);
            }

            List<KeyValuePair<int, int>> path = new List<KeyValuePair<int, int>>();
            for (int i = target; pre.ContainsKey(i); i = pre[i].Key)
                path.Add(pre[i]);
            return new KeyValuePair<int, List<KeyValuePair<int, int>>>(dist[target], path);
        }
    }
}
