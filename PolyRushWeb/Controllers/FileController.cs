using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using PolyRushWeb.DA;

namespace PolyRushWeb.Controllers
{
    public class FileController : Controller
    {
        private readonly IWebHostEnvironment _appEnvironment;

        public FileController(IWebHostEnvironment appEnvironment)
        {
            _appEnvironment = appEnvironment;
        }

        // GET
        public IActionResult Index()
        {
            return Content("");
        }
        //method to get the user image via a link, so the pages load faster.
        /*public async Task<IActionResult> GetUserImage([FromQuery] int UserID)
        {
            IActionResult result = null;
            
            DateTime date = DateTime.Parse("2022-04-18");
            string strDate = date.ToString("yyyyMMdd");
            string rootPath = Path.Combine(_appEnvironment.ContentRootPath, "wwwroot");
            string dirPath = Path.Combine(rootPath, "img", "userimages", strDate);
            //if dir doesn't exist, create it
            if (!Directory.Exists(dirPath))
            {
                Directory.CreateDirectory(dirPath);
            }
            //remove images from other dates
            await RemoveOldImages(strDate);

            //path to the image
            string imgPath = Path.Combine(dirPath, UserID + "-" + strDate + ".jpg");

            byte[] imageBytes;
            //if image exists, return it
            if (System.IO.File.Exists(imgPath))
            {
                imageBytes = await System.IO.File.ReadAllBytesAsync(imgPath);
                result = base.File(imageBytes, "image/jpeg");
            }
            //else get it from the database and save it in the dir
            imageBytes = Convert.FromBase64String(UserDA.GetAvatarByID(UserID).Result);
            System.IO.File.WriteAllBytesAsync(imgPath, imageBytes);
            result = base.File(imageBytes, "image/jpeg");

            return result;

        }

        private async Task RemoveOldImages(string except)
        {
            Task.Run(() =>
            {
                string rootPath = Path.Combine(_appEnvironment.ContentRootPath, "wwwroot");
                string dirPath = Path.Combine(rootPath, "img", "userimages");
                //foreach dir that isn't from today
                foreach (string item in Directory.GetDirectories(dirPath))
                {
                    if (item.Contains(except)) continue;
                    //delete every file, then the dir
                    foreach (FileInfo fileInfo in new DirectoryInfo(item).EnumerateFiles())
                    {
                        fileInfo.Delete();
                    }
                    Directory.Delete(item);
                }
            });
           
        }*/
    }
}