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
    public partial class Draw : System.Windows.Window
    {
        public Draw()
        {
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            // 创建黑色的图像
            var img = new Mat(new OpenCvSharp.Size(512, 512), MatType.CV_8UC3, Scalar.Green);
            // 绘制一条厚度为5的蓝色对角线
            Cv2.Line(img, 0, 0, 511, 511, Scalar.Pink, 5);
            var bm = img.ToWriteableBitmap().ToBitmapImage();
            image.Source = bm;
        }
    }
}
