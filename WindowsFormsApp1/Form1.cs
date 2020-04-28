using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WindowsFormsApp1
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        public static bool CompareBitmapsFast(Bitmap bmp1, Bitmap bmp2)
        {
            if (bmp1 == null || bmp2 == null)
                return false;
            if (object.Equals(bmp1, bmp2))
                return true;
            if (!bmp1.Size.Equals(bmp2.Size) || !bmp1.PixelFormat.Equals(bmp2.PixelFormat))
                return false;

            int bytes = bmp1.Width * bmp1.Height * (Image.GetPixelFormatSize(bmp1.PixelFormat) / 8);

            bool result = true;
            byte[] b1bytes = new byte[bytes];
            byte[] b2bytes = new byte[bytes];

            BitmapData bitmapData1 = bmp1.LockBits(new Rectangle(0, 0, bmp1.Width, bmp1.Height), ImageLockMode.ReadOnly, bmp1.PixelFormat);
            BitmapData bitmapData2 = bmp2.LockBits(new Rectangle(0, 0, bmp2.Width, bmp2.Height), ImageLockMode.ReadOnly, bmp2.PixelFormat);

            Marshal.Copy(bitmapData1.Scan0, b1bytes, 0, bytes);
            Marshal.Copy(bitmapData2.Scan0, b2bytes, 0, bytes);

            for (int n = 0; n <= bytes - 1; n++)
            {
                if (b1bytes[n] != b2bytes[n])
                {
                    result = false;
                    break;
                }
            }

            bmp1.UnlockBits(bitmapData1);
            bmp2.UnlockBits(bitmapData2);

            return result;
        }
        void ProcessImage()
        {
            bool locked;
            string txtpath = @"D:\Random Pics\mapping.txt";
            StreamWriter fi = File.CreateText(txtpath);
            Bitmap map = new Bitmap(textBox1.Text, true);
            Bitmap piece;
            List<Bitmap> list = new List<Bitmap>();
            Bitmap spritesmap;
            for (int i = 0; i < map.Height; i += 64)
            {
                for (int j = 0; j < map.Width; j += 64)
                {
                    locked = true;
                    piece = map.Clone(new System.Drawing.Rectangle(j, i, 64, 64), map.PixelFormat);
                    foreach (Bitmap b in list)
                    {
                        if (CompareBitmapsFast(piece,b))
                        {
                            fi.Write(list.IndexOf(b));
                            locked = false;
                            break;
                        }
                    }
                    if (locked)
                    {
                        fi.Write(list.Count);
                        list.Add(piece);
                    }
                    fi.Write(" ");
                }
                fi.WriteLine();
            }
            int width = 6;
            int m = 0, n = 0;
            int height = list.Count / width;
            if (list.Count % width > 0) height++;
            spritesmap = new Bitmap(width*64, height*64);
            using (Graphics g = Graphics.FromImage(spritesmap))
            {
                foreach (Bitmap b in list)
                {
                    g.DrawImage(b, new System.Drawing.Rectangle(n*64, m*64, 64, 64));
                    n++;
                    if (n >= width)
                    {
                        n = 0;
                        m++;
                    }
                }
            }
            spritesmap.Save(@"D:\Random Pics\results.png", ImageFormat.Png);
            fi.Close();
        }
        private void button1_Click(object sender, EventArgs e)
        {
            ProcessImage();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            openFileDialog1.ShowDialog();
        }

        private void openFileDialog1_FileOk(object sender, CancelEventArgs e)
        {
            textBox1.Text = openFileDialog1.FileName;
        }

        private void textBox2_TextChanged(object sender, EventArgs e)
        {

        }
    }
}
