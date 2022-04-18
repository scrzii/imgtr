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

            panel1.Width = image.Width;
            panel1.Height = image.Height;
            panel2.Width = image.Width;
            panel2.Height = image.Height;

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
                        image.SetPixel(x, y, State.Buffer.GetPixel(x, y));
                        image.SetPixel(x + 1, y, State.Buffer.GetPixel(x + 1, y));
                    }
                    if (State.Buffer.GetPixel(x, y) != State.Buffer.GetPixel(x, y + 1))
                    {
                        image.SetPixel(x, y, State.Buffer.GetPixel(x, y));
                        image.SetPixel(x, y + 1, State.Buffer.GetPixel(x, y + 1));
                    }
                }
            }
            Update(image);
        }

        private void button3_Click(object sender, EventArgs e)
        {
            var image = State.CreateImage();
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
                    if (sim >= int.Parse(textBox3.Text))
                    {
                        image.SetPixel(x, y, color);
                        continue;
                    }
                    image.SetPixel(x, y, colorDict.OrderByDescending(_ => _.Value).First().Key);
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
    }
}
