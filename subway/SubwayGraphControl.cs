using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;
using Subway.Data;
using Newtonsoft.Json;

namespace Subway
{
    public partial class SubwayGraphControl : UserControl
    {
        public SubwayGraphControl()
        {
            InitializeComponent();
            SetStyle(ControlStyles.AllPaintingInWmPaint | ControlStyles.OptimizedDoubleBuffer
                | ControlStyles.ResizeRedraw | ControlStyles.UserPaint, true);
        }

        private SubwayMap m_map;
        private Dictionary<string, int> m_stationIds = new Dictionary<string, int>();
        private Dictionary<string, int> m_lineIds = new Dictionary<string, int>();
        private UndirectedGraph m_graph = new UndirectedGraph();
        private int m_source;
        private int m_target;
        private KeyValuePair<int, List<KeyValuePair<int, int>>> m_shorestPath;


        /// <summary>
        /// 获取地铁线路图。
        /// </summary>
        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public SubwayMap Map
        {
            get { return m_map; }
        }

        /// <summary>
        /// 获取地铁线路抽象图。
        /// </summary>
        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public UndirectedGraph Graph
        {
            get { return m_graph; }
        }

        /// <summary>
        /// 获取或设置起点。
        /// </summary>
        [Browsable(false)]
        public int Source
        {
            get { return m_source; }
            set { m_source = value; }
        }

        /// <summary>
        /// 获取或设置终点。
        /// </summary>
        [Browsable(false)]
        public int Target
        {
            get { return m_target; }
            set { m_target = value; }
        }

        [Browsable(false)]
        public KeyValuePair<int, List<KeyValuePair<int, int>>> ShorestPath
        {
            get { return m_shorestPath; }
            set { m_shorestPath = value; }
        }

        /// <summary>
        /// 从文件打开地铁线路图。
        /// </summary>
        /// <param name="fileName">文件名。</param>
        /// <exception cref="ArgumentNullException"/>
        /// <exception cref="FileNotFoundException"/>
        public void OpenFromFile(string fileName)
        {
            if (fileName == null) throw new ArgumentNullException(nameof(fileName));
            if (!File.Exists(fileName)) throw new FileNotFoundException();

            m_map = JsonConvert.DeserializeObject<SubwayMap>(File.ReadAllText(fileName));

            for (int i = 0; i < Map.Stations.Count; i++)
            {
                m_stationIds[Map.Stations[i].Name] = i;
            }

            for (int i = 0; i < Map.Lines.Count; i++)
            {
                m_lineIds[Map.Lines[i].Name] = i;
            }

            foreach (SubwayLine line in Map.Lines)
            {
                for (int i = 1; i < line.Path.Count; i++)
                {
                    int from = m_stationIds[line.Path[i - 1]];
                    int to = m_stationIds[line.Path[i]];
                    Graph.AddEdge(from, to, m_lineIds[line.Name]);
                }
            }
        }

        /// <summary>
        /// 查找最短路线。
        /// </summary>
        /// <param name="source"></param>
        /// <param name="target"></param>
        /// <exception cref="ArgumentException"/>
        public void FindPath(string source, string target)
        {
            if (!m_stationIds.ContainsKey(source))
                throw new ArgumentException($"Station {source} not exist.");
            if (!m_stationIds.ContainsKey(target))
                throw new ArgumentException($"Station {target} not exist.");

            Source = m_stationIds[source];
            Target = m_stationIds[target];
            ShorestPath = Graph.ShortestPath(Target, Source);
        }

    }
}
