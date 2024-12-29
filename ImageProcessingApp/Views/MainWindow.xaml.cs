using Microsoft.Win32;
using System.Windows;
using ImageProcessingApp.Utils;
using ImageProcessingApp.Models;
using System.Collections.ObjectModel;

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

            //string mode = (ProcessingModeComboBox.SelectedItem as ComboBoxItem)?.Content.ToString();
            for (int i = 0; i < FileListBox.Items.Count; i++)
            {
                // 模拟处理每个文件
                //System.Threading.Thread.Sleep(500); // 替换为实际的处理逻辑
                //FileListBox.Items[i] = FileListBox.Items[i].ToString().Replace("[ 待处理 ]", "[ 已处理 ]");
                string outputPath = @"C:\Users\SN\Desktop\temp";
                string inputPath = ((FileItem)FileListBox.Items[i]).FilePath;
                //MessageBox.Show(inputPath);
                if (ImageProcessor.ConvertToGray(inputPath, outputPath)) ProcessingProgressBar.Value += 1;
                else MessageBox.Show("error");
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
