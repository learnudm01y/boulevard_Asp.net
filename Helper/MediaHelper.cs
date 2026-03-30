using System;
using System.Collections.Generic;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Web;

namespace Boulevard.Helper
{
    public class MediaHelper
    {

        /// <summary>
        /// Original File Upload
        /// </summary>
        /// <param name="sourceImage"></param>
        /// <param name="urlPath"></param>
        /// <returns></returns>
        public static string UploadOriginalFile(HttpPostedFileBase sourceImage, string urlPath)
        {
            string filename = string.Empty;
            System.Drawing.Image souimage =
                System.Drawing.Image.FromStream(sourceImage.InputStream);
            filename = UploadFile(souimage, urlPath);

            return filename;
        }

        /// <summary>
        /// Resize Image Upload
        /// You can customized Image Size
        /// Large Size : Width-478 , Height-595 
        /// Medium Size : Width-368 , Height-349
        /// Medium Size : Width-208 , Height-183
        /// </summary>
        /// <param name="sourceImage"></param>
        /// <param name="urlPath"></param>
        /// <param name="width"></param>
        /// <param name="height"></param>
        /// <returns></returns>
        public static string UploadResizeFile(HttpPostedFileBase sourceImage, string urlPath, int width = 478, int height = 595)
        {
            string filename = string.Empty;
            System.Drawing.Image souimage =
                System.Drawing.Image.FromStream(sourceImage.InputStream);
            Image image = ResizeImageOriginalRatio(souimage, width, height);
            filename = UploadFile(image, urlPath);

            return filename;
        }

        /// <summary>
        /// Upload
        /// </summary>
        /// <param name="image"></param>
        /// <param name="urlPath"></param>
        /// <returns></returns>
        public static string UploadFile(Image image, string urlPath)
        {
            string filename = string.Empty;
            filename = GetFileName();
            var baseUrl = HttpRuntime.AppDomainAppPath;
            string MonthDate = DateTime.UtcNow.ToString("MMMM-yyyy");
            string customUrl = urlPath + "/" + MonthDate + "/";
            var filePath = baseUrl + customUrl;
            if (!Directory.Exists(filePath))
            {
                DirectoryInfo di = Directory.CreateDirectory(filePath);
            }
            filePath += filename;
            image.Save(filePath);
            filename = customUrl + filename;
            return filename;
        }

        /// <summary>
        /// Resize Image
        /// </summary>
        /// <param name="image"></param>
        /// <param name="width"></param>
        /// <param name="height"></param>
        /// <returns></returns>
        public static Image ResizeImageOriginalRatio(Image image, int width, int height)
        {


            var destRect = new Rectangle(0, 0, width, height);
            var destImage = new Bitmap(width, height);

            destImage.SetResolution(image.HorizontalResolution, image.VerticalResolution);

            using (var graphics = Graphics.FromImage(destImage))
            {
                graphics.CompositingMode = CompositingMode.SourceCopy;
                graphics.CompositingQuality = CompositingQuality.HighQuality;
                graphics.InterpolationMode = InterpolationMode.HighQualityBicubic;
                graphics.SmoothingMode = SmoothingMode.HighQuality;
                graphics.PixelOffsetMode = PixelOffsetMode.HighQuality;

                using (var wrapMode = new ImageAttributes())
                {
                    wrapMode.SetWrapMode(WrapMode.TileFlipXY);
                    graphics.DrawImage(image, destRect, 0, 0, image.Width, image.Height, GraphicsUnit.Pixel, wrapMode);
                }
            }

            return destImage;
        }

        public static string UploadImage(HttpPostedFileBase sourceImage, string urlPath)
        {
            System.Drawing.Image image =
                System.Drawing.Image.FromStream(sourceImage.InputStream);
            string filename = string.Empty;
            filename = GetFileName();
            var baseUrl = HttpRuntime.AppDomainAppPath;
            string MonthDate = DateTime.UtcNow.ToString("MMMM-yyyy");
            string customUrl = urlPath + "/" + MonthDate + "/";
            var filePath = baseUrl + customUrl;
            if (!Directory.Exists(filePath))
            {
                DirectoryInfo di = Directory.CreateDirectory(filePath);
            }
            filePath += filename;
            image.Save(filePath);
            filename = customUrl + filename;
            return filename;
        }

