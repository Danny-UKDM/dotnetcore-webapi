using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using WebApi.Models;

namespace WebApi.Data.Writers
{
    public interface IImageRepository
    {
        Task<ModelResult<IFormFile>> GetImageAsync(Guid key);
        Task<ModelResult<IFormFile>> SaveImageAsync(IFormFile file);
        Task<ModelResult<IFormFile>> DeleteImageAsync(Guid key);
    }
}
