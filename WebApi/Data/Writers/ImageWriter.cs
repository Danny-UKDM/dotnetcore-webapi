using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using WebApi.Models;

namespace WebApi.Data.Writers
{
    public class ImageWriter : IImageWriter
    {
        public async Task<string> UploadImage(IFormFile file)
        {
            if (ValidateImageFile(file).Errors.Any())
            {
                return "Invalid image file";
            }

            return await WriteImageFile(file);
        }

        public ImageModelResult ValidateImageFile(IFormFile file)
        {
            var modelResult = new ImageModelResult { Errors = new List<ImageModelError>() };

            if (file.Length == 0)
                modelResult.Errors.Add(new ImageModelError { Error = "File size is zero" });
        }

        public async Task<string> WriteImageFile(IFormFile file)
        {
            string fileId;
            try
            {
                var extension = "." + file.FileName.Split('.')[file.FileName.Split('.').Length - 1];
                fileId = Guid.NewGuid().ToString() + extension;
                var path = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot\\images", fileId);

                using (var bits = new FileStream(path, FileMode.Create))
                {
                    await file.CopyToAsync(bits);
                }
            }
            catch (Exception e)
            {
                return e.Message;
            }

            return fileId;
        }
    }
}
