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
            var res = new Bitmap(Width, Height);
            for (int x = 0; x < Width; x++)
            {
                for (int y = 0; y < Height; y++)
                {
                    res.SetPixel(x, y, Color.White);
                }
            }

            return res;
        }
    }
}
