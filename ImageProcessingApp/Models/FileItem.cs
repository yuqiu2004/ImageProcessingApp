using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace ImageProcessingApp.Models
{
    public class FileItem : INotifyPropertyChanged
    {
        private string? _filePath;
        private string? _status;
        private string? _outputPath;

        public string FilePath
        {
            get => _filePath;
            set => SetProperty(ref _filePath, value);
        }

        public string Status
        {
            get => _status;
            set => SetProperty(ref _status, value);
        }

        public string OutputPath
        {
            get => _outputPath;
            set => SetProperty(ref _outputPath, value);
        }

        public override string ToString()
        {
            return $"{System.IO.Path.GetFileName(FilePath)} -----> [ {Status} ]";
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        protected bool SetProperty<T>(ref T storage, T value, [CallerMemberName] string propertyName = null)
        {
            if (Equals(storage, value)) return false;
            storage = value;
            OnPropertyChanged(propertyName);
            return true;
        }
    }

}
