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

        private ManualResetEvent globalStopSignal = new ManualResetEvent(false);

        public MainWindow()
        {
            InitializeComponent();
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
                var selectedFile = (FileItem)FileListBox.SelectedItem;
                string beforePath = selectedFile.FilePath;
                string afterPath = selectedFile.OutputPath;
                if (!string.IsNullOrEmpty(afterPath))
                {
                    DetailWindow detailsWindow = new DetailWindow(beforePath, afterPath);
                    detailsWindow.ShowDialog();
                }
                else
                {
                    MessageBox.Show("未找到处理结果文件。", "提示");
                }
            }
            else
            {
                MessageBox.Show("请选择一个文件。", "提示");
            }
        }


        // 开始处理
        private async void StartProcessing_Click(object sender, RoutedEventArgs e)
        {
            globalStopSignal.Reset();

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
            string outputDir = folder.FolderName;
            string mode = (ProcessingModeComboBox.SelectedItem as ComboBoxItem)?.Content.ToString();

            int completedCount = 0;

            // 启动任务
            await Task.Run(() =>
            {
                foreach (var item in FileItems)
                {
                    if (globalStopSignal.WaitOne(0))
                    {
                        item.Status = Status.CANCELED;
                        continue;
                    }

                    string inputPath = item.FilePath;
                    string outputPath = ProcessImage(inputPath, mode, outputDir);

                    if (!string.IsNullOrEmpty(outputPath))
                    {
                        item.Status = Status.COMPLETED;
                        item.OutputPath = outputPath;
                        Interlocked.Increment(ref completedCount);

                        // 调用 UI 线程更新进度
                        Dispatcher.Invoke(() =>
                        {
                            ProcessingProgressBar.Value = completedCount;
                            if (completedCount == FileItems.Count)
                            {
                                StatusTextBlock.Text = "处理完成";
                                MessageBox.Show("所有文件处理完成！", "提示");
                            }
                        });
                    }
                    else
                    {
                        item.Status = Status.FAILED;
                    }
                }
            });
        }

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
                "高斯模糊" => ImageProcessor.ApplyGaussianBlur(inputPath, outputDir),
                "边缘检测" => ImageProcessor.ApplyEdgeDetection(inputPath, outputDir),
                "对比度增强" => ImageProcessor.EnhanceContrast(inputPath, outputDir),
                _ => null,
            };
        }

        // 取消处理
        private void CancelProcessing_Click(object sender, RoutedEventArgs e)
        {
            StatusTextBlock.Text = "已取消处理。";
            globalStopSignal.Set();
        }

        private void ClearFile_Click(object sender, RoutedEventArgs e)
        {
            FileItems.Clear();
        }
    }
}
