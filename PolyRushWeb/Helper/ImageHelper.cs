using System;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Net;
using FluentFTP;
using Microsoft.VisualBasic.CompilerServices;

namespace PolyRushWeb.Helper
{
    public static class ImageHelper
    {
        public static string ConvertImagePathToBase64String(string path)
        {
            //read the file
            FileInfo fileInfo = new(path);
            byte[] imageData = new byte[fileInfo.Length];
            using FileStream fs = fileInfo.OpenRead();
            fs.Read(imageData, 0, imageData.Length);

            imageData = ReduceImageSize(imageData);
            
            return Convert.ToBase64String(imageData);
        }
        
        //method to reduce image size
        //https://yeahexp.com/how-to-decrease-byte-array-size-representing-an-image/
        public static byte[] ReduceImageSize(this byte[] imageData, int width = 500, int height = 500)
        {
            ImageFormat type = ImageFormat.Png;
            if (imageData.Length == 0)
                return imageData;
            using MemoryStream myMemStream = new(imageData);
            Image fullsizeImage = Image.FromStream(myMemStream);
            if (width <= 0 || width > fullsizeImage.Width)
                width = fullsizeImage.Width;
            if (height <= 0 || height > fullsizeImage.Height)
                height = fullsizeImage.Height;
            Image newImage = fullsizeImage.GetThumbnailImage(width, height, null, IntPtr.Zero);
            using MemoryStream myResult = new();
            newImage.Save(myResult, type);
            return myResult.ToArray();
        }
        //https://stackoverflow.com/questions/41665/bmp-to-jpg-png-in-c-sharp
        public static async Task SavePNG100Async(this Bitmap bmp, string filename)
        {
            
            //resize bitmap to 500,500
            Bitmap resized = new(bmp, new Size(500, 500));
            //make sure its the best quality
            EncoderParameters encoderParameters = new(1);
            encoderParameters.Param[0] = new EncoderParameter(Encoder.Quality, 100L);
            //save the image locally
            resized.Save(filename, GetEncoder(ImageFormat.Png), encoderParameters);

            //using var ftp = new FtpClient(host: "prjemiel.netwerkit.be", user: "ftpemieldelaey", pass: "48c/e:aZT]"); https://imgur.com/a/9CT2PZE
            //var imagepath = "ftp://prjemiel.netwerkit.be/wwwroot/user";
            //try
            //{
            //    await ftp.ConnectAsync();
            //    await ftp.UploadFileAsync(filename, imagepath, verifyOptions: FtpVerify.Retry);

            //}
            //catch (Exception e)
            //{
            //    Console.WriteLine(e);
            //    throw;
            //}
        }
        private static ImageCodecInfo GetEncoder(ImageFormat format)
        {
            ImageCodecInfo[] codecs = ImageCodecInfo.GetImageDecoders();

            foreach (ImageCodecInfo codec in codecs)
            {
                if (codec.FormatID == format.Guid)
                {
                    return codec;
                }
            }

            return null;
        }


    }
}