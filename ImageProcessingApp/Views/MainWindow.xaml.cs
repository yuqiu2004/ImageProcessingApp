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
            FileListBox.ItemsSource = FileItems; // 绑定数据源
        }

        private ObservableCollection<FileItem> FileItems { get; set; } = new ObservableCollection<FileItem>();

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

        private void DeleteFile_Click(object sender, RoutedEventArgs e)
        {
            var selectedItems = FileListBox.SelectedItems;
            while (selectedItems.Count > 0)
            {
                FileListBox.Items.Remove(selectedItems[0]);
            }
        }

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

        private void StartProcessing_Click(object sender, RoutedEventArgs e)
        {
            OpenFolderDialog folder = new OpenFolderDialog()
            {
                Title = "选择输出路径",
            };
            if (folder.ShowDialog() == false) {
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
            for (int i = 0; i < FileListBox.Items.Count; i++)
            {
                var item = (FileItem)FileListBox.Items[i];
                string inputPath = item.FilePath;
                string outputPath = "";
                if (mode.Equals("灰度"))
                {
                    outputPath = ImageProcessor.ConvertToGray(inputPath, outputDir);
                } 
                else if (mode.Equals("放大 200%"))
                {
                    outputPath = ImageProcessor.EnlargeTo200Percent(inputPath, outputDir);
                }
                else if (mode.Equals("缩小 50%"))
                {
                    outputPath = ImageProcessor.ShrinkTo50Percent(inputPath, outputDir);
                }
                else if (mode.Equals("顺时针旋转90°"))
                {
                    outputPath = ImageProcessor.RotateClockwise90(inputPath, outputDir);
                }
                else if (mode.Equals("逆时针旋转90°"))
                {
                    outputPath = ImageProcessor.RotateCounterClockwise90(inputPath, outputDir);
                }
                if (!String.IsNullOrEmpty(outputPath))
                {
                    item.Status = Status.COMPLETED;
                    ProcessingProgressBar.Value += 1;
                }
                else
                {
                    item.Status = Status.FAILED;
                }
            }

            StatusTextBlock.Text = "处理完成！";
            //MessageBox.Show($"所有文件处理完成，模式：{mode}。", "提示");
        }

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
