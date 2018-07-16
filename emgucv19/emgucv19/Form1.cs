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
                pictureBox1.Image = m.Bitmap;
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
            while (isReadingFrame==true && FrameNo<TotalFrame)
            {
                FrameNo += Convert.ToInt16(numericUpDown1.Value);
                capture.SetCaptureProperty(Emgu.CV.CvEnum.CapProp.PosFrames, FrameNo);
                capture.Read(m);
                pictureBox1.Image = m.Bitmap;
                await Task.Delay(1000 /Convert.ToInt16( Fps));
                label1.Text = FrameNo.ToString() + "/" + TotalFrame.ToString();
      
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            isReadingFrame = false;
        }
    }
}
