using System.Windows;
using System.Windows.Media.Imaging;

namespace ImageProcessingApp.Views
{
    /// <summary>
    /// DetailWindow.xaml 的交互逻辑
    /// </summary>
    public partial class DetailWindow : Window
    {
        public DetailWindow(string beforePath, string afterPath)
        {
            InitializeComponent();
            // 加载处理前后的图片
            BeforeImage.Source = LoadImage(beforePath);
            AfterImage.Source = LoadImage(afterPath);
        }

        // 加载图片并返回BitmapImage
        private BitmapImage LoadImage(string path)
        {
            var bitmap = new BitmapImage();
            bitmap.BeginInit();
            bitmap.UriSource = new Uri(path);
            bitmap.EndInit();
            return bitmap;
        }
    }
}
