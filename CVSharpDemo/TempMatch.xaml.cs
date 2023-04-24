using OpenCvSharp;
using OpenCvSharp.WpfExtensions;
using System.Windows;

namespace CVSharpDemo
{
    /// <summary>
    /// TempMatch.xaml 的交互逻辑
    /// </summary>
    public partial class TempMatch : System.Windows.Window
    {
        public TempMatch()
        {
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            MatchCarPlate();
        }

        private void MatchCarPlate()
        {
            var raw = Cv2.ImRead("mario.png");
            var rawTemp = Cv2.ImRead("coin.png");
            var mat1 = new Mat();
            Cv2.CvtColor(raw, mat1, ColorConversionCodes.BGR2GRAY);
            var mat2 = new Mat();
            Cv2.CvtColor(rawTemp, mat2, ColorConversionCodes.BGR2GRAY);
            Mat mat3 = new Mat();
            Cv2.MatchTemplate(mat1, mat2, mat3, TemplateMatchModes.CCoeffNormed);
            var mat5 = mat3.Threshold(0.8, 1, ThresholdTypes.Binary);
            var indexer = mat5.GetGenericIndexer<Vec3b>();
            for (int i = 0; i < mat5.Width; i++)
            {
                for (int j = 0; j < mat5.Height; j++)
                {
                    if (indexer[j, i].Item2 > 0)
                    {
                        Cv2.Rectangle(raw, new(new(i, j), new(mat2.Width, mat2.Height)), Scalar.Red, 1);
                    }
                }
            }

            image1.Source = mat1.ToWriteableBitmap().ToBitmapImage();
            image2.Source = mat2.ToWriteableBitmap().ToBitmapImage();
            image3.Source = mat5.ToWriteableBitmap().ToBitmapImage();
            image4.Source = raw.ToWriteableBitmap().ToBitmapImage();

        }
    }
}
