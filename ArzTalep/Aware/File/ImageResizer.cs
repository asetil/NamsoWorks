//using System;
//using System.Drawing;
//using System.Drawing.Drawing2D;
//using System.Drawing.Imaging;
//using System.IO;
//using Aware.Dependency;
//using Aware.Util;
//using Aware.Util.Log;

//namespace Aware.File
//{
//    public class ImageResizer : IImageResizer
//    {
//        public void ResizeImage(Stream stream, string path,int relationType)
//        {
//            if (stream != null && !string.IsNullOrEmpty(path) && relationType > 0)
//            {
//                if (relationType == (int)RelationTypes.Product)
//                {
//                    var image = Image.FromStream(stream);
//                    ResizeTo(image, path.Replace("Product\\", "Product\\s\\"), 80, 120);
//                    ResizeTo(image, path.Replace("Product\\", "Product\\m\\"), 480, 640);
//                    ResizeTo(image, path.Replace("Product\\", "Product\\l\\"), 640, 960);
//                }
//            }
//        }

//        private void ResizeTo(Image image, string saveDirectory, int width, int height)
//        {
//            try
//            {
//                if (image != null && !string.IsNullOrEmpty(saveDirectory) && width > 0 && height > 0)
//                {
//                    int sourceWidth = image.Width;
//                    int sourceHeight = image.Height;
//                    const int SOURCE_X = 0;
//                    const int SOURCE_Y = 0;
//                    int destX = 0;
//                    int destY = 0;

//                    float nPercent;

//                    float nPercentW = (width / (float)sourceWidth);
//                    float nPercentH = (height / (float)sourceHeight);

//                    if (nPercentH < nPercentW)
//                    {
//                        nPercent = nPercentH;
//                        destX = (int)((width - (sourceWidth * nPercent)) / 2);
//                    }
//                    else
//                    {
//                        nPercent = nPercentW;
//                        destY = (int)((height - (sourceHeight * nPercent)) / 2);
//                    }

//                    int destWidth = (int)(sourceWidth * nPercent);
//                    int destHeight = (int)(sourceHeight * nPercent);

//                    Bitmap bitmap = new Bitmap(width, height, PixelFormat.Format24bppRgb);
//                    bitmap.SetResolution(image.HorizontalResolution, image.VerticalResolution);

//                    Graphics graphics = Graphics.FromImage(bitmap);
//                    graphics.Clear(Color.White);
//                    graphics.InterpolationMode = InterpolationMode.HighQualityBicubic;
//                    graphics.DrawImage(image, new Rectangle(destX, destY, destWidth, destHeight), new Rectangle(SOURCE_X, SOURCE_Y, sourceWidth, sourceHeight), GraphicsUnit.Pixel);
//                    graphics.Dispose();

//                    bitmap.Save(saveDirectory, ImageFormat.Png);
//                    bitmap.Dispose();
//                }
//            }
//            catch (Exception ex)
//            {
//                var logger = WindsorBootstrapper.Resolve<ILogger>();
//                logger.Error("ImageResizer > ResizeTo - failed", ex);
//            }
//        }

//        private void ResizeTo(string saveDirectory)
//        {
//            var image = Image.FromFile("FilePath");
//            int newwidthimg = 160;
//            float AspectRatio = (float)image.Size.Width / (float)image.Size.Height;
//            int newHeight = Convert.ToInt32(newwidthimg / AspectRatio);
//            Bitmap thumbnailBitmap = new Bitmap(newwidthimg, newHeight);
//            Graphics thumbnailGraph = Graphics.FromImage(thumbnailBitmap);
//            thumbnailGraph.CompositingQuality = CompositingQuality.HighQuality;
//            thumbnailGraph.SmoothingMode = SmoothingMode.HighQuality;
//            thumbnailGraph.InterpolationMode = InterpolationMode.HighQualityBicubic;
//            var imageRectangle = new Rectangle(0, 0, newwidthimg, newHeight);
//            thumbnailGraph.DrawImage(image, imageRectangle);
//            thumbnailBitmap.Save(saveDirectory, ImageFormat.Jpeg);
//            thumbnailGraph.Dispose();
//            thumbnailBitmap.Dispose();
//            image.Dispose();
//        }
//    }
//}