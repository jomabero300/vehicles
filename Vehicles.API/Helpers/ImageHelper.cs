using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Vehicles.API.Helpers
{
    public class ImageHelper : IImageHelper
    {
        private readonly IWebHostEnvironment _env;

        public ImageHelper(IWebHostEnvironment env)
        {
            _env = env;
        }

        public async Task<string> DeleteImageAsync(string File, string Folder)
        {
            int start = File.LastIndexOf("/") + 1;

            var file2 = File.Substring(start, File.Length - start);

            file2 = Path.Combine(_env.WebRootPath, Folder, file2);

            if (System.IO.File.Exists(file2))
            {
                FileInfo fi = new FileInfo(file2);

                if (fi != null)
                {
                    System.IO.File.Delete(file2);
                    fi.Delete();
                }
            }

            return await Task.FromResult("Ok");

        }

        public string DeleteImageAsync(string File)
        {
            string file2 = Path.Combine(_env.WebRootPath, "Images", File);

            if (System.IO.File.Exists(file2))
            {
                FileInfo fi = new FileInfo(file2);

                if (fi != null)
                {
                    System.IO.File.Delete(file2);
                    fi.Delete();
                }
            }

            return "Ok";
        }

        public async Task<Guid> UploadImageAsync(IFormFile ProFile, string Folder)
        {
            Guid fileName = await UploadImage(ProFile, Folder);

            return fileName;
        }
        private async Task<Guid> UploadImage(IFormFile ProFile, string Folder, string fileName = "")
        {
            string FileName = string.Empty;
            //string url = "https://localhost:44372/";
            Guid name = Guid.NewGuid();

            string[] exten = { ".png", ".jpg", ".jpeg", ".gif", ".bpm" };

            if (ProFile != null)
            {
                string uploadsFolder = Path.Combine(_env.WebRootPath,"Images", Folder);
                //Folder = Folder.Replace('\\', '/');

                string FileExt = Path.GetExtension(ProFile.FileName);
                int respue = Array.IndexOf(exten, FileExt);

                if (respue > -1)
                {
                    FileExt = ".png";
                }
                else
                {
                    FileExt = ".pdf";
                }
                

                FileName = fileName.Trim() == "" ? (name.ToString() + FileExt) : fileName.Trim();

                string filePath = Path.Combine(uploadsFolder, FileName);

                try
                {
                    using (var fileStream = new FileStream(filePath, FileMode.Create))
                    {
                        await ProFile.CopyToAsync(fileStream);
                    }
                }
                catch (Exception ex)
                {
                    filePath = ex.Message;
                }
            }

            return name;
        }
    }
}
