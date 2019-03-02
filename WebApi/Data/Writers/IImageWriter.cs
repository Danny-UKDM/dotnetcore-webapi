using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using WebApi.Models;

namespace WebApi.Data.Writers
{
    public interface IImageWriter
    {
        Task<ModelResult<IFormFile>> UploadImage(IFormFile file);
    }
}
