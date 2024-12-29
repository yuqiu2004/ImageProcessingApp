using OpenCvSharp;
using System.IO;

namespace ImageProcessingApp.Utils
{
    public static class ImageProcessor
    {
        /// <summary>
        /// 将图片转换为灰度图并保存到指定目录
        /// </summary>
        public static string ConvertToGray(string inputPath, string outputDirectory)
        {
            try {
                Mat input = Cv2.ImRead(inputPath);
                if (input.Empty()) return "";
                Mat gray = new Mat();
                Cv2.CvtColor(input, gray, ColorConversionCodes.BGR2GRAY);
                string outputPath = GetModifiedFilePath(inputPath, outputDirectory, "_gray");
                Cv2.ImWrite(outputPath, gray);
                return outputPath;
            } catch (Exception ex) {
                return "";
            }
        }

        /// <summary>
        /// 将图片放大至200%并保存到指定目录
        /// </summary>
        public static string EnlargeTo200Percent(string inputPath, string outputDirectory)
        {
            try  {
                Mat input = Cv2.ImRead(inputPath);
                if (input.Empty()) return "";
                Mat enlarged = new Mat();
                Cv2.Resize(input, enlarged, new Size(input.Width * 2, input.Height * 2));
                string outputPath = GetModifiedFilePath(inputPath, outputDirectory, "_enlarged");
                Cv2.ImWrite(outputPath, enlarged);
                return outputPath;
            } catch (Exception ex) {
                return "";
            }
        }

        /// <summary>
        /// 将图片缩小至50%并保存到指定目录
        /// </summary>
        public static string ShrinkTo50Percent(string inputPath, string outputDirectory)
        {
            try {
                Mat input = Cv2.ImRead(inputPath);
                if (input.Empty()) return "";
                Mat shrunk = new Mat();
                Cv2.Resize(input, shrunk, new Size(input.Width / 2, input.Height / 2));
                string outputPath = GetModifiedFilePath(inputPath, outputDirectory, "_shrunk");
                Cv2.ImWrite(outputPath, shrunk);
                return outputPath;
            } catch (Exception ex) {
                return "";
            }
        }

        /// <summary>
        /// 顺时针旋转90°并保存到指定目录
        /// </summary>
        public static string RotateClockwise90(string inputPath, string outputDirectory)
        {
            try
            {
                Mat input = Cv2.ImRead(inputPath);
                if (input.Empty()) return "";
                Mat rotated = new Mat();
                Cv2.Rotate(input, rotated, RotateFlags.Rotate90Clockwise);
                string outputPath = GetModifiedFilePath(inputPath, outputDirectory, "_rotated_cw");
                Cv2.ImWrite(outputPath, rotated);
                return outputPath;
            } catch (Exception ex) {
                return "";
            }
        }

        /// <summary>
        /// 逆时针旋转90°并保存到指定目录
        /// </summary>
        public static string RotateCounterClockwise90(string inputPath, string outputDirectory)
        {
            try {
                Mat input = Cv2.ImRead(inputPath);
                if (input.Empty()) return "";
                Mat rotated = new Mat();
                Cv2.Rotate(input, rotated, RotateFlags.Rotate90Counterclockwise);
                string outputPath = GetModifiedFilePath(inputPath, outputDirectory, "_rotated_ccw");
                Cv2.ImWrite(outputPath, rotated);
                return outputPath;
            } catch (Exception ex) {
                return "";
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

        /// <summary>
        /// 对图片应用高斯模糊并保存到指定目录
        /// </summary>
        public static string ApplyGaussianBlur(string inputPath, string outputDirectory)
        {
            try
            {
                Mat input = Cv2.ImRead(inputPath);
                if (input.Empty()) return "";
                Mat blurred = new Mat();
                Cv2.GaussianBlur(input, blurred, new Size(15, 15), 0); // 内核大小和标准差可调整
                string outputPath = GetModifiedFilePath(inputPath, outputDirectory, "_gaussian");
                Cv2.ImWrite(outputPath, blurred);
                return outputPath;
            }
            catch (Exception ex)
            {
                return "";
            }
        }

        /// <summary>
        /// 对图片执行边缘检测并保存到指定目录
        /// </summary>
        public static string ApplyEdgeDetection(string inputPath, string outputDirectory)
        {
            try
            {
                Mat input = Cv2.ImRead(inputPath);
                if (input.Empty()) return "";
                Mat gray = new Mat();
                Cv2.CvtColor(input, gray, ColorConversionCodes.BGR2GRAY); // 边缘检测前需灰度化
                Mat edges = new Mat();
                Cv2.Canny(gray, edges, 100, 200); // 低阈值和高阈值可调整
                string outputPath = GetModifiedFilePath(inputPath, outputDirectory, "_edges");
                Cv2.ImWrite(outputPath, edges);
                return outputPath;
            }
            catch (Exception ex)
            {
                return "";
            }
        }

        /// <summary>
        /// 增强图片对比度（直方图均衡化）并保存到指定目录
        /// </summary>
        public static string EnhanceContrast(string inputPath, string outputDirectory)
        {
            try
            {
                Mat input = Cv2.ImRead(inputPath);
                if (input.Empty()) return "";

                // 转为灰度图
                Mat gray = new Mat();
                Cv2.CvtColor(input, gray, ColorConversionCodes.BGR2GRAY);

                // 应用直方图均衡化
                Mat enhanced = new Mat();
                Cv2.EqualizeHist(gray, enhanced);

                string outputPath = GetModifiedFilePath(inputPath, outputDirectory, "_enhanced");
                Cv2.ImWrite(outputPath, enhanced);

                return outputPath;
            }
            catch (Exception ex)
            {
                return "";
            }
        }

    }
}
