using Microsoft.Win32;
using System.Windows;
using ImageProcessingApp.Utils;
using ImageProcessingApp.Models;
using System.Collections.ObjectModel;
using System.Windows.Controls;

namespace ImageProcessingApp.Views
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {
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
            var selectedItems = FileListBox.SelectedItems;
            while (selectedItems.Count > 0)
            {
                FileListBox.Items.Remove(selectedItems[0]);
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

            // 为每个文件启动一个独立线程
            foreach (var item in FileItems)
            {
                Thread processingThread = new Thread(() =>
                {
                    try
                    {
                        string inputPath = item.FilePath;
                        string outputPath = ProcessImage(inputPath, mode, outputDir);

                        if (!string.IsNullOrEmpty(outputPath))
                        {
                            item.Status = Status.COMPLETED;
                            item.OutputPath = outputPath;

                            // 使用 Dispatcher 更新进度条和UI
                            Dispatcher.Invoke(() =>
                            {
                                ProcessingProgressBar.Value += 1;
                                if (ProcessingProgressBar.Value == FileItems.Count) StatusTextBlock.Text = "处理完成";
                            });
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
                    finally
                    {
                        // 使用 Dispatcher 更新UI显示状态
                        //Dispatcher.Invoke(() =>
                        //{
                        //    FileListBox.Items.Refresh();
                        //});
                    }
                });
                processingThread.IsBackground = true;
                processingThread.Start();
            }
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
