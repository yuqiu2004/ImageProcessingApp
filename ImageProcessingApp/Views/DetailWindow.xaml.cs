using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;

namespace ImageProcessingApp.Views
{
    /// <summary>
    /// DetailWindow.xaml 的交互逻辑
    /// </summary>
    public partial class DetailWindow : Window
    {

        private string _beforePath;
        private string _afterPath;

        public DetailWindow(string beforePath, string afterPath)
        {
            InitializeComponent();
            _beforePath = beforePath;
            _afterPath = afterPath;
            // 加载处理前后的图片
            LoadImages();
        }

        private void LoadImages()
        {
            // 加载处理前的图片
            var beforeBitmap = new BitmapImage(new Uri(_beforePath));
            var afterBitmap = new BitmapImage(new Uri(_afterPath));

            // 计算缩放比例
            double beforeScale = CalculateScale(beforeBitmap, 350, 350);
            double afterScale = CalculateScale(afterBitmap, 350, 350);

            // 取较大的缩放比例
            double unifiedScale = Math.Min(beforeScale, afterScale);

            // 设置图片尺寸
            SetImageSize(BeforeImage, beforeBitmap, unifiedScale);
            SetImageSize(AfterImage, afterBitmap, unifiedScale);
        }

        private double CalculateScale(BitmapImage image, double maxWidth, double maxHeight)
        {
            double widthScale = maxWidth / image.PixelWidth;
            double heightScale = maxHeight / image.PixelHeight;
            return Math.Min(widthScale, heightScale); // 保持原比例
        }

        private void SetImageSize(Image imageControl, BitmapImage bitmap, double scale)
        {
            imageControl.Source = bitmap;
            imageControl.Width = bitmap.PixelWidth * scale;
            imageControl.Height = bitmap.PixelHeight * scale;
        }
    }
}
