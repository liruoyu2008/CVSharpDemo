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
            PlayFromFile(@"E:\电影\生化危机：诅咒.720p.BD中英双字幕.rmvb");
        }

        private void PlayFromCapture()
        {
            Task.Run(() =>
            {
                var cap = new VideoCapture(0);
                using (Mat img = new Mat())
                {
                    while (true)
                    {
                        cap.Read(img);
                        if (img.Empty())
                        {
                            break;
                        }

                        var img1 = new Mat(img.Size(), MatType.CV_8UC1);
                        Cv2.CvtColor(img, img1, ColorConversionCodes.BGR2GRAY);

                        var bitmap = img1.ToWriteableBitmap().ToBitmapImage();
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
