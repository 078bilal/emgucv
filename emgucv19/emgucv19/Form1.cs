using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Emgu.CV;
using Emgu.CV.Structure;
using Emgu.CV.UI;
using Emgu.CV.CvEnum;
using Emgu.CV.Util;

namespace emgucv19
{
    public partial class Form1 : Form
    {
        double Fps;
        double TotalFrame;
        int FrameNo;
        VideoCapture capture;
        bool isReadingFrame;
        public Form1()
        {
            InitializeComponent();
        }
        public Point anchor { get; private set; }
        public MCvScalar borderValue { get; private set; }
        public BorderType borderType { get; private set; }
        public IOutputArray hierarchy { get; private set; }
        public object parent { get; private set; }
        OpenFileDialog op = new OpenFileDialog();
        UMat gimage = new UMat();
        UMat bimage = new UMat();
        UMat timage = new UMat();
        UMat etimage = new UMat();
        UMat dtimage = new UMat();
        double egim = 1;
        float[,] centerP = new float[3, 3];
        UMat img = new UMat();
        VectorOfVectorOfPoint cnst = new VectorOfVectorOfPoint();
       // Image<Bgr, Byte> img;
        double th = 200.0;
        double thmax = 250.0;
        private void splitContainer1_Panel1_Paint(object sender, PaintEventArgs e)
        {

        }

        private void openToolStripMenuItem_Click(object sender, EventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog();
            if (ofd.ShowDialog() == DialogResult.OK)
            {
                capture = new VideoCapture(ofd.FileName);
                Mat m = new Mat();
                capture.Read(m);
               // pictureBox1.Image = m.Bitmap;
                TotalFrame = capture.GetCaptureProperty(Emgu.CV.CvEnum.CapProp.FrameCount);
                Fps = capture.GetCaptureProperty(Emgu.CV.CvEnum.CapProp.Fps);
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (capture==null)
            {
                return;
            }
            isReadingFrame = true;
            ReadAllFrames();
           
        }
        private async void ReadAllFrames()
        {
            Mat m = new Mat();
            while (isReadingFrame==true && FrameNo<TotalFrame-1)
            {
                FrameNo += Convert.ToInt16(numericUpDown1.Value);
                capture.SetCaptureProperty(Emgu.CV.CvEnum.CapProp.PosFrames, FrameNo);
                capture.Read(m);
                CvInvoke.CvtColor(m, gimage, ColorConversion.Bgr2Gray);
                CvInvoke.GaussianBlur(gimage, bimage, new Size(7,7), 3, 0);
                CvInvoke.Threshold(bimage, timage, th, thmax, ThresholdType.Binary);
                CvInvoke.Erode(timage, etimage, null, anchor, 2, borderType, borderValue);
                CvInvoke.Dilate(etimage, dtimage, null, anchor, 4, borderType, borderValue);
                CvInvoke.FindContours(etimage, cnst, hierarchy, Emgu.CV.CvEnum.RetrType.External, Emgu.CV.CvEnum.ChainApproxMethod.ChainApproxSimple);
                Dictionary<int, double> dict = new Dictionary<int, double>();
                for (int i = 0; i < cnst.Size; i++)
                {
                    double area = CvInvoke.ContourArea(cnst[i]);
                    Rectangle rect = CvInvoke.BoundingRectangle(cnst[i]);
                    CircleF crc = CvInvoke.MinEnclosingCircle(cnst[i]);
                    dict.Add(i, area);
                }
                 
                foreach (var item in dict)
                {
                    int key = int.Parse(item.Key.ToString());
                    Rectangle rectt = CvInvoke.BoundingRectangle(cnst[key]);
                    CvInvoke.Rectangle(m, rectt, new MCvScalar(0, 0, 255), 2);
                    CvInvoke.PutText(m, key.ToString(), new Point(rectt.X, rectt.Y),0 , 0.45, new MCvScalar(255, 255, 255), 2);
                    /*******AÇI BULDURUP DOĞRULARI ÇİZDİRMEEEEEEEEEEEEEE*******/
                    centerP[key, 0] = rectt.X + (rectt.Width / 2);
                    centerP[key, 1] = rectt.Y + (rectt.Height / 2);
                    //CircleF ccf = CvInvoke.MinEnclosingCircle(cnst[key]);
                    //Point cnter = new Point((int)ccf.Center.X,(int)ccf.Center.Y);
                    //CvInvoke.Circle(img, cnter, (int)ccf.Radius, new MCvScalar(0, 0, 255),3);
                    if (key > 0)
                    {
                        float y1, x1;
                        y1 = centerP[key, 1] - centerP[key - 1, 1];
                        x1 = centerP[key, 0] - centerP[key - 1, 0];
                        CvInvoke.Line(m, new Point((int)centerP[key, 0], (int)centerP[key, 1]), new Point((int)centerP[key - 1, 0], (int)centerP[key - 1, 1]), new MCvScalar(0, 255, 0));
                        centerP[key, 2] = y1 / x1;
                        egim = centerP[key-1, 2] - centerP[key, 2] / ((centerP[key, 2] * centerP[key - 1, 2]) + 1);
                       
                    }
                    Console.WriteLine(Math.Atan(egim) * (180 / Math.PI));
                }
                pictureBox1.Image = m.Bitmap;
                await Task.Delay(1000 /Convert.ToInt16( Fps));
                label1.Text = FrameNo.ToString() + "/" + TotalFrame.ToString();
      
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            isReadingFrame = false;
        }

        public FontFace fontFace { get; set; }

        public double fontScale { get; set; }
    }
}
