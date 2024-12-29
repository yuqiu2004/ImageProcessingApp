
namespace ImageProcessingApp.Models
{
    public static class Status
    {
        private const string _PENDING = "待处理";
        private const string _COMPLETED = "处理完成";
        private const string _FAILED = "处理失败";
        private const string _CANCELED = "取消处理";

        public static string PENDING => _PENDING;
        public static string COMPLETED => _COMPLETED;
        public static string FAILED => _FAILED;
        public static string CANCELED => _CANCELED;
    }
}
