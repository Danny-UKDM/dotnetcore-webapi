using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using WebApi.Models;

namespace WebApi.Data.Writers
{
    public interface IImageWriter
    {
        Task<string> UploadImage(IFormFile file);
        ImageModelResult ValidateImageFile(IFormFile file);
        Task<string> WriteImageFile(IFormFile file);
    }
}
