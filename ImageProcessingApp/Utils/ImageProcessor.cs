using OpenCvSharp;
using System.IO;

namespace ImageProcessingApp.Utils
{
    public static class ImageProcessor
    {
        /// <summary>
        /// 将图片转换为灰度图并保存到指定目录
        /// </summary>
        public static bool ConvertToGray(string inputPath, string outputDirectory)
        {
            try {
                Mat input = Cv2.ImRead(inputPath);
                if (input.Empty()) return false;

                Mat gray = new Mat();
                Cv2.CvtColor(input, gray, ColorConversionCodes.BGR2GRAY);

                string outputPath = GetModifiedFilePath(inputPath, outputDirectory, "_gray");
                Cv2.ImWrite(outputPath, gray);
                return true;
            } catch (Exception ex) {
                return false;
            }
        }

        /// <summary>
        /// 将图片放大至200%并保存到指定目录
        /// </summary>
        public static bool EnlargeTo200Percent(string inputPath, string outputDirectory)
        {
            try  {
                Mat input = Cv2.ImRead(inputPath);
                if (input.Empty()) return false;

                Mat enlarged = new Mat();
                Cv2.Resize(input, enlarged, new Size(input.Width * 2, input.Height * 2));

                string outputPath = GetModifiedFilePath(inputPath, outputDirectory, "_enlarged");
                Cv2.ImWrite(outputPath, enlarged);
                return true;
            } catch (Exception ex) {
                return false;
            }
        }

        /// <summary>
        /// 将图片缩小至50%并保存到指定目录
        /// </summary>
        public static bool ShrinkTo50Percent(string inputPath, string outputDirectory)
        {
            try {
                Mat input = Cv2.ImRead(inputPath);
                if (input.Empty()) return false;

                Mat shrunk = new Mat();
                Cv2.Resize(input, shrunk, new Size(input.Width / 2, input.Height / 2));

                string outputPath = GetModifiedFilePath(inputPath, outputDirectory, "_shrunk");
                Cv2.ImWrite(outputPath, shrunk);

                Console.WriteLine($"缩小处理完成，保存至：{outputPath}");
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"缩小处理失败：{ex.Message}");
                return false;
            }
        }

        /// <summary>
        /// 顺时针旋转90°并保存到指定目录
        /// </summary>
        public static bool RotateClockwise90(string inputPath, string outputDirectory)
        {
            try
            {
                Mat input = Cv2.ImRead(inputPath);
                if (input.Empty()) return false;

                Mat rotated = new Mat();
                Cv2.Rotate(input, rotated, RotateFlags.Rotate90Clockwise);

                string outputPath = GetModifiedFilePath(inputPath, outputDirectory, "_rotated_cw");
                Cv2.ImWrite(outputPath, rotated);
                return true;
            } catch (Exception ex) {
                return false;
            }
        }

        /// <summary>
        /// 逆时针旋转90°并保存到指定目录
        /// </summary>
        public static bool RotateCounterClockwise90(string inputPath, string outputDirectory)
        {
            try {
                Mat input = Cv2.ImRead(inputPath);
                if (input.Empty()) return false;

                Mat rotated = new Mat();
                Cv2.Rotate(input, rotated, RotateFlags.Rotate90Counterclockwise);

                string outputPath = GetModifiedFilePath(inputPath, outputDirectory, "_rotated_ccw");
                Cv2.ImWrite(outputPath, rotated);
                return true;
            } catch (Exception ex) {
                return false;
            }
        }

        /// <summary>
        /// 根据输入路径生成修改后的输出路径
        /// </summary>
        private static string GetModifiedFilePath(string inputPath, string outputDirectory, string suffix)
        {
            if (!Directory.Exists(outputDirectory)) Directory.CreateDirectory(outputDirectory);
            string fileNameWithoutExtension = Path.GetFileNameWithoutExtension(inputPath);
            string extension = Path.GetExtension(inputPath);
            string timestamp = DateTime.Now.ToString("yyyy-MM-dd-HH-mm-ss");
            string outputFileName = $"{fileNameWithoutExtension}{suffix}_{timestamp}{extension}";
            return Path.Combine(outputDirectory, outputFileName);
        }
    }
}
