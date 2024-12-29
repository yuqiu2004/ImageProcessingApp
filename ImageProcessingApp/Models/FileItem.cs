using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace ImageProcessingApp.Models
{
    public class FileItem : INotifyPropertyChanged
    {
        private string? _filePath;
        private string? _fileName;
        private string? _status;
        private string? _outputPath;

        public string FilePath
        {
            get => _filePath;
            set
            {
                _filePath = value;
                _fileName = System.IO.Path.GetFileName(value);
            }
        }

        public string FileName
        {
            get => _fileName;
            set
            {
                _fileName = value;
            }
        }

        public string Status
        {
            get => _status;
            set 
            {
                _status = value;
                OnPropertyChanged(nameof(Status));
            }
        }

        public string OutputPath
        {
            get => _outputPath;
            set
            { 
                _outputPath = value;
            }
        }

        public override string ToString()
        {
            return $"{System.IO.Path.GetFileName(FilePath)} -----> [ {Status} ]";
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }

}
