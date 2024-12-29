namespace ImageProcessingApp.Models
{
    public class FileItem
    {
        public string FilePath { get; set; } // 文件路径
        public string Status { get; set; } // 状态

        public override string ToString()
        {
            return $"{System.IO.Path.GetFileName(FilePath)} -----> [ {Status} ]";
        }
    }

}