        private static string GetFileName()
        {
            string extension = ".jpg";
            string fileName = Path.ChangeExtension(
                Path.GetRandomFileName(),
                extension
            );
            return fileName;
        }

        /// <summary>
        /// Video File Upload
        /// </summary>
        /// <param name="image"></param>
        /// <param name="urlPath"></param>
        /// <returns></returns>
        public static string UploadVideoFile(HttpPostedFileBase image, string urlPath)
        {
            string filename = string.Empty;

            filename = GetVideoFileName();
            var baseUrl = HttpRuntime.AppDomainAppPath;
            string MonthDate = DateTime.UtcNow.ToString("MMMM-yyyy");
            string customUrl = urlPath + "/" + MonthDate + "/";
            var filePath = baseUrl + customUrl;
            if (!Directory.Exists(filePath))
            {
                DirectoryInfo di = Directory.CreateDirectory(filePath);
            }
            filePath += filename;
            image.SaveAs(filePath);
            filename = customUrl + filename;

            return filename;
        }

        private static string GetVideoFileName()
        {
            string extension = ".mp4";
            string fileName = Path.ChangeExtension(
                Path.GetRandomFileName(),
                extension
            );
            return fileName;
        }

        public static string UploadFileImport(HttpPostedFileBase file, string urlPath)
        {
            string filename = string.Empty;

            FileInfo fi = new FileInfo(file.FileName);
            string extension = fi.Extension;

            filename = GetImportFileName(extension);
            var baseUrl = HttpRuntime.AppDomainAppPath;
            string MonthDate = DateTime.UtcNow.ToString("MMMM-yyyy");
            string customUrl = urlPath + "/" + MonthDate + "/";
            var filePath = baseUrl + customUrl;
            if (!Directory.Exists(filePath))
            {
                DirectoryInfo di = Directory.CreateDirectory(filePath);
            }
            filePath += filename;
            file.SaveAs(filePath);
            filename = customUrl + filename;

            return filename;
        }
        private static string GetImportFileName(string extension)
        {
            string fileName = Path.ChangeExtension(
                Path.GetRandomFileName(),
                extension
            );
            return fileName;
        }

        public static bool HasImageExtension(string source)
        {
            return (source.EndsWith(".png") || source.EndsWith(".jpg") || source.EndsWith(".jpeg") || source.EndsWith(".gif"));
        }

        public static void ImageRemove(string imagePath)
        {
            var filePath = HttpContext.Current.Server.MapPath(imagePath);
            if (File.Exists(filePath))
            {
                File.Delete(filePath);
            }
        }

        public static string UploadRequestFile(HttpPostedFile sourceImage, string urlPath)
        {
            string filename = string.Empty;
            System.Drawing.Image souimage =
                System.Drawing.Image.FromStream(sourceImage.InputStream);
            Image image = ResizeImageOriginalRatio(souimage, 478, 595);
            filename = UploadFile(image, urlPath);
            return filename;
        }

        public static string UploadLargeFile(HttpPostedFileBase sourceImage, string urlPath)
        {
            string filename = string.Empty;
            System.Drawing.Image souimage =
                System.Drawing.Image.FromStream(sourceImage.InputStream);
            Image image = ResizeImageOriginalRatio(souimage, 478, 595);
            filename = UploadFile(image, urlPath);

            return filename;
        }

        public static string UploadMediumFile(HttpPostedFileBase sourceImage, string urlPath)
        {
            string filename = string.Empty;
            System.Drawing.Image souimage =
                System.Drawing.Image.FromStream(sourceImage.InputStream);
            Image image = ResizeImageOriginalRatio(souimage, 368, 349);
            filename = UploadFile(image, urlPath);
            return filename;
        }
    }
}