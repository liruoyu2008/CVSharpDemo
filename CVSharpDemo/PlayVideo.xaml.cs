using HandyControl.Tools;
using OpenCvSharp;
using OpenCvSharp.Flann;
using OpenCvSharp.WpfExtensions;
using System;
using System.Collections;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;

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
            // PlayFromCapture();
            // PlayFromFile(@"E:\电影\生化危机：诅咒.720p.BD中英双字幕.rmvb");
            PlayCompare(@"C:\Users\Ryu\Desktop\4.黄鹤\4.黄鹤.mp4", @"C:\Users\Ryu\Desktop\4.黄鹤\4.黄鹤2.mp4");
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

                        var bitmap = img.ToWriteableBitmap().ToBitmapImage();
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
                    cap.Set(VideoCaptureProperties.PosMsec, 600000);
                    while (true)
                    {
                        cap.Read(img);

                        if (img.Empty())
                        {
                            break;
                        }
                        var img2 = AdjustMent2(img);
                        var bright2 = GetBrightness(img2);
                        var bitmap = img2.ToWriteableBitmap().ToBitmapImage();

                        if (Application.Current != null)
                        {
                            Dispatcher.Invoke(new Action(() =>
                            {
                                if (image1 != null)
                                {
                                    image1.Source = bitmap;
                                    tb1.Text = bright2.ToString();
                                }
                            }));
                        }
                    }
                }
            });
        }



        private double _sliderValue;
        private bool _canChangeSlider = true;
        private readonly object _readLocker = new object();
        private readonly object _writeLocker = new object();

        /// <summary>
        /// 播放视频对比,并输出到文件
        /// </summary>
        /// <param name="filename"></param>
        /// <param name="outfile"></param>
        private void PlayCompare(string filename, string outfile)
        {
            var cap = new VideoCapture(filename);
            var writer = new VideoWriter(outfile, FourCC.H264, 30, new OpenCvSharp.Size(1280, 720));
            Task.Run((() =>
            {
                PlayAndWrite(cap, writer);
                cap.Release();
                writer.Release();
            }));
        }

        /// <summary>
        /// 播放并写入
        /// </summary>
        /// <param name="cap"></param>
        private void PlayAndWrite(VideoCapture cap, VideoWriter writer)
        {
            using (Mat img1 = new Mat())
            {
                while (true)
                {
                    double pos = 0;
                    lock (_readLocker)
                    {
                        pos = _sliderValue;
                        cap.Set(VideoCaptureProperties.PosFrames, _sliderValue);
                        cap.Read(img1);
                        _sliderValue++;
                    }

                    if (img1.Empty())
                    {
                        break;
                    }

                    // 左
                    var bright1 = GetBrightness(img1);
                    var bitmap1 = img1.ToWriteableBitmap().ToBitmapImage();

                    //// 右
                    var img2 = AdjustMent2(img1);
                    var bright2 = GetBrightness(img2);
                    var bitmap2 = img2.ToWriteableBitmap().ToBitmapImage();

                    //// 屏幕区域
                    //var img3 = new Mat();
                    //var img4 = new Mat();
                    //var img5 = new Mat();
                    //var img6 = new Mat();
                    //var img7 = new Mat();
                    //var img8 = new Mat();
                    //Cv2.CvtColor(img1, img3, ColorConversionCodes.BGRA2GRAY);
                    //Cv2.MedianBlur(img3, img3, 7);
                    //Cv2.Threshold(img3, img3, 80, 255, ThresholdTypes.Binary);
                    //Cv2.Canny(img3, img3, 200, 2.5);
                    //Cv2.FindContours(img3, out Mat[] cons, img4, RetrievalModes.CComp, ContourApproximationModes.ApproxSimple);
                    //var max = cons.OrderBy(it => GetArea(Cv2.BoundingRect(it))).Last();
                    //var rec = Cv2.BoundingRect(max);
                    //Cv2.Rectangle(img2, rec, Scalar.Red, 2);
                    //var bright3 = GetBrightness(img2);
                    //var bitmap3 = img2.ToWriteableBitmap().ToBitmapImage();

                    // 写入
                    writer.Write(img2);

                    // 播放
                    if (Application.Current != null)
                    {
                        Dispatcher.Invoke(new Action(() =>
                        {
                            image1.Source = bitmap1;
                            image2.Source = bitmap2;
                            tb1.Text = bright1.ToString();
                            tb2.Text = bright2.ToString();

                            // 滑动条
                            if (_canChangeSlider)
                            {
                                slider.Value = _sliderValue;
                            }
                        }));
                    }
                }
            }
        }

        /// <summary>
        /// 获取图像平均亮度
        /// </summary>
        /// <param name="mat"></param>
        /// <returns></returns>
        private double GetBrightness(Mat mat)
        {
            var brightness = Cv2.Mean(mat);
            return (brightness.Val0 + brightness.Val1 + brightness.Val2) / 3;
        }

        /// <summary>
        /// 低亮度图像加高对比度，并保证经过处理的图像亮度最低为130
        /// </summary>
        /// <param name="img"></param>
        /// <returns></returns>
        private Mat AdjustMent(Mat img)
        {
            var b = GetBrightness(img);

            if (b > 100)
            {
                return img;
            }

            // 对比度调整
            Mat img2 = new Mat();
            img.ConvertTo(img2, MatType.CV_8UC3, 2.5, 0);
            var b2 = GetBrightness(img2);

            if (b2 > 130)
            {
                return img2;
            }

            Mat img3 = new Mat();
            img2.ConvertTo(img3, MatType.CV_8UC3, 1, (130 - b2));
            return img3;
        }

        /// <summary>
        /// 根据反比例函数平滑处理图像对比图,并保证最低亮度
        /// </summary>
        /// <param name="img"></param>
        /// <returns></returns>
        private Mat AdjustMent2(Mat img)
        {
            var b = GetBrightness(img);

            if (b >= 130)
            {
                return img;
            }

            // 对比度调整
            Mat img2 = new Mat();
            var c = (250 / b) - 0.92;
            img.ConvertTo(img2, MatType.CV_8UC3, b <= 70 ? 2.5 : c, 0);
            var b2 = GetBrightness(img2);
            if (b2 > 130)
            {
                return img2;
            }

            // 保底亮度
            Mat img3 = new Mat();
            img2.ConvertTo(img3, MatType.CV_8UC3, 1, (130 - b2));
            return img3;
        }

        private void slider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            _sliderValue = e.NewValue;
            tb3.Text = ((int)e.NewValue).ToString();
            tb4.Text = TimeSpan.FromSeconds((e.NewValue / 75575) * 2519).ToString();
        }

        private void slider_DragStarted(object sender, System.Windows.Controls.Primitives.DragStartedEventArgs e)
        {
            _canChangeSlider = false;
        }

        private void slider_DragCompleted(object sender, System.Windows.Controls.Primitives.DragCompletedEventArgs e)
        {
            _canChangeSlider = true;
        }

        private void Window_PreviewKeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            _canChangeSlider = false;
            if (e.Key == Key.Left)
            {
                if (_sliderValue <= 10)
                {
                    _sliderValue = 0;
                }
                else
                {
                    _sliderValue -= 10;
                }
            }
            else if (e.Key == Key.Right)
            {
                if (_sliderValue >= 75565)
                {
                    _sliderValue = 75575;
                }
                else
                {
                    _sliderValue += 10;
                }
            }
            slider.Value = _sliderValue;
        }

        private void Window_PreviewKeyUp(object sender, System.Windows.Input.KeyEventArgs e)
        {
            _canChangeSlider = true;
        }
    }
}

