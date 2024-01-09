using System.Windows;
using Window = System.Windows.Window;

namespace CVSharpDemo
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            App.Current.Shutdown();
        }

        private void Play_Video(object sender, RoutedEventArgs e)
        {
            new PlayVideo().Show();
        }

        private void Temp_Match(object sender, RoutedEventArgs e)
        {
            new TempMatch().Show();
        }

        private void Face_Detect(object sender, RoutedEventArgs e)
        {
            new FaceDetection().Show();
        }
    }
}
