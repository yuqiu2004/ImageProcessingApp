using Microsoft.Win32;
using System.Windows;
using ImageProcessingApp.Utils;
using ImageProcessingApp.Models;
using System.Collections.ObjectModel;
using System.Windows.Controls;
using System.Runtime.InteropServices;
using System.Windows.Interop;

namespace ImageProcessingApp.Views
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {

        private const int WM_USER_PROGRESS = 0x0400; // 自定义消息

        private IntPtr _windowHandle;

        private void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            _windowHandle = new WindowInteropHelper(this).Handle;

            // 添加消息钩子
            HwndSource source = HwndSource.FromHwnd(_windowHandle);
            source.AddHook(WndProc);
        }

        // 消息处理函数
        private IntPtr WndProc(IntPtr hwnd, int msg, IntPtr wParam, IntPtr lParam, ref bool handled)
        {
            if (msg == WM_USER_PROGRESS)
            {
                // 获取进度
                int completed = wParam.ToInt32();
                UpdateProgress(completed);
                handled = true;
            }
            return IntPtr.Zero;
        }

        private void UpdateProgress(int completed)
        {
            ProcessingProgressBar.Value = completed;

            if (completed == FileItems.Count)
            {
                StatusTextBlock.Text = "处理完成";
                MessageBox.Show("所有文件处理完成！", "提示");
            }
        }

        public MainWindow()
        {
            InitializeComponent();
            // 初始化窗口句柄
            Loaded += MainWindow_Loaded;
            this.DataContext = this;
            FileListBox.ItemsSource = FileItems; // 绑定数据源
        }

        private ObservableCollection<FileItem> FileItems { get; set; } = new ObservableCollection<FileItem>();

        // 添加文件项
        private void AddImage_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog
            {
                Multiselect = true,
                Filter = "Image Files (*.jpg;*.png;*.bmp)|*.jpg;*.png;*.bmp"
            };
            if (openFileDialog.ShowDialog() == true)
            {
                foreach (string filename in openFileDialog.FileNames)
                {
                    FileItems.Add(new FileItem { FilePath = filename, Status = Status.PENDING });
                }
            }
        }

        // 删除文件项
        private void DeleteFile_Click(object sender, RoutedEventArgs e)
        {
            var selectedItems = FileListBox.SelectedItems.Cast<FileItem>().ToList();
            foreach (var item in selectedItems)
            {
                FileItems.Remove(item);
            }
        }

        // 查看结果
        private void ViewResult_Click(object sender, RoutedEventArgs e)
        {
            if (FileListBox.SelectedItem != null)
            {
                MessageBox.Show($"查看 {FileListBox.SelectedItem} 的处理结果。", "提示");
            }
            else
            {
                MessageBox.Show("请选择一个文件。", "提示");
            }
        }

        // 开始处理
        private void StartProcessing_Click(object sender, RoutedEventArgs e)
        {
            OpenFolderDialog folder = new OpenFolderDialog()
            {
                Title = "选择输出路径",
            };
            if (folder.ShowDialog() == false)
            {
                MessageBox.Show("系统出错");
                return;
            }
            if (FileListBox.Items.Count == 0)
            {
                StatusTextBlock.Text = "请先添加文件。";
                MessageBox.Show("请先添加文件。", "提示");
                return;
            }
            if (ProcessingModeComboBox.SelectedItem == null)
            {
                StatusTextBlock.Text = "请选择处理模式。";
                MessageBox.Show("请选择处理模式。", "提示");
                return;
            }

            ProcessingProgressBar.Value = 0;
            ProcessingProgressBar.Maximum = FileListBox.Items.Count;
            StatusTextBlock.Text = "处理中...";
            //string outputPath = @"C:\Users\SN\Desktop\temp";
            string outputDir = folder.FolderName;
            string mode = (ProcessingModeComboBox.SelectedItem as ComboBoxItem)?.Content.ToString();
            int completedCount = 0;

            // 为每个文件启动一个独立线程
            foreach (var item in FileItems)
            {
                Thread processingThread = new Thread(() =>
                {
                    try
                    {
                        string inputPath = item.FilePath;
                        string outputPath = ProcessImage(inputPath, mode, outputDir);
                        Thread.Sleep(1000); // 方便测试
                        if (!string.IsNullOrEmpty(outputPath))
                        {
                            item.Status = Status.COMPLETED;
                            item.OutputPath = outputPath;
                            Interlocked.Increment(ref completedCount);

                            // 使用 PostMessage 发送更新进度的消息
                            PostMessage(_windowHandle, WM_USER_PROGRESS, new IntPtr(completedCount), IntPtr.Zero);
                        }
                        else
                        {
                            item.Status = Status.FAILED;
                        }
                    }
                    catch (Exception ex)
                    {
                        item.Status = Status.FAILED;
                    }
                });
                processingThread.IsBackground = true;
                processingThread.Start();
            }
        }

        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        private static extern bool PostMessage(IntPtr hWnd, int Msg, IntPtr wParam, IntPtr lParam);

        // 处理调用
        private string ProcessImage(string inputPath, string mode, string outputDir)
        {
            return mode switch
            {
                "灰度" => ImageProcessor.ConvertToGray(inputPath, outputDir),
                "放大 200%" => ImageProcessor.EnlargeTo200Percent(inputPath, outputDir),
                "缩小 50%" => ImageProcessor.ShrinkTo50Percent(inputPath, outputDir),
                "顺时针旋转90°" => ImageProcessor.RotateClockwise90(inputPath, outputDir),
                "逆时针旋转90°" => ImageProcessor.RotateCounterClockwise90(inputPath, outputDir),
                _ => null,
            };
        }

        // 取消处理
        private void CancelProcessing_Click(object sender, RoutedEventArgs e)
        {
            ProcessingProgressBar.Value = 0;
            StatusTextBlock.Text = "已取消处理。";
            for (int i = 0; i < FileListBox.Items.Count; i++)
            {
                FileListBox.Items[i] = ((FileItem)FileListBox.Items[i]).Status = Status.CANCELED;
            }
        }

    }
}
