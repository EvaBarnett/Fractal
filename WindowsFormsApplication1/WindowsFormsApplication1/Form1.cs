using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WindowsFormsApplication1
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
            bp = new Bitmap(this.pictureBox1.Height, this.pictureBox1.Width);
            g1 = Graphics.FromImage(bp);
            pictureBox1.Image = bp;
            init();
            start();
            Refresh();
        }

        private const int MAX = 256;
        private const double SX = -2.025; // start value real
        private const double SY = -1.125; // start value imaginary
        private const double EX = 0.6;    // end value real
        private const double EY = 1.125;  // end value imaginary
        private static int x1, y1, xs, ys, xe, ye;
        private static double xstart, ystart, xende, yende, xzoom, yzoom;
        private static bool action, rectangle, finished;
        private static float xy;
        private Image bp;
        private Graphics g1;
        //private Cursor c1, c2;  


        public void init() // all instances will be prepared    
        {
            //HSBcol = new HSB();        
            // ---->Size done in properties of pictureBox1
            // also add FixedSingle as BorderStyle
            // ---->Set size of form in designer accordingly
            finished = false;
            x1 = this.pictureBox1.Width;  //---->added    pictureBox1   
            y1 = this.pictureBox1.Height; //---->added    pictureBox1        
            xy = (float)x1 / (float)y1;
            //?JAVA? picture = createImage(x1, y1);       
            //?JAVA? g1 = picture.getGraphics();        
            finished = true;



        }

        public void destroy() // delete all instances 
        {
            if (finished)
            {
                g1 = null;
                bp = null;
                g1 = null;
                GC.Collect(); // garbage collection
            }
        }
        public void stop()
        {
        }

        public void paint(Graphics g)
        {
            update(g);
        }

        public void update(Graphics g)
        {
            g.DrawImage(bp, 0, 0);
            if (rectangle)
            {
                Color color = Color.White;
                Pen mypen = new Pen(color);
                if (xs < xe)
                {
                    if (ys < ye) g.DrawRectangle(mypen, xs, ys, (xe - xs), (ye - ys));
                    else g.DrawRectangle(mypen, xs, ye, (xe - xs), (ys - ye));
                }
                else
                {
                    if (ys < ye) g.DrawRectangle(mypen, xe, ys, (xs - xe), (ye - ys));
                    else g.DrawRectangle(mypen, xe, ye, (xs - xe), (ys - ye));
                }
            }
        }

        private void mandelbrot() // calculate all points
        {
            int x, y;
            float h, b, alt = 0.0f;

            action = false;
            ///setCursor(c1);
            ///showStatus("Mandelbrot-Set will be produced - please wait...");
            for (x = 0; x < x1; x += 2)
                for (y = 0; y < y1; y++)
                {
                    h = pointcolour(xstart + xzoom * (double)x, ystart + yzoom * (double)y); // color value
                    if (h != alt)
                    {
                        b = 1.0f - h * h; // brightness
                        Color color = HSBColor.FromHSB(new HSBColor(h * 255, 0.8f * 255, b * 255));
                        Pen pen = new Pen(color);
                        g1.DrawLine(pen, x, y, x + 1, y);
                        /// alt = h;
                    }
                }

            ///showStatus("Mandelbrot-Set ready - please select zoom area with pressed mouse.");
            ///setCursor(c2);
            action = true;
        }

        public void start()
        {
            action = false;
            rectangle = false;
            initvalues();
            xzoom = (xende - xstart) / (double)x1;
            yzoom = (yende - ystart) / (double)y1;
            mandelbrot();
        }

        private float pointcolour(double xwert, double ywert) // color value from 0.0 to 1.0 by iterations
        {
            double r = 0.0, i = 0.0, m = 0.0;
            int j = 0;

            while ((j < MAX) && (m < 4.0))
            {
                j++;
                m = r * r - i * i;
                i = 2.0 * r * i + ywert;
                r = m + xwert;
            }
            return (float)j / (float)MAX;
        }

        private void initvalues() // reset start values
        {
            xstart = SX;
            ystart = SY;
            xende = EX;
            yende = EY;
            if ((float)((xende - xstart) / (yende - ystart)) != xy)
                xstart = xende - (yende - ystart) * (double)xy;
        }

        /// <summary>
        /// djm added, Java can work with the HSB/V colour system
        /// c# can only do RGB
        /// so I searched the internet for some code to convert
        /// see http://www.codeproject.com/dotnet/HSBColorClass.asp
        /// note I have removed some code from the downloaded class that isn't needed, just to make it clearer
        /// </summary>
        public struct HSBColor
        {
            float h;
            float s;
            float b;
            int a;

            public HSBColor(float h, float s, float b)
            {
                this.a = 0xff;
                this.h = Math.Min(Math.Max(h, 0), 255);
                this.s = Math.Min(Math.Max(s, 0), 255);
                this.b = Math.Min(Math.Max(b, 0), 255);
            }

            public HSBColor(int a, float h, float s, float b)
            {
                this.a = a;
                this.h = Math.Min(Math.Max(h, 0), 255);
                this.s = Math.Min(Math.Max(s, 0), 255);
                this.b = Math.Min(Math.Max(b, 0), 255);
            }

            public float H
            {
                get { return h; }
            }

            public float S
            {
                get { return s; }
            }

            public float B
            {
                get { return b; }
            }

            public int A
            {
                get { return a; }
            }

            public Color Color
            {
                get
                {
                    return FromHSB(this);
                }
            }

            public static Color FromHSB(HSBColor hsbColor)
            {
                float r = hsbColor.b;
                float g = hsbColor.b;
                float b = hsbColor.b;
                if (hsbColor.s != 0)
                {
                    float max = hsbColor.b;
                    float dif = hsbColor.b * hsbColor.s / 255f;
                    float min = hsbColor.b - dif;

                    float h = hsbColor.h * 360f / 255f;

                    if (h < 60f)
                    {
                        r = max;
                        g = h * dif / 60f + min;
                        b = min;
                    }
                    else if (h < 120f)
                    {
                        r = -(h - 120f) * dif / 60f + min;
                        g = max;
                        b = min;
                    }
                    else if (h < 180f)
                    {
                        r = min;
                        g = max;
                        b = (h - 120f) * dif / 60f + min;
                    }
                    else if (h < 240f)
                    {
                        r = min;
                        g = -(h - 240f) * dif / 60f + min;
                        b = max;
                    }
                    else if (h < 300f)
                    {
                        r = (h - 240f) * dif / 60f + min;
                        g = min;
                        b = max;
                    }
                    else if (h <= 360f)
                    {
                        r = max;
                        g = min;
                        b = -(h - 360f) * dif / 60 + min;
                    }
                    else
                    {
                        r = 0;
                        g = 0;
                        b = 0;
                    }
                }

                return Color.FromArgb
                    (
                        hsbColor.a,
                        (int)Math.Round(Math.Min(Math.Max(r, 0), 255)),
                        (int)Math.Round(Math.Min(Math.Max(g, 0), 255)),
                        (int)Math.Round(Math.Min(Math.Max(b, 0), 255))
                        );
            }

        }

        private void pictureBox1_Click(object sender, EventArgs e)
        {

        }

        private void pictureBox1_MouseUp(object sender, MouseEventArgs e)
        {

        }

        private void pictureBox1_MouseMove(object sender, MouseEventArgs e)
        {

        }
    }
}
