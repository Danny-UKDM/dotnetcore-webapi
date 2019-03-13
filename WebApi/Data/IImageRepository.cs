using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using WebApi.Models;

namespace WebApi.Data
{
    public interface IImageRepository
    {
        Task<ModelResult<byte[]>> GetImageAsync(string imageId);
        Task<ModelResult<IFormFile>> SaveImageAsync(IFormFile file);
        Task<ModelResult<IFormFile>> DeleteImageAsync(string key);
    }
}
