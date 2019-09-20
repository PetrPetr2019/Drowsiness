using System;
using System.Drawing;
using System.Windows.Forms;
using Emgu.CV;
using Emgu.CV.Structure;
using Emgu.CV.CvEnum;
using Tulpep.NotificationWindow;
using System.Collections.Generic;
using Drowsiness.Properties;

namespace Drowsiness
{
    public partial class Form1 : Form
    {
//        private Timer timer = new Timer();
//        private Capture camera;
        private int _count;
        private int _i;
        private Image<Bgr, byte> _currentFrame;
        private VideoCapture _camera;
        private readonly CascadeClassifier _eye;
        private readonly CascadeClassifier _faces;

       public Form1()
       {
           _eye = new CascadeClassifier("haarcascade_eye.xml");
           _faces = new CascadeClassifier("haarcascade_frontalface_default.xml");
//            ProfFace = new CascadeClassifier("haarcascade_profileface.xml");
            TopMost = true;
           InitializeComponent();
//           timer.Enabled = true;
            timer2.Enabled = true;
//            timer.Interval = 60000;
//            timer.Tick += Timer_Tick;
        }

       private void Timer_Tick(object sender, EventArgs e)
       {
           if (DateTime.Now.TimeOfDay <= new TimeSpan(2)) return;
           var pop = new PopupNotifier
           {
               Image = Resources.Look,
               ContentText = "Attention!.",
               BodyColor = Color.Green,
               ContentColor = Color.White,
               AnimationInterval = 3000,
               AnimationDuration = 100,
               ContentFont = new Font("Tahoma", 15F),
               Delay = 4000,
               BorderColor = Color.Yellow
           };
           pop.Popup();
       }

       static Dictionary<DateTime, string> signal; // Создал словарь
/*

        private void Timer_Tick(object sender, EventArgs e)
        {

            if (DateTime.Now.TimeOfDay > new TimeSpan(2))
            {

                PopupNotifier p = new PopupNotifier();
                p.Image = Properties.Resources.Look;
                p.ContentText = "Пора сделать перерыв";
                p.BodyColor = Color.Teal;
                p.ContentColor = Color.White;
                p.AnimationInterval = 1000;
                p.AnimationDuration = 100;
                p.ContentFont = new Font("Tahoma", 10F);
                p.Delay = 2000;
                p.BorderColor = Color.Yellow;
                p.Popup();
                //i = i + 1;
            }

        }
*/

        public void FormOpacity()
        {
            FormBorderStyle = FormBorderStyle.FixedSingle;
            AllowTransparency = true;
            BackColor = Color.AliceBlue; //цвет фона  
            TransparencyKey = BackColor; //он же будет заменен на прозрачный цвет
            panel1.BackColor = Color.Transparent;
            BackColor = label1.BackColor = Color.AliceBlue;
        }

        private void Application_Idle(object sender, EventArgs e)
        {
            _currentFrame = _camera.QueryFrame().ToImage<Bgr, byte>().Resize(640, 480, Inter.Cubic);
            var faceDetect = _faces.DetectMultiScale(_currentFrame, 1.2, 10, new Size(10, 10), new Size(640, 480));
            foreach (var f in faceDetect)
            {
                _currentFrame.Draw(f, new Bgr(Color.Blue), 2);


                var eyeDetect = _eye.DetectMultiScale(_currentFrame,
                    1.2,
                    10,
                    new Size(8,
                        8),
                    new Size(160,
                        120));
                if (eyeDetect.Length <= 0)
                {
                    var image = new Image<Gray, byte>(_currentFrame.Width, _currentFrame.Height, new Gray(0));
                    _currentFrame.Canny(50, 20);
                    imageBox2.Image = _currentFrame;
                    _currentFrame.Draw(f, new Bgr(Color.Red), 5);
                    _currentFrame.Draw("Close Eye", new Point(f.X, f.Y), FontFace.HersheyTriplex, 2.0,
                        new Bgr(Color.White));
//                    label2.Text = i++.ToString();


                    Invoke((MethodInvoker) delegate
                    {
                        {
                            var popup = new PopupNotifier
                            {
                                Image = Resources.Look,
                                ContentText = "Attention!\nOften squinting",
                                BodyColor = Color.Teal,
                                ContentColor = Color.White,
                                AnimationInterval = 1000,
                                AnimationDuration = 100,
                                ContentFont = new Font("Tahoma",
                                10F),
                                Delay = 2000,
                                BorderColor = Color.Yellow
                            };
                            popup.Popup();
                            _i += 1;

                            if (_i <= 8) return;
                            var pop = new PopupNotifier
                            {
                                Image = Resources.Look,
                                ContentText = "Attention!\nEyes are tired.",
                                BodyColor = Color.Green,
                                ContentColor = Color.White,
                                AnimationInterval = 3000,
                                AnimationDuration = 100,
                                ContentFont = new Font("Tahoma",
                                    15F),
                                Delay = 4000,
                                BorderColor = Color.Yellow
                            };
                            _i = 0;

                            pop.Popup();
                        }
                    });
                    _count = 0;
                }
                else
                {
                    _count = _count + 1;
                }

                if (_count >= 20)
                    _count = 0;
                foreach (var eye in eyeDetect)
                {
                    eye.Inflate(-8,
                        -8);
                    _currentFrame.Draw(eye,
                        new Bgr(Color.Blue),
                        5);
                }

                _currentFrame.ROI = f;
                FormOpacity();
            }

            _currentFrame.ROI = Rectangle.Empty;
            imageBox2.Image = _currentFrame;
        }

        private void button2_Click(object sender,
            EventArgs e)
        {
            Invoke((MethodInvoker) delegate
            {
                _camera = new VideoCapture();
                Application.Idle += Application_Idle;
                button2.Visible = true;
//                label1.Visible = true;
            });
        }

        /*
        private void button1_Click(object sender, EventArgs e)
        {

        }
        private Point MouseHook;
        private new void Move(object sender, MouseEventArgs e)
        {
            Cursor = Cursors.Default;
            if (e.Button != MouseButtons.Left) MouseHook = e.Location;
            else
            {
                Location = new Point((Size)Location - (Size)MouseHook + (Size)e.Location);
                Cursor = Cursors.Hand;
            }

        }
*/


    }
   
}



   
   


           

    

