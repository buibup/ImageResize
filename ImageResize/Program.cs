using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImageResize
{
    public static class Program
    {
        static string sourcePath = @"E:\images\test.jpg";
        static string destinationPath = @"E:\images\resize\testResize.jpg";
        private static int allowedFileSizeInByte = 100_000;
        static void Main(string[] args)
        {
            //TestResizeImage();
            //TestResizeImageNew();
            //TestNDID();

            TestResizeImageSmall();
        }

        public static void TestNDID()
        {
            double lengthLimit = 100;
            long qualityLevel = 80L;
            long compressionLevel = 100L;
            Image img = Image.FromFile(@"E:\images\test.jpg");
            byte[] arr;
            using (MemoryStream ms = new MemoryStream())
            {
                img.Save(ms, ImageFormat.Jpeg);
                arr = ms.ToArray();
            }
            Console.WriteLine($"Start arr:{ConvertBytesToKilobytes(arr.LongLength)}");
            while (ConvertBytesToKilobytes(arr.LongLength) > lengthLimit)
            {
                arr = CompressImageQualityLevel(arr, qualityLevel, compressionLevel);

                qualityLevel = qualityLevel - 10;
                compressionLevel = compressionLevel - 10;
                Console.WriteLine($"qualityLevel:{qualityLevel}, compressionLevel:{compressionLevel}");
                Console.WriteLine($"In while arr:{ConvertBytesToKilobytes(arr.LongLength)}");
                System.Threading.Thread.Sleep(2000);
            }
            Console.WriteLine($"End arr:{ConvertBytesToKilobytes(arr.LongLength)}");
            Console.ReadLine();
        }

        public static void TestResizeImage()
        {
            using (MemoryStream ms = new MemoryStream())
            {
                using (FileStream fs = new FileStream(sourcePath, FileMode.Open))
                {
                    Bitmap bmp = (Bitmap)Image.FromStream(fs);
                    SaveTemporary(bmp, ms, 50);

                    Console.WriteLine($"Start:{ms.ToArray().Length}");
                    while (ms.Length < 0.9 * allowedFileSizeInByte || ms.Length > allowedFileSizeInByte)
                    {
                        double scale = Math.Sqrt((double)allowedFileSizeInByte / (double)ms.Length);

                        Console.WriteLine($"In scale:{scale}");
                        ms.SetLength(0);
                        bmp = ScaleImage(bmp, scale);
                        Console.WriteLine($"In Width:{bmp.Width}");
                        SaveTemporary(bmp, ms, 50);

                        Console.WriteLine($"In while:{ms.ToArray().Length}");
                        System.Threading.Thread.Sleep(1000);
                    }

                    if (bmp != null)
                        bmp.Dispose();
                    //SaveImageToFile(ms);

                    Console.WriteLine($"End:{ms.ToArray().Length}");
                    Console.ReadLine();
                }
            }
        }

        public static void TestResizeImageNew()
        {
            using (MemoryStream ms = new MemoryStream())
            {
                using (FileStream fs = new FileStream(sourcePath, FileMode.Open))
                {
                    Bitmap bmp = (Bitmap)Image.FromStream(fs);
                    SaveTemporary(bmp, ms, 100);
                    Console.WriteLine($"Start:{ms.ToArray().Length}");

                    var result = ResizeImage(ms.ToArray());
                    Console.WriteLine($"result:{result.Length}");

                    Console.ReadLine();
                }
            }
        }

        public static void TestResizeImageSmall()
        {
            using (MemoryStream ms = new MemoryStream())
            {
                using (FileStream fs = new FileStream(sourcePath, FileMode.Open))
                {
                    Bitmap bmp = (Bitmap)Image.FromStream(fs);
                    SaveTemporary(bmp, ms, 100);
                    Console.WriteLine($"Start:{ms.ToArray().Length}");

                    var result = ResizeImage(ms.ToArray(), 600, 1200);
                    Console.WriteLine($"result:{result.Length}");
                    Console.ReadLine();
                }
            }
        }

        public static byte[] ResizeImage(byte[] data)
        {
            byte[] result = null;

            using(var ms = new MemoryStream(data))
            {
                Bitmap bmp = new Bitmap(ms);
                while (ms.Length < 0.9 * allowedFileSizeInByte || ms.Length > allowedFileSizeInByte)
                {
                    double scale = Math.Sqrt((double)allowedFileSizeInByte / (double)ms.Length);
                    ms.SetLength(0);
                    bmp = ScaleImage(bmp, scale);
                    SaveTemporary(bmp, ms, 100);

                    Console.WriteLine($"In while:{ms.ToArray().Length}");
                    System.Threading.Thread.Sleep(1000);
                }

                if (bmp != null)
                    bmp.Dispose();

                result = ms.ToArray();

            }

            return result;
        }

        public static void TestNew()
        {
            Image img = Image.FromFile(@"E:\images\test.jpg");
            Image newImage = img.GetThumbnailImage(img.Width, img.Height, null, IntPtr.Zero);
            byte[] arr;
            using (MemoryStream ms = new MemoryStream())
            {
                newImage.Save(ms, ImageFormat.Jpeg);
                arr = ms.ToArray();
            }
            Console.WriteLine($"Start arr:{ConvertBytesToKilobytes(arr.LongLength)}");

            MemoryStream myResult = new MemoryStream();
            newImage.Save(myResult, ImageFormat.Jpeg);

            Bitmap bmp;
            using (var ms = new MemoryStream(myResult.ToArray()))
            {
                bmp = new Bitmap(ms);
                while (ms.Length < 0.9 * allowedFileSizeInByte || ms.Length > allowedFileSizeInByte)
                {
                    double scale = Math.Sqrt(allowedFileSizeInByte / ms.Length);
                    ms.SetLength(0);
                    bmp = ScaleImage(bmp, scale);
                    SaveTemporary(bmp, ms, 100);

                    Console.Write($"In while:{ConvertBytesToKilobytes(ms.ToArray().Length)}");
                    System.Threading.Thread.Sleep(1000);
                }

                if (bmp != null)
                    bmp.Dispose();

                byte[] bytes = ms.ToArray();
                Console.WriteLine($"End arr:{ConvertBytesToKilobytes(bytes.Length)}");
            }
        }

        private static double ConvertBytesToKilobytes(long bytes)
        {
            return (bytes / 1024f);
        }

        public static byte[] Test()
        {
            Image img = Image.FromFile(@"E:\images\test.jpg");
            //byte[] arr;
            //using (MemoryStream ms = new MemoryStream())
            //{
            //    img.Save(ms, ImageFormat.Jpeg);
            //    arr = ms.ToArray();
            //}

            //var fullsizeImage = ByteArrayToImage(Constants.ImageByte);
            System.Drawing.Image newImage = img.GetThumbnailImage(img.Width, img.Height, null, IntPtr.Zero);
            System.IO.MemoryStream myResult = new System.IO.MemoryStream();
            newImage.Save(myResult, System.Drawing.Imaging.ImageFormat.Jpeg);  //Or whatever format you want.
            return myResult.ToArray();  //Returns a new byte array.
        }

        public static void ScaleImage()
        {
            Image img = Image.FromFile(@"E:\images\test.jpg");
            System.Drawing.Image newImage = img.GetThumbnailImage(img.Width, img.Height, null, IntPtr.Zero);

            System.IO.MemoryStream myResult = new System.IO.MemoryStream();
            newImage.Save(myResult, System.Drawing.Imaging.ImageFormat.Jpeg);

            Bitmap bmp;
            using (var ms = new MemoryStream(myResult.ToArray()))
            {
                bmp = new Bitmap(ms);
                while (ms.Length < 0.9 * allowedFileSizeInByte || ms.Length > allowedFileSizeInByte)
                {
                    double scale = Math.Sqrt(allowedFileSizeInByte / ms.Length);
                    ms.SetLength(0);
                    bmp = ScaleImage(bmp, scale);
                    SaveTemporary(bmp, ms, 100);
                }

                if (bmp != null)
                    bmp.Dispose();

                byte[] bytes = ms.ToArray();
                Console.WriteLine(bytes.Length);
            }
        }

        private static void SaveTemporary(Bitmap bmp, MemoryStream ms, int quality)
        {
            EncoderParameter qualityParam = new EncoderParameter(System.Drawing.Imaging.Encoder.Quality, quality);
            var codec = GetImageCodecInfo();
            var encoderParams = new EncoderParameters(1);
            encoderParams.Param[0] = qualityParam;

            if (codec != null)
                bmp.Save(ms, codec, encoderParams);
            else
                bmp.Save(ms, GetImageFormat());
        }

        private static ImageCodecInfo GetImageCodecInfo()
        {
            FileInfo fi = new FileInfo(sourcePath);

            switch (fi.Extension)
            {
                case ".bmp": return ImageCodecInfo.GetImageEncoders()[0];
                case ".jpg":
                case ".jpeg": return ImageCodecInfo.GetImageEncoders()[1];
                case ".gif": return ImageCodecInfo.GetImageEncoders()[2];
                case ".tiff": return ImageCodecInfo.GetImageEncoders()[3];
                case ".png": return ImageCodecInfo.GetImageEncoders()[4];
                default: return null;
            }
        }

        private static ImageFormat GetImageFormat()
        {
            FileInfo fi = new FileInfo(sourcePath);

            switch (fi.Extension)
            {
                case ".jpg": return ImageFormat.Jpeg;
                case ".bmp": return ImageFormat.Bmp;
                case ".gif": return ImageFormat.Gif;
                case ".png": return ImageFormat.Png;
                case ".tiff": return ImageFormat.Tiff;
                default: return ImageFormat.Png;
            }
        }

        public static byte[] ImageToByteArray(System.Drawing.Image imageIn)
        {
            MemoryStream ms = new MemoryStream();
            imageIn.Save(ms, System.Drawing.Imaging.ImageFormat.Gif);
            return ms.ToArray();
        }

        public static Image ByteArrayToImage(byte[] byteArrayIn)
        {
            MemoryStream ms = new MemoryStream(byteArrayIn);
            Image returnImage = Image.FromStream(ms);
            return returnImage;
        }

        public static Bitmap ScaleImage(Bitmap image, double scale)
        {
            int newWidth = (int)(image.Width * scale);
            int newHeight = (int)(image.Height * scale);

            Bitmap result = new Bitmap(newWidth, newHeight, PixelFormat.Format24bppRgb);
            result.SetResolution(image.HorizontalResolution, image.VerticalResolution);

            using (Graphics g = Graphics.FromImage(result))
            {
                g.InterpolationMode = InterpolationMode.HighQualityBicubic;
                g.CompositingQuality = CompositingQuality.HighQuality;
                g.SmoothingMode = SmoothingMode.HighQuality;
                g.PixelOffsetMode = PixelOffsetMode.HighQuality;

                g.DrawImage(image, 0, 0, result.Width, result.Height);
            }
            return result;
        }

        private static byte[] CompressImageQualityLevel(byte[] imageSource, long qualityLevel = 49L, long compressionLevel = 100L)
        {
            // Get a bitmap. The using statement ensures objects  
            // are automatically disposed from memory after use.  
            byte[] byteArray = imageSource;

            try
            {
                using (System.Drawing.Bitmap bmp1 = new System.Drawing.Bitmap(new System.IO.MemoryStream(imageSource)))
                {
                    System.Drawing.Imaging.ImageCodecInfo jpgEncoder = GetEncoder(System.Drawing.Imaging.ImageFormat.Jpeg);

                    // Create an Encoder object based on the GUID  
                    // for the Quality parameter category.  
                    System.Drawing.Imaging.Encoder myEncoderQuality = System.Drawing.Imaging.Encoder.Quality;
                    System.Drawing.Imaging.Encoder myEncoderCompression = System.Drawing.Imaging.Encoder.Compression;

                    // Create an EncoderParameters object.  
                    // An EncoderParameters object has an array of EncoderParameter  
                    // objects. In this case, there is only one  
                    // EncoderParameter object in the array.  
                    System.Drawing.Imaging.EncoderParameters myEncoderParameters = new System.Drawing.Imaging.EncoderParameters(1);

                    if (compressionLevel > 0L)
                    {
                        myEncoderParameters = new System.Drawing.Imaging.EncoderParameters(2);
                        System.Drawing.Imaging.EncoderParameter myEncoderParameter2 = new System.Drawing.Imaging.EncoderParameter(myEncoderCompression, compressionLevel);
                        myEncoderParameters.Param[1] = myEncoderParameter2;
                    }

                    System.Drawing.Imaging.EncoderParameter myEncoderParameter1 = new System.Drawing.Imaging.EncoderParameter(myEncoderQuality, qualityLevel);
                    myEncoderParameters.Param[0] = myEncoderParameter1;
                    //bmp1.Save(@"c:\TestPhotoQualityFifty.jpg", jpgEncoder, myEncoderParameters);

                    using (System.IO.MemoryStream memoryStream = new System.IO.MemoryStream())
                    {
                        bmp1.Save(memoryStream, jpgEncoder, myEncoderParameters);

                        byteArray = memoryStream.ToArray();
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return byteArray;
        }

        private static System.Drawing.Imaging.ImageCodecInfo GetEncoder(System.Drawing.Imaging.ImageFormat format)
        {
            System.Drawing.Imaging.ImageCodecInfo[] codecs = System.Drawing.Imaging.ImageCodecInfo.GetImageDecoders();
            foreach (System.Drawing.Imaging.ImageCodecInfo codec in codecs)
            {
                if (codec.FormatID == format.Guid)
                {
                    return codec;
                }
            }
            return null;
        }

        private static byte[] ResizeImage(byte[] imageData, int maxWidth = 600, int maxHeight = 1200)
        {
            byte[] result = null;
            try
            {
                using (MemoryStream ms = new MemoryStream(imageData, 0, imageData.Length))
                {
                    using (Image img = Image.FromStream(ms))
                    {
                        var size = new Size(img.Width, img.Height);
                        Console.WriteLine($"OriginSize -> w:{img.Width}, h:{img.Height}");
                        var resize = ResizeKeepAspect(size, maxWidth, maxHeight);
                        Console.WriteLine($"Resize -> w:{resize.Width}, h:{resize.Height}");
                        using (Bitmap b = new Bitmap(img, resize))
                        {
                            using (MemoryStream ms2 = new MemoryStream())
                            {
                                b.Save(ms2, ImageFormat.Jpeg);
                                result = ms2.ToArray();
                                SaveImageToFile(ms2);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {

            }

            return result;
        }

        public static Size ResizeKeepAspect(this Size src, int maxWidth, int maxHeight, bool enlarge = false)
        {
            maxWidth = enlarge ? maxWidth : Math.Min(maxWidth, src.Width);
            maxHeight = enlarge ? maxHeight : Math.Min(maxHeight, src.Height);

            decimal rnd = Math.Min(maxWidth / (decimal)src.Width, maxHeight / (decimal)src.Height);
            return new Size((int)Math.Round(src.Width * rnd), (int)Math.Round(src.Height * rnd));
        }

        private static void SaveImageToFile(MemoryStream ms)
        {
            byte[] data = ms.ToArray();

            using (FileStream fs = new FileStream(destinationPath, FileMode.Create))
            {
                fs.Write(data, 0, data.Length);
            }
        }
    }
}
