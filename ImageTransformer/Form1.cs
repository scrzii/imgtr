using ImageTransformer.Algorithms;
using ImageTransformer.Algorithms.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ImageTransformer
{
    public partial class Form1 : Form
    {
        private Bitmap Input { get; set; }
        private Bitmap Output { get; set; }
        private Graphics InputGr { get; set; }
        private Graphics OutputGr { get; set; }
        private List<Line> Buffer { get; set; }
        private List<Line> OutputLines { get; set; }

        private int CW = 20;

        public Form1()
        {
            InitializeComponent();
        }

        private void comboBox1_Click(object sender, EventArgs e)
        {
            comboBox1.Items.Clear();
            comboBox1.Items.AddRange(Directory.GetFiles(@"samples\").Select(_ => _.Split('\\').Last()).ToArray());
        }

        private void comboBox1_SelectedValueChanged(object sender, EventArgs e)
        {
            var imagePath = Path.Combine(@"samples\", comboBox1.SelectedItem.ToString());
            var image = new Bitmap(imagePath);
            State.Width = image.Width;
            State.Height = image.Height;
            State.Original = new Bitmap(image);

            Output = image;
            InputGr.DrawImage(State.Original, 0, 0);

            Update(image);
        }

        private void Update(Image image)
        {
            OutputGr.DrawImage(image, 0, 0);
            State.Buffer = new Bitmap(image);
            InputGr.DrawImage(State.Original, 0, 0);
        }

        private void Form1_Activated(object sender, EventArgs e)
        {
            InputGr = panel1.CreateGraphics();
            OutputGr = panel2.CreateGraphics();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            var points = new List<PlanePoint>();
            for (int x = 0; x < State.Width; x++)
            {
                for (int y = 0; y < State.Height; y++)
                {
                    var color = Output.GetPixel(x, y);
                    points.Add(new PlanePoint(x, y, new double[] { color.R, color.G, color.B }));
                }
            }

            var algo = new KMeans(points);
            algo.Calculate(int.Parse(textBox2.Text));
            var image = State.CreateImage();

            foreach (var point in points)
            {
                var cluster = algo.Centers[point.Label];
                var r = (int)cluster.Coords[0];
                var g = (int)cluster.Coords[1];
                var b = (int)cluster.Coords[2];
                image.SetPixel((int)point.X, (int)point.Y, Color.FromArgb(r, g, b));
            }
            Update(image);
        }

        private Color ParseColor(string hex)
        {
            hex = hex.Trim('#').Trim('\r');
            var r = int.Parse(hex.Substring(0, 2), System.Globalization.NumberStyles.HexNumber);
            var g = int.Parse(hex.Substring(2, 2), System.Globalization.NumberStyles.HexNumber);
            var b = int.Parse(hex.Substring(4, 2), System.Globalization.NumberStyles.HexNumber);
            return Color.FromArgb(r, g, b);
        }

        private void button2_Click(object sender, EventArgs e)
        {
            var image = State.CreateImage();
            for (int x = 0; x < image.Width - 1; x++)
            {
                for (int y = 0; y < image.Height - 1; y++)
                {
                    if (State.Buffer.GetPixel(x, y) != State.Buffer.GetPixel(x + 1, y))
                    {
                        // image.SetPixel(x, y, State.Buffer.GetPixel(x, y));
                        // image.SetPixel(x + 1, y, State.Buffer.GetPixel(x + 1, y));
                        image.SetPixel(x, y, Color.Black);
                        image.SetPixel(x + 1, y, Color.Black);
                    }
                    if (State.Buffer.GetPixel(x, y) != State.Buffer.GetPixel(x, y + 1))
                    {
                        // image.SetPixel(x, y, State.Buffer.GetPixel(x, y));
                        // image.SetPixel(x, y + 1, State.Buffer.GetPixel(x, y + 1));
                        image.SetPixel(x, y, Color.Black);
                        image.SetPixel(x, y + 1, Color.Black);
                    }
                }
            }
            Update(image);
        }

        private void button3_Click(object sender, EventArgs e)
        {
            var image = State.CreateImage();
            var maxNoise = int.Parse(textBox3.Text);
            for (int noise = 1; noise <= maxNoise; noise++) 
            {
                for (int x = 2; x < image.Width - 2; x++)
                {
                    for (int y = 2; y < image.Height - 2; y++)
                    {
                        var color = State.Buffer.GetPixel(x, y);
                        var sim = 0;
                        var colorDict = new Dictionary<Color, int>();
                        for (int i = x - 2; i <= x + 2; i++)
                        {
                            for (int j = y - 2; j <= y + 2; j++)
                            {
                                var c = State.Buffer.GetPixel(i, j);
                                if (c == color)
                                {
                                    sim++;
                                }
                                else
                                {
                                    if (colorDict.ContainsKey(c))
                                    {
                                        colorDict[c]++;
                                    }
                                    else
                                    {
                                        colorDict[c] = 1;
                                    }
                                }
                            }
                        }
                        if (sim >= noise)
                        {
                            image.SetPixel(x, y, color);
                            continue;
                        }
                        image.SetPixel(x, y, colorDict.OrderByDescending(_ => _.Value).First().Key);
                    }
                }
            }
            Update(image);
        }


        private void button4_Click(object sender, EventArgs e)
        {
            var colors = textBox1.Text.Split('\n').Select(_ => ParseColor(_)).ToList();
            var image = State.CreateImage();
            for (int x = 0; x < State.Width; x++)
            {
                for (int y = 0; y < State.Height; y++)
                {
                    image.SetPixel(x, y, GetNearestColor(State.Buffer.GetPixel(x, y), colors));
                }
            }

            Update(image);
        }

        private Color GetNearestColor(Color color, List<Color> colors)
        {
            var minDist = 1e6;
            var res = 0;
            for (int i = 0; i < colors.Count; i++)
            {
                var dist = Math.Pow(color.R - colors[i].R, 2) +
                    Math.Pow(color.G - colors[i].G, 2) +
                    Math.Pow(color.B - colors[i].B, 2);
                if (dist < minDist)
                {
                    minDist = dist;
                    res = i;
                }
            }

            return colors[res];
        }

        private void button5_Click(object sender, EventArgs e)
        {
            var h = new HashSet<Color>();
            var data = new List<List<BFSPoint<Color>>>();
            for (int x = 0; x < State.Width; x++)
            {
                data.Add(new List<BFSPoint<Color>>());
                for (int y = 0; y < State.Height; y++)
                {
                    data[x].Add(new BFSPoint<Color>(x, y, State.Buffer.GetPixel(x, y)));
                    h.Add(data[x][y].Value);
                    if (data[x][y].Value.ToArgb() == Color.Black.ToArgb())
                    {
                        data[x][y].Enabled = true;
                    }
                }
            }
            var bfs = new FlatBFS(data);
            bfs.StartAll(0);
            ClusterizeBfs(bfs);
            ShowLines();
        }

        private void ShowLines()
        {
            var img = State.CreateImage(State.Width * 2, State.Height * 2);
            var gr = Graphics.FromImage(img);
            foreach (var line in OutputLines)
            {
                ShowLine(line, gr);
            }
            Update(img);
        }

        private void ShowLine(Line line, Graphics gr)
        {
            int x1 = (int)line.P1.X;
            int y1 = (int)line.P1.Y;
            int x2 = (int)line.P2.X;
            int y2 = (int)line.P2.Y;
            gr.DrawLine(new Pen(Color.Gray, 3), x1 * 2, y1 * 2, x2 * 2, y2 * 2);
        }

        private void ClusterizeBfs(FlatBFS data)
        {
            var centers = new Dictionary<string, FlatPoint>();
            var keys = new List<List<string>>();
            for (int x = 0; x < State.Width; x++)
            {
                keys.Add(new List<string>());
                for (int y = 0; y < State.Height; y++)
                {
                    keys[x].Add("");
                }
            }
            for (int comp = 0; comp < data.PointList[0].Count; comp++)
            {
                for (int gen = 0; gen < data.PointList[0][comp].Count; gen++)
                {
                    SelectField(data, comp, gen);
                    data.StartAll(1);
                    for (int i = 0; i < data.PointList[1].Count; i++)
                    {
                        var c = $"{comp}_{gen}_{i}";
                        if (!centers.ContainsKey(c))
                        {
                            centers[c] = GetCompAvg(data, i);
                            FillKeys(keys, data, i, c);
                        }
                    }
                }
            }

            var set = new HashSet<Line>();
            for (int x = 0; x < State.Width; x++)
            {
                for (int y = 0; y < State.Height; y++)
                {
                    if (data.Points[x][y].Cluster[0] == -1)
                    {
                        continue;
                    }
                    var key1 = keys[x][y];
                    if (x < State.Width - 1 && data.Points[x + 1][y].Cluster[0] != -1)
                    {
                        var key2 = keys[x + 1][y];
                        if (key1 != key2)
                        {
                            set.Add(new Line(centers[key1], centers[key2]));
                        }
                    }
                    if (y < State.Height - 1 && data.Points[x][y + 1].Cluster[0] != -1)
                    {
                        var key2 = keys[x][y + 1];
                        if (key1 != key2)
                        {
                            set.Add(new Line(centers[key1], centers[key2]));
                        }
                    }
                }
            }

            OutputLines = set.ToList();
        }

        private void FillKeys(List<List<string>> keys, FlatBFS data, int comp, string key)
        {
            foreach (var gen in data.PointList[1][comp])
            {
                foreach (var point in gen)
                {
                    keys[(int)point.X][(int)point.Y] = key;
                }
            }
        }

        private FlatPoint GetCompAvg(FlatBFS data, int comp)
        {
            var res = new FlatPoint(0, 0);
            var count = 0;
            foreach (var gen in data.PointList[1][comp])
            {
                count += gen.Count;
                foreach (var point in gen)
                {
                    res.X += point.X;
                    res.Y += point.Y;
                }
            }
            res.X /= count;
            res.Y /= count;
            return res;
        }

        private void SelectField(FlatBFS data, int comp, int gen)
        {
            data.SetEnabled(false);
            foreach (var point in data.PointList[0][comp][gen])
            {
                data.Points[(int)point.X][(int)point.Y].Enabled = true;
            }
        }

        private void Show(FlatBFS model)
        {
            var colors = new List<Color>
            {
                Color.FromArgb(255, 0, 0),
                Color.FromArgb(200, 0, 0),
                Color.FromArgb(150, 0, 0),
                Color.FromArgb(100, 0, 0),
                Color.FromArgb(50, 0, 0),
            };
            var image = State.CreateImage();
            for (int x = 0; x < State.Width; x++)
            {
                for (int y = 0; y < State.Height; y++)
                {
                    var gen = model.Points[x][y].Generation[0];
                    if (gen != -1)
                    {
                        image.SetPixel(x, y, colors[gen % colors.Count]);
                    }
                }
            }
            Update(image);
        }

        private void button7_Click(object sender, EventArgs e)
        {
            var k = State.Width / CW;
            Buffer = new List<Line>();
            foreach (var line in OutputLines)
            {
                var x1 = (int)((double)line.P1.X / State.Width * CW) * k;
                var y1 = (int)((double)line.P1.Y / State.Height * CW) * k;
                var x2 = (int)((double)line.P2.X / State.Width * CW) * k;
                var y2 = (int)((double)line.P2.Y / State.Height * CW) * k;
                if (x1 == x2 && y1 == y2)
                {
                    continue;
                }
                Buffer.Add(new Line(new FlatPoint(x1, y1), new FlatPoint(x2, y2)));
            }
            OutputLines = Buffer;
            ShowLines();
        }

        private void button6_Click(object sender, EventArgs e)
        {
            var joints = new Dictionary<FlatPoint, HashSet<Line>>();
            foreach (var line in OutputLines)
            {
                if (!joints.ContainsKey(line.P1))
                {
                    joints[line.P1] = new HashSet<Line>();
                }
                if (!joints.ContainsKey(line.P2))
                {
                    joints[line.P2] = new HashSet<Line>();
                }
                joints[line.P1].Add(line);
                joints[line.P2].Add(line);
            }
            var corePoints = new HashSet<FlatPoint>();
            var toDelete = new HashSet<FlatPoint>();
            foreach (var pair in joints)
            {
                var point = pair.Key;
                var count = pair.Value.Count;
                if (count > 2)
                {
                    corePoints.Add(point);
                }
                if (count == 1)
                {
                    toDelete.Add(point);
                }
            }
            foreach (var point in toDelete)
            {
                RemoveLine(joints, joints[point].FirstOrDefault());
            }
            var result = new HashSet<Line>();
            while (joints.Count > 0)
            {
                var buffer = new List<FlatPoint> { joints.FirstOrDefault().Key };
                while (true)
                {
                    var first = buffer.Last();
                    var line = joints[first].FirstOrDefault();
                    var second = GetSecond(line, first);
                    buffer.Add(second);
                    if (corePoints.Contains(second) || joints[second].Count == 1 || IsTooWide(buffer))
                    {
                        if (buffer.Count == 2)
                        {
                            RemoveLine(joints, line);
                        }
                        break;
                    }
                    RemoveLine(joints, line);
                }
                result.Add(new Line(buffer.FirstOrDefault(), buffer.LastOrDefault()));
            }
            OutputLines = result.ToList();
            ShowLines();
        }

        private bool IsTooWide(List<FlatPoint> data)
        {
            var first = data.FirstOrDefault();
            var last = data.LastOrDefault();
            var width = Math.Sqrt(Sqr(first.X - last.X) + Sqr(first.Y - last.Y));
            return GetSquare(data) / width > double.Parse(textBox4.Text);
        }

        private double Sqr(double x)
        {
            return x * x;
        }

        private double GetSquare(List<FlatPoint> data)
        {
            var res = 0.0;
            if (data.Count < 3)
            {
                return 0;
            }
            for (int i = 2; i < data.Count; i++)
            {
                var a = data[i - 2];
                var b = data[i - 1];
                var c = data[0];
                res += ((b.X - a.X) * (c.Y - a.Y) - (b.Y - a.Y) * (c.X - a.X)) / 2;
            }
            return Math.Abs(res);
        }

        private FlatPoint GetSecond(Line line, FlatPoint first)
        {
            return line.P1 == first ? line.P2 : line.P1;
        }

        private void RemoveLine(Dictionary<FlatPoint, HashSet<Line>> joints, Line line)
        {
            var p1 = line.P1;
            var p2 = line.P2;
            joints[p1].Remove(line);
            joints[p2].Remove(line);
            if (joints[p1].Count == 0)
            {
                joints.Remove(p1);
            }
            if (joints[p2].Count == 0)
            {
                joints.Remove(p2);
            }
        }

        private void button8_Click(object sender, EventArgs e)
        {
            var k = State.Width / CW;
            for (int i = 0; i < CW; i++)
            {
                OutputGr.DrawLine(new Pen(Color.Blue), i * k * 2, 0, i * k * 2, State.Height * 2 - 1);
                OutputGr.DrawLine(new Pen(Color.Blue), 0, i * k * 2, State.Width * 2 - 1, i * k * 2);
            }
        }
    }
}
