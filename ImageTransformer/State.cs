using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImageTransformer
{
    public class State
    {
        public static int Width { get; set; }
        public static int Height { get; set; }
        public static Bitmap Original { get; set; }
        public static Bitmap Buffer { get; set; }

        public static Bitmap CreateImage()
        {
            return CreateImage(Width, Height);
        }

        public static Bitmap CreateImage(int width, int height)
        {
            var res = new Bitmap(width, height);
            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    res.SetPixel(x, y, Color.White);
                }
            }

            return res;
        }
    }
}
