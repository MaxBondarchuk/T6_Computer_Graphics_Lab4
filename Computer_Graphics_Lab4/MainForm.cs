using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Computer_Graphics_Lab4
{
    public partial class MainForm : Form
    {
        int SizeX = 1500;
        int SizeY = 600;
        //const int halfSizeX = SizeX / 2;
        //const int halfSizeY = SizeY / 2;

        Bitmap bmp;

        readonly Pen _penBlack = new Pen(Color.Black);
        readonly Pen _penFutureTransparent = new Pen(Color.Red);
        readonly Color colorTransparent = Color.Red;
        SolidBrush brushBlack = new SolidBrush(Color.Black);
        SolidBrush brushTransparent = new SolidBrush(Color.Red);

        int rankFinal;

        public class PointsForLine
        {
            public int x1;
            public int y1;
            public int x2;
            public int y2;

            public PointsForLine(int x1, int y1, int x2, int y2, bool _23 = false)
            {
                if (_23)
                {
                    this.x1 = Math.Abs(x2 - x1) / 3 + Math.Min(x1, x2);
                    this.y1 = Math.Max(y1, y2) - Math.Abs(y2 - y1) / 3;
                    this.x2 = Math.Abs(x2 - x1) * 2 / 3 + Math.Min(x1, x2);
                    this.y2 = Math.Max(y1, y2) - Math.Abs(y2 - y1) * 2 / 3;
                }
                else
                {
                    this.x1 = x1;
                    this.y1 = y1;
                    this.x2 = x2;
                    this.y2 = y2;
                }
            }
        }

        public MainForm()
        {
            InitializeComponent();
        }

        // Draws line with DDA algorithm.
        // Used here because doen't need a lot of memory
        void DrawLine(Pen pen, int x1, int y1, int x2, int y2)
        {
            int max = Math.Max(Math.Abs(y2 - y1), Math.Abs(x2 - x1));
            double dx = (x2 - x1) / (double)max;
            double dy = (y2 - y1) / (double)max;

            double x = x1;
            double y = y1;
            for (int i = 0; i < max; i++)
            {
                bmp.SetPixel((int)x, (int)y, pen.Color);
                x += dx;
                y += dy;
            }
        }

        // Builds Koch's fractal
        async void Koch(int x1, int y1, int x2, int y2, int rankCurrent = 0, double rotate = 0)
        {
            // Drawing first horizontal line
            if (rankCurrent == 0)
            {
                DrawLine(_penBlack, x1, y1, x2, y2);
                pbox.Image = bmp;
                await Task.Delay(1);

                Koch(x1, y1, x2, y2, rankCurrent + 1);
                return;
            }

            int dx = x2 - x1;
            int dy = y2 - y1;

            // 13 - 1/3-part
            // 23 - 2/3-part
            int x13 = x1 + dx / 3;
            int y13 = y1 + dy / 3;
            int x23 = x1 + dx * 2 / 3;
            int y23 = y1 + dy * 2 / 3;

            // Middle points
            int mx = x1 + dx / 2;
            int my = y1 + dy / 2;

            // Third triangle point
            double a = Math.Sqrt(Math.Pow(x1 - x2, 2) + Math.Pow(y1 - y2, 2)) / 3.0;
            double h = a * Math.Sqrt(3) / 2.0;
            double xBeforeRotate = mx - mx; // It's vertical first
            double yBeforeRotate = my - h - my;

            // Rotating it to normal position
            int x = (int)(xBeforeRotate * Math.Cos(rotate) - yBeforeRotate * Math.Sin(rotate)) + mx;
            int y = (int)(xBeforeRotate * Math.Sin(rotate) + yBeforeRotate * Math.Cos(rotate)) + my;

            // Removing old part of the line
            DrawLine(_penFutureTransparent, x1, y1, x2, y2);

            // Drawing 4 needed lines
            DrawLine(_penBlack, x1, y1, x13, y13);
            DrawLine(_penBlack, x13, y13, x, y);
            DrawLine(_penBlack, x, y, x23, y23);
            DrawLine(_penBlack, x23, y23, x2, y2);
            bmp.MakeTransparent(colorTransparent);

            // Showing what we built
            pbox.Image = bmp;
            await Task.Delay(1);

            // Build next rank
            if (rankCurrent < rankFinal)
            {
                Koch(x1, y1, x13, y13, rankCurrent + 1, rotate);
                //pbox.Image = bmp;
                //await Task.Delay(500);

                Koch(x13, y13, x, y, rankCurrent + 1, rotate - Math.PI / 3.0);
                //pbox.Image = bmp;
                //await Task.Delay(500);

                Koch(x, y, x23, y23, rankCurrent + 1, rotate + Math.PI / 3.0);
                //pbox.Image = bmp;
                //await Task.Delay(500);

                Koch(x23, y23, x2, y2, rankCurrent + 1, rotate);
                //pbox.Image = bmp;
                //await Task.Delay(500);
            }

        }

        // Builds Serpinski's triangle
        async void Sierpinski(int x1, int y1, int x2, int y2, int x3, int y3, int rankCurrent = 0)
        {
            pbox.Image = bmp;
            await Task.Delay(500);

            // Drawing initial black rectangle
            var g = Graphics.FromImage(bmp);
            if (rankCurrent == 0)
            {
                Point[] points = { new Point(x1, y1), new Point(x2, y2), new Point(x3, y3) };
                g.FillPolygon(brushBlack, points);
                pbox.Image = bmp;
                await Task.Delay(500);

                Sierpinski(x1, y1, x2, y2, x3, y3, rankCurrent + 1);
                return;
            }

            int a = x2 - x1;    // Length of side
            int h = y1 - y3;    // Triangles' height

            // kl - point on triangles' side between points k and l
            int x13 = x1 + a / 4;
            int y13 = y1 - h / 2;
            int x12 = x1 + a / 2;
            int y12 = y1;
            int x23 = x3 + a / 4;
            int y23 = y13;

            // Making transparent new triangle
            Point[] _points = { new Point(x13, y13), new Point(x12, y12), new Point(x23, y23) };
            g.FillPolygon(brushTransparent, _points);
            bmp.MakeTransparent(colorTransparent);
            pbox.Image = bmp;
            await Task.Delay(500);

            // Building triangles with higher rank
            if (rankCurrent < rankFinal)
            {
                Sierpinski(x1, y1, x12, y12, x13, y13, rankCurrent + 1);
                pbox.Image = bmp;
                await Task.Delay(500);

                Sierpinski(x12, y12, x2, y2, x23, y23, rankCurrent + 1);
                pbox.Image = bmp;
                await Task.Delay(500);

                Sierpinski(x13, y13, x23, y23, x3, y3, rankCurrent + 1);
                pbox.Image = bmp;
                await Task.Delay(500);
            }
        }

        // Builds Mandelbrot's set
        async void Mandelbrot()
        {
            SizeX = 1700;
            SizeY = SizeX;
            bmp = new Bitmap(SizeX, SizeY);
            //Graphics g = Graphics.FromImage(bmp);
            //g.FillRectangle(new SolidBrush(Color.Black), 0, 0, SizeX, SizeY);

            // Precision of building
            int depth = 100;
            
            // Going through square with side equal to SizeY
            for (int i = 0; i < SizeX; i++)
            {
                for (int j = 0; j < SizeY; j++)
                {
                    // Checking current point for belongin to the Mandelbrot's set
                    // C = x + iy
                    double x = (i - SizeX / 2.0) / (double)SizeX *4 ;
                    double y = (j - SizeY / 2.0) / (double)SizeX * 4; 
                    
                    double zRe = x, zIm = y;
                    bool belongToSet = true;
                    Random rnd = new Random();
                    Color currColor = Color.FromArgb(rnd.Next(255), rnd.Next(255), rnd.Next(255));
                    for (int k = 0; k < depth; k++)
                    {
                        // Calculating Zn = Zn-1^2 + C
                        double buf = zRe * zRe - zIm * zIm + x;
                        zIm = 2 * zRe * zIm + y;
                        zRe = buf;

                        // Wikipedia tells us that points from Mandelbrot's set isn't greater than 2
                        // If 2<|Zn| then this point (i, j) doesn't belong to the set
                        if (2 < zRe * zRe + zIm * zIm)
                        {
                            belongToSet = false;
                            break;
                        }

                        //bmp.SetPixel((int)(zRe * SizeX / 4.0) + SizeX / 2, (int)(zIm * SizeY / 4.0) + SizeY / 2, currColor);
                        //pbox.Image = bmp;
                        //await Task.Delay(1);
                    }


                    if (belongToSet)
                    {
                        zRe = x;
                        zIm = y;
                        for (int k = 0; k < depth; k++)
                        {
                            // Calculating Zn = Zn-1^2 + C
                            double buf = zRe * zRe - zIm * zIm + x;
                            zIm = 2 * zRe * zIm + y;
                            zRe = buf;

                            bmp.SetPixel((int)(zRe * SizeX / 4.0) + SizeX / 2, (int)(zIm * SizeY / 4.0) + SizeY / 2, currColor);
                            //pbox.Image = bmp;
                            //await Task.Delay(1);
                        }
                    }

                    //if (!belongToSet)
                    //    bmp.SetPixel(i, j, Color.Black);

                    
                }
                pbox.Image = bmp;
                await Task.Delay(1);
            }
        }

        // Call need function for fractal
        private void bBuild_Click(object sender, EventArgs e)
        {
            bClear.PerformClick();

            rankFinal = Convert.ToInt32(rank.Value);
            if (rbKoch.Checked)
                Koch(0, SizeY - 1, SizeX - 1, SizeY - 1);
            else
                if (rbMandelbrot.Checked)
                    Mandelbrot();
                else
                    Sierpinski((int)(SizeX / 2 - SizeY / Math.Sqrt(3)), SizeY - 1, (int)(SizeX / 2 + SizeY / Math.Sqrt(3)), SizeY - 1, SizeX / 2, 0);

            pbox.Image = bmp;
        }

        // Computer Graphics finishes here
        private void bClear_Click(object sender, EventArgs e)
        {
            bmp = new Bitmap(SizeX, SizeY);
            pbox.Image = bmp;
        }

        private void bSave_Click(object sender, EventArgs e)
        {
            if (saveFile.ShowDialog() == DialogResult.OK)
                bmp.Save(saveFile.FileName);
        }

        private void rbKoch_CheckedChanged(object sender, EventArgs e)
        {
            rank.Enabled = true;
            lRank.Enabled = true;
        }

        private void rbMandelbrot_CheckedChanged(object sender, EventArgs e)
        {
            rank.Enabled = false;
            lRank.Enabled = false;
        }

        private void rank_ValueChanged(object sender, EventArgs e)
        {
            
        }
    }
}