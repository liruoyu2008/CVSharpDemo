using OpenCvSharp;
using OpenCvSharp.WpfExtensions;
using System;
using System.Threading.Tasks;
using System.Windows;

namespace CVSharpDemo
{
    /// <summary>
    /// FaceDetection.xaml 的交互逻辑
    /// </summary>
    public partial class FaceDetection : System.Windows.Window
    {
        public FaceDetection()
        {
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            Detect();
        }

        private void Detect()
        {
            var face_cascade = new CascadeClassifier("C:\\Users\\ryu\\.conda\\envs\\py312" +
                "\\Lib\\site-packages\\cv2\\data\\haarcascade_frontalface_default.xml");

            Task.Run(() =>
            {
                var cap = new VideoCapture(0, VideoCaptureAPIs.DSHOW);
                while (cap.IsOpened())
                {
                    // 加载视频帧
                    var image = new Mat();
                    var res = cap.Read(image);
                    var gray = new Mat();
                    Cv2.CvtColor(image, gray, ColorConversionCodes.BGR2GRAY);

                    //进行面部检测
                    var faces = face_cascade.DetectMultiScale(gray, scaleFactor: 1.1, minNeighbors: 5, minSize: new OpenCvSharp.Size(30, 30));

                    // 在检测到的面部周围画矩形框
                    foreach (var item in faces)
                    {
                        Cv2.Rectangle(image, new OpenCvSharp.Rect(item.Location, item.Size), new Scalar(255, 0, 0), 2);
                    }

                    var bitmap = image.ToWriteableBitmap().ToBitmapImage();
                    if (Application.Current != null)
                    {
                        Dispatcher.Invoke(new Action(() =>
                        {
                            if (image1 != null)
                            {
                                image1.Source = bitmap;

                            }
                        }));
                    }
                }
            });
        }
    }
}
