using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.Runtime.InteropServices;

namespace ALA_ImageReduction
{
    internal class Program
    {
        static void Main(string[] args)
        {
            //C:\Users\Lenidar\OneDrive\Pictures\Laspona_College.jpg
            string imagePath = "";
            Bitmap bmp = null;
            Bitmap bmpReduce = null;
            float aspectRatio = 0f;
            float scale = 0f;
            string uInput = "";
            int temp = 0;
            float targetX = 0f;
            float targetY = 0f;
            float scaleX = 0f;
            float scaleY = 0f;

            int[,,] pixelMap = null;
            int[,,] pixelMapReduce = null;

            Console.Write("Please input path to image: ");
            imagePath = Console.ReadLine();

            bmp = new Bitmap(imagePath);
            pixelMap = new int[bmp.Height, bmp.Width, 3];

            // maps individual pixels to editable int array
            for (int y = 0; y < pixelMap.GetLength(0); y++)
            {
                for (int x = 0; x < pixelMap.GetLength(1); x++)
                {
                    Color pixel = bmp.GetPixel(x, y);
                    pixelMap[y, x, 0] = pixel.R;
                    pixelMap[y, x, 1] = pixel.G;
                    pixelMap[y, x, 2] = pixel.B;
                }
            }

            while (true)
            {
                Console.Write("Please input the percentage of how the image would be scaled down?\n" +
                    "10-100. Pleasen note that this is in %: ");
                uInput = Console.ReadLine();
                if (int.TryParse(uInput, out temp))
                    if (temp >= 10 && temp <= 100)
                        break;
            }

            scale = (float)temp / 100;
            aspectRatio = (float)bmp.Width / (float)bmp.Height;

            bmpReduce = new Bitmap((int)Math.Floor((decimal)bmp.Width * (decimal)scale), (int)Math.Floor((decimal)bmp.Height * (decimal)scale));
            pixelMapReduce = new int[(int)Math.Floor((decimal)bmp.Width * (decimal)scale), (int)Math.Floor((decimal)bmp.Height * (decimal)scale), 3];


            Console.WriteLine($"Original image size is : {bmp.Width} x {bmp.Height}");
            Console.WriteLine($"Aspect ratio of original image is : {aspectRatio}");
            Console.WriteLine($"New image size is : {bmpReduce.Width} x {bmpReduce.Height}");

            // Bilinear Interpolation

            scaleX = ((float)bmp.Width / (float)bmpReduce.Width);
            scaleY = ((float)bmp.Height / (float)bmpReduce.Height);

            for (int x = 0; x < pixelMapReduce.GetLength(0); x++)
            {
                for (int y = 0; y < pixelMapReduce.GetLength(1); y++)
                {
                    // calculate original pixel counterpart
                    targetX = (float)x * scaleX;
                    targetY = (float)y * scaleY;

                    for (int c = 0; c < pixelMapReduce.GetLength(2); c++)
                    {
                        // 4 neightbor pixels coordinates
                        int x0 = (int)targetX;
                        int y0 = (int)targetY;

                        // this makes sure it doesnt go outside the original pixels
                        int x1 = Math.Min(x0 + 1, bmp.Width - 1);
                        int y1 = Math.Min(y0 + 1, bmp.Height - 1);

                        // weights
                        float dx = targetX - x0;
                        float dy = targetY - y0;

                        // 4 neightbor pixels value
                        int TL = pixelMap[y0, x0, c];
                        int TR = pixelMap[y1, x0, c];
                        int BL = pixelMap[y0, x1, c];
                        int BR = pixelMap[y1, x1, c];

                        // Horizontal blend
                        float top = TL * (1 - dx) + TR * dx;
                        float bottom = BL * (1 - dx) + BR * dx;
                        // Vertical blend
                        pixelMapReduce[x, y, c] = (byte)(top * (1 - dy) + bottom * dy);
                    }

                    bmpReduce.SetPixel(x, y, Color.FromArgb(pixelMapReduce[x, y, 0], pixelMapReduce[x, y, 1], pixelMapReduce[x, y, 2]));
                }
            }

            bmpReduce.Save($"Reduce_{scale * 100}.jpg");
            Console.ReadKey();
        }
    }
}
