using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
using System.Windows.Forms;
using GKLab4.Utils;

namespace GKLab4
{
    public partial class Form1 : Form
    {
        double[,] ModelMatrix;
        double[,] ViewMatrix;
        double[,] ProjMatrix;
        double a;
        double e;
        double fov = Math.PI / 180 * 35;
        double n, f;
        double[][] Points;
        List<List<int>> lines;
        Point[] mappedPoints;
        int angle = 0;

        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs args)
        {
            n = 1;
            f = 100;

            e = 1 / Math.Tan(fov / 2);
            a = (double)this.pictureBox1.Height / this.pictureBox1.Width;
            double[,] _ModelMatrix = { { 1, 0, 0, 0 }, { 0, 1, 0, 0 }, { 0, 0, 1, 0 }, { 0, 0, 0, 1 } };
            double[,] _ViewMatrix = { { 0, 1, 0, -0.5 }, { 0, 0, 1, -0.5 }, { 1, 0, 0, -3 }, { 0, 0, 0, 1 } };
            double[,] _ProjMatrix = { { e, 0, 0, 0 }, { 0, e / a, 0, 0 }, { 0, 0, -(f + n) / (f - n), -2 * f * n / (f - n) }, { 0, 0, -1, 0 } };
            double[][] points = {new double[] { 0, 0, 0, 1 }, new double[] {0,1,0,1}, new double[] {1,0,0,1}, new double[] {0,0,1,1} };

            ModelMatrix = _ModelMatrix;
            ViewMatrix = _ViewMatrix;
            ProjMatrix = _ProjMatrix;

            mappedPoints = new Point[4];

            Points = points;

            lines = new List<List<int>>();
            ConnectPoints();

            for(int i = 0; i < 4; i++)
            {
                double[] projPoint = ProjectPoint(Points[i]);
                mappedPoints[i] = ConvertPicturebox(projPoint);
            }

            timer1 = new System.Windows.Forms.Timer();
            timer1.Interval = 50;
            timer1.Tick += new EventHandler(OnTimedEvent);
            timer1.Start();
        }

        private void OnTimedEvent(Object myObject, EventArgs myEventArgs)
        {
            double[,] _ModelMatrix = { { Math.Cos(Math.PI * angle / 180.0), -1* Math.Sin(Math.PI * angle / 180.0), 0, 0.1 }, 
                { Math.Sin(Math.PI * angle / 180.0), Math.Cos(Math.PI * angle / 180.0), 0, 0.2 }, { 0, 0, 1, 0.3 }, { 0, 0, 0, 1 } };
            ModelMatrix = _ModelMatrix;
            angle += 1;

            for (int i = 0; i < 4; i++)
            {
                double[] projPoint = ProjectPoint(Points[i]);
                mappedPoints[i] = ConvertPicturebox(projPoint);
            }

            pictureBox1.Refresh();
        }

        private double[] ProjectPoint(double[] point)
        {
            double[] resPoint = Utilities.Multiply(ProjMatrix, Utilities.Multiply(ViewMatrix, Utilities.Multiply(ModelMatrix, point)));
            return resPoint.Select(x => x / resPoint[3]).ToArray();
        }

        private Point ConvertPicturebox(double[] point)
        {
            int x = (int)Math.Round(pictureBox1.Width/2 + pictureBox1.Width / 2 * (point[0]));
            int y = (int)Math.Round(pictureBox1.Height/2 - pictureBox1.Height / 2 * (point[1]));
            return new Point(x, y);
        }

        private void pictureBox1_Paint(object sender, PaintEventArgs e)
        {
            for (int i = 0; i < mappedPoints.Length; ++i)
            {
                e.Graphics.FillRectangle(Brushes.Red, mappedPoints[i].X, mappedPoints[i].Y, 4, 4);
            }

            foreach(var line in lines)
                e.Graphics.DrawLine(Pens.Black, mappedPoints[line[0]], mappedPoints[line[1]]);
        }

        private void ConnectPoints()
        {
            List<int> line1 = new List<int>();
            line1.Add(0);
            line1.Add(1);
            lines.Add(line1);

            List<int> line2 = new List<int>();
            line2.Add(0);
            line2.Add(2);
            lines.Add(line2);

            List<int> line3 = new List<int>();
            line3.Add(0);
            line3.Add(3);
            lines.Add(line3);

            List<int> line4 = new List<int>();
            line4.Add(1);
            line4.Add(2);
            lines.Add(line4);

            List<int> line5 = new List<int>();
            line5.Add(1);
            line5.Add(3);
            lines.Add(line5);

            List<int> line6 = new List<int>();
            line6.Add(2);
            line6.Add(3);
            lines.Add(line6);

            return;
        }
    }
}
