using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Windows.Forms;
using Subway.Data;
using Newtonsoft.Json;
using Subway;


namespace WindowsFormsApp1
{
    public partial class FormMain : Form
    {
        SubwayMap map;
        Dictionary<string, int> stationIds;
        Dictionary<string, int> lineIds;
        UndirectedGraph graph;
        List<Point> path;

        public static SubwayMap LoadMap(string fileName)
        {
            string content = File.ReadAllText(fileName);
            return JsonConvert.DeserializeObject<SubwayMap>(content);
        }


        public FormMain()
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

            path = new List<Point>();

            InitializeComponent();
            this.BackgroundImageLayout = ImageLayout.Zoom;

            this.pictureBox1.MouseDown += new System.Windows.Forms.MouseEventHandler(this.pictureBox1_MouseDown);
            this.pictureBox1.MouseMove += new System.Windows.Forms.MouseEventHandler(this.pictureBox1_MouseMove);
            
            this.pictureBox1.MouseUp += new System.Windows.Forms.MouseEventHandler(this.pictureBox1_MouseUp);
            this.pictureBox1.MouseWheel += new System.Windows.Forms.MouseEventHandler(this.pictureBox1_MouseWheel);
            

        }

        private void Form1_Load(object sender, EventArgs e)
        {
        }

        private void label1_Click(object sender, EventArgs e)
        {
        }

        private void label2_Click(object sender, EventArgs e)
        {
        }
        static int radius = 7;
        private void button1_Click(object sender, EventArgs e)
        {
            string start = textBox1.Text.ToString();
            string end = textBox2.Text.ToString();

            if (!stationIds.ContainsKey(start) || !stationIds.ContainsKey(end))
            {
                MessageBox.Show("站名输入错误！");
                return;
            }

            int source = stationIds[start];
            int target = stationIds[end];

            KeyValuePair<int, List<KeyValuePair<int, int>>> sp = graph.ShortestPath(target, source);

            if (sp.Key == 0x3f3f3f3f)
            {
                MessageBox.Show("不可到达！");
                return;
            }


            path.Clear();
            path.Add(new Point(map.Stations[source].X, map.Stations[source].Y));
            foreach (var i in sp.Value)
            {
                path.Add(new Point(map.Stations[i.Key].X, map.Stations[i.Key].Y));
            }

            pictureBox1.Refresh();
            DrawPath();
            
        }

        private void DrawPath()
        {
            Graphics g = pictureBox1.CreateGraphics();
            double scale = (double)pictureBox1.Width / 1034;
            int r = (int)(radius * scale / 2);
            foreach (var i in path)
            {
                int x = (int)(i.X * scale);
                int y = (int)(i.Y * scale);
                g.FillEllipse(Brushes.Black, x - r, y - r, 2 * r, 2 * r);
            }
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
        }

        private void textBox2_TextChanged(object sender, EventArgs e)
        {
        }

        private void pictureBox1_Click(object sender, EventArgs e)
        {
        }

        //Bitmap myBmp;
        Point mouseDownPoint = new Point(); //记录拖拽过程鼠标位置
        bool isMove = false;  //判断鼠标在picturebox上移动时，是否处于拖拽过程(鼠标左键是否按下)
        int zoomStep = 20;   //缩放步长

        //鼠标按下功能
        private void pictureBox1_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                mouseDownPoint.X = Cursor.Position.X;
                mouseDownPoint.Y = Cursor.Position.Y;
                isMove = true;
                pictureBox1.Focus();
            }
        }
        //鼠标松开功能
        private void pictureBox1_MouseUp(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                isMove = false;
            }
        }
        //鼠标移动功能
        private void pictureBox1_MouseMove(object sender, MouseEventArgs e)
        {
            pictureBox1.Focus();
            if (isMove)
            {
                int x, y;
                int moveX, moveY;
                moveX = Cursor.Position.X - mouseDownPoint.X;
                moveY = Cursor.Position.Y - mouseDownPoint.Y;
                x = pictureBox1.Location.X + moveX;
                y = pictureBox1.Location.Y + moveY;

                if (x > 0) x = 0;
                if (y > 0) y = 0;

                if (x + pictureBox1.Width < panel1.Width)
                    x = panel1.Width - pictureBox1.Width;
                if (y + pictureBox1.Height < panel1.Height)
                    y = panel1.Height - pictureBox1.Height;

                pictureBox1.Location = new Point(x, y);
                mouseDownPoint.X = Cursor.Position.X;
                mouseDownPoint.Y = Cursor.Position.Y;
            }
            DrawPath();
        }

        //鼠标滚轮滚动功能
        private void pictureBox1_MouseWheel(object sender, MouseEventArgs e)
        {
            int X = e.Location.X;
            int Y = e.Location.Y;
            int ow = pictureBox1.Width;
            int oh = pictureBox1.Height;
            int VX, VY;
            if (e.Delta > 0)
            {
                pictureBox1.Width += (int)(zoomStep * 1.5690440060698027314112291350531);
                pictureBox1.Height += zoomStep;
                PropertyInfo pInfo = pictureBox1.GetType().GetProperty("ImageRectangle", BindingFlags.Instance |
                  BindingFlags.NonPublic);
                Rectangle rect = (Rectangle)pInfo.GetValue(pictureBox1, null);
                pictureBox1.Width = rect.Width;
                pictureBox1.Height = rect.Height;
            }
            if (e.Delta < 0)
            {
                pictureBox1.Width -= (int)(zoomStep* 1.5690440060698027314112291350531);
                pictureBox1.Height -= zoomStep;
                if (pictureBox1.Width < panel1.Width || pictureBox1.Height < panel1.Height)
                {
                    pictureBox1.Width = panel1.Width;
                    pictureBox1.Height = panel1.Height;

                }
                PropertyInfo pInfo = pictureBox1.GetType().GetProperty("ImageRectangle", BindingFlags.Instance |
                  BindingFlags.NonPublic);
                Rectangle rect = (Rectangle)pInfo.GetValue(pictureBox1, null);
                pictureBox1.Width = rect.Width;
                pictureBox1.Height = rect.Height;
            }
            VX = (int)((double)X * (ow - pictureBox1.Width) / ow);
            VY = (int)((double)Y * (oh - pictureBox1.Height) / oh);
            pictureBox1.Location = new Point(pictureBox1.Location.X + VX, pictureBox1.Location.Y + VY);

            int x = pictureBox1.Location.X;
            int y = pictureBox1.Location.Y;

            if (x > 0) x = 0;
            if (y > 0) y = 0;

            if (x + pictureBox1.Width < panel1.Width)
                x = panel1.Width - pictureBox1.Width;
            if (y + pictureBox1.Height < panel1.Height)
                y = panel1.Height - pictureBox1.Height;

            pictureBox1.Location = new Point(x, y);
            mouseDownPoint.X = Cursor.Position.X;
            mouseDownPoint.Y = Cursor.Position.Y;

        }
        
        private void panel1_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                mouseDownPoint.X = Cursor.Position.X;
                mouseDownPoint.Y = Cursor.Position.Y;
                isMove = true;
            }
        }
        private void panel1_MouseUp(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                isMove = false;
            }
        }
        private void panel1_MouseMove(object sender, MouseEventArgs e)
        {
            panel1.Focus();
            if (isMove)
            {
                int x, y;
                int moveX, moveY;
                moveX = Cursor.Position.X - mouseDownPoint.X;
                moveY = Cursor.Position.Y - mouseDownPoint.Y;
                x = pictureBox1.Location.X + moveX;
                y = pictureBox1.Location.Y + moveY;
                pictureBox1.Location = new Point(x, y);
                mouseDownPoint.X = Cursor.Position.X;
                mouseDownPoint.Y = Cursor.Position.Y;
            }
        }
    }
}

