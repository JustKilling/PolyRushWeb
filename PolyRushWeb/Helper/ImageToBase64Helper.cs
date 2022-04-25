using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;

namespace PolyRushWeb.Helper
{
    public static class ImageToBase64Helper
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
            using (MemoryStream myMemStream = new(imageData))
            {
                Image fullsizeImage = Image.FromStream(myMemStream);
                if (width <= 0 || width > fullsizeImage.Width)
                    width = fullsizeImage.Width;
                if (height <= 0 || height > fullsizeImage.Height)
                    height = fullsizeImage.Height;
                Image newImage = fullsizeImage.GetThumbnailImage(width, height, null, IntPtr.Zero);
                using (MemoryStream myResult = new())
                {
                    newImage.Save(myResult, type);
                    return myResult.ToArray();
                }
            }
        }

    }
}