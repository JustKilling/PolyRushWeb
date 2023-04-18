using System;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Net;
using System.Net.Mime;
using System.Text;
using FluentFTP;
using Microsoft.AspNetCore.Mvc.Formatters;
using Microsoft.VisualBasic.CompilerServices;
using Newtonsoft.Json;
using Encoder = System.Drawing.Imaging.Encoder;

namespace PolyRushWeb.Helper
{
    public static class ImageHelper
    {
        //convert image to a base64string
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
        public static void SavePNG(this Bitmap bmp, string filename)
        {
            //resize bitmap to 500,500
            Bitmap resized = new(bmp, new Size(500, 500));
            //make sure its the best quality
            EncoderParameters encoderParameters = new(1);
            encoderParameters.Param[0] = new EncoderParameter(Encoder.Quality, 100L);
            //save the image locally
            resized.Save(filename, GetEncoder(ImageFormat.Png), encoderParameters);
        }
        
        public static async Task UploadAvatar(string base64Image, int userId)
        {
            HttpClientHandler clientHandler = new HttpClientHandler();
            clientHandler.ServerCertificateCustomValidationCallback = (sender, cert, chain, sslPolicyErrors) => { return true; };
            //"using" so it gets disposed so no connection stays open.
            using HttpClient client = new HttpClient(clientHandler);
            string jsonObj = JsonConvert.SerializeObject(new UserAvatar { ID = userId, Image = base64Image });
            HttpRequestMessage request = new(HttpMethod.Post, $"https://localhost/gip");
            request.Content = new StringContent(jsonObj, Encoding.UTF8, "application/json");
            var response = await client.SendAsync(request);
         
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
public class UserAvatar
{
    public int ID { get; set; }
    public string Image { get; set; }
}