using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Drawing.Drawing2D;
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
            //Cursor variables
            c1 = Cursors.WaitCursor;
            c2 = Cursors.Cross;

            //This saves the set heights of the generated picture boxes
            bp = new Bitmap(this.pictureBox1.Height, this.pictureBox1.Width);
            g1 = Graphics.FromImage(bp);
            area = new Bitmap(this.pictureBox1.Height, this.pictureBox1.Width);
            g2 = Graphics.FromImage(area);
            pictureBox1.Image = bp;


            init();
            start();
            Refresh();
        }

        private Cursor c1; //sets c1 to the Cursor variable 
        private Cursor c2; //sets c2 to the Cursor variable 
        private const int MAX = 256;      // max iterations
        private const double SX = -2.025; // start value real
        private const double SY = -1.125; // start value imaginary
        private const double EX = 0.6;    // end value real
        private const double EY = 1.125;  // end value imaginary
        private static int x1, y1, xs, ys, xe, ye;
        private static double xstart, ystart, xende, yende, xzoom, yzoom;
        private static bool action, rectangle, finished;
        private static float xy;
        private Image bp, area;
        private Graphics g1, g2;



        //private HSB HSBcol=new HSB();


        public void init() // all instances will be prepared
        {
            //HSBcol = new HSB();
            //this.Size = new Size(640, 480);
            finished = false;
            x1 = this.pictureBox1.Width;  //---->added    pictureBox1   
            y1 = this.pictureBox1.Height; //---->added    pictureBox1   
            xy = (float)x1 / (float)y1;
            //?JAVA? picture = createImage(x1, y1);
            //?JAVA? g1 = picture.getGraphics();
            finished = true;
            //mandelbrot();
        }

        public void destroy() // delete all instances 
        {
            if (finished)
            {
                //if program finishes, set below values to null
                g1 = null;
                bp = null;
                g1 = null;
                GC.Collect(); // garbage collection
            }
        }


        public void update()
        {
            //clears the colour and implements a transparent background
            g2.Clear(Color.Transparent);
          
            Graphics g = this.CreateGraphics();
            //if the drawing is a rectangle then excecute the below code
            if (rectangle)
            {
                //set a default colour for the pen
                Color color = Color.White;
                //instantiate the pen function
                Pen mypen = new Pen(color);
                if (xs < xe)
                {
                    if (ys < ye) g2.DrawRectangle(mypen, xs, ys, (xe - xs), (ye - ys));
                    else g2.DrawRectangle(mypen, xs, ye, (xe - xs), (ys - ye));
                }
                else
                {
                    if (ys < ye) g2.DrawRectangle(mypen, xe, ys, (xs - xe), (ye - ys));
                    else g2.DrawRectangle(mypen, xe, ye, (xs - xe), (ys - ye));
                }
            }
        }

        //picture box function
        private void pictureBox1_Paint(object sender, PaintEventArgs e)
        {
            //this draws the background and overlay using the specified co-ordinates
            e.Graphics.DrawImageUnscaled(bp, 0, 0);
            e.Graphics.DrawImageUnscaled(area, 0, 0);

        }

        private void mandelbrot() // calculate all points
        {
            int x, y;
            float h, b, alt = 0.0f;

            action = false;
            c1 = Cursor.Current;
            //showStatus("Mandelbrot-Set will be produced - please wait...");
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
            c2 = Cursor.Current;
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

        private void pictureBox1_MouseDown_1(object sender, MouseEventArgs e)
        {
            //e.Consume();
            if (action)
            {
                xs = e.X;
                ys = e.Y;
            }
        }

        private void pictureBox1_Click(object sender, EventArgs e)
        {

        }

        private void Form1_MouseUp(object sender, MouseEventArgs e)
        {

            int z, w;
            if (e.Button == MouseButtons.Left)
            {
                //e.consume();
                if (action)
                {
                    xe = e.X;
                    ye = e.Y;
                    if (xs > xe)
                    {
                        z = xs;
                        xs = xe;
                        xe = z;
                    }
                    if (ys > ye)
                    {
                        z = ys;
                        ys = ye;
                        ye = z;
                    }
                    w = (xe - xs);
                    z = (ye - ys);
                    if ((w < 2) && (z < 2)) initvalues();
                    else
                    {
                        if (((float)w > (float)z * xy)) ye = (int)((float)ys + (float)w / xy);
                        else xe = (int)((float)xs + (float)z * xy);
                        xende = xstart + xzoom * (double)xe;
                        yende = ystart + yzoom * (double)ye;
                        xstart += xzoom * (double)xs;
                        ystart += yzoom * (double)ys;
                    }
                    xzoom = (xende - xstart) / (double)x1;
                    yzoom = (yende - ystart) / (double)y1;
                    mandelbrot();
                    rectangle = false;
                    Refresh();

                }
            }
        }



        private void pictureBox1_MouseMove(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                if (action)
                {
                    xe = e.X;
                    ye = e.Y;
                    rectangle = true;
                    update();
                    Refresh();
                }
            }
            g2.Clear(Color.Transparent);
        }


        private void Form1_Load(object sender, System.EventArgs e)
        {

        }





        }




}