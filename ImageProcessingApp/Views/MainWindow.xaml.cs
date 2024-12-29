using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

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
        }

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
                    FileListBox.Items.Add($"{filename} [待处理]");
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
                MessageBox.Show("请先添加文件。", "提示");
                return;
            }

            string mode = (ProcessingModeComboBox.SelectedItem as ComboBoxItem)?.Content.ToString() ?? "默认模式";
            MessageBox.Show($"开始处理文件，模式：{mode}。", "提示");
            for (int i = 0; i < FileListBox.Items.Count; i++)
            {
                FileListBox.Items[i] = FileListBox.Items[i].ToString().Replace("[待处理]", "[处理中]");
            }
        }

        private void CancelProcessing_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("取消处理。", "提示");
            for (int i = 0; i < FileListBox.Items.Count; i++)
            {
                FileListBox.Items[i] = FileListBox.Items[i].ToString().Replace("[处理中]", "[待处理]");
            }
        }
    }
}
