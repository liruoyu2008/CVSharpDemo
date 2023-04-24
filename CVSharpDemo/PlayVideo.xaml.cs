using HandyControl.Tools;
using OpenCvSharp;
using OpenCvSharp.WpfExtensions;
using System;
using System.Threading.Tasks;
using System.Windows;


namespace CVSharpDemo
{
    /// <summary>
    /// PlayVideo.xaml 的交互逻辑
    /// </summary>
    public partial class PlayVideo : System.Windows.Window
    {
        public PlayVideo()
        {
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            PlayFromCapture();
        }

        private void PlayFromCapture()
        {
            Task.Run(() =>
            {
                var cap = new VideoCapture(0, VideoCaptureAPIs.DSHOW);
                using (Mat img = new Mat())
                {
                    while (true)
                    {
                        cap.Read(img);
                        if (img.Empty())
                        {
                            break;
                        }

                        // 转换为灰度图便与检测
                        var gray = new Mat(img.Size(), MatType.CV_8UC1);
                        Cv2.CvtColor(img, gray, ColorConversionCodes.BGR2GRAY);

                        // 载入级联分类器参数
                        var haarCascade = new CascadeClassifier("haarcascade_frontalface_alt2.xml");

                        // 人脸检测
                        OpenCvSharp.Rect[] faces = haarCascade.DetectMultiScale(
                                            gray, 1.08, 2, HaarDetectionTypes.ScaleImage, new OpenCvSharp.Size(30, 30));

                        // Canny边缘检测
                        var edges = new Mat();
                        Cv2.Canny(gray, edges, 50, 60);
                        
                        // 原图上画框
                        foreach (var face in faces)
                        {
                            Cv2.Rectangle(edges, face, Scalar.White);
                        }

                        var bitmap = edges.ToWriteableBitmap().ToBitmapImage();
                        if (Application.Current != null)
                        {
                            Dispatcher.Invoke(new Action(() =>
                            {
                                if (image != null)
                                {
                                    image.Source = bitmap;

                                }
                            }));
                        }
                    }
                }
            });
        }

        private void PlayFromFile(string filename)
        {
            var cap = new VideoCapture(filename);
            Task.Run(() =>
            {
                using (Mat img = new Mat())
                {
                    while (true)
                    {
                        cap.Read(img);
                        if (img.Empty())
                        {
                            break;
                        }

                        var bitmap = img.ToWriteableBitmap().ToBitmapImage();
                        if (Application.Current != null)
                        {
                            Dispatcher.Invoke(new Action(() =>
                            {
                                if (image != null)
                                {
                                    image.Source = bitmap;

                                }
                            }));
                        }
                    }
                }
            });
        }
    }
}
