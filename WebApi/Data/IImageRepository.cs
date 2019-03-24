using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using WebApi.Models;

namespace WebApi.Data
{
    public interface IImageRepository
    {
        Task<ModelResult<byte[]>> GetImageAsync(Guid imageId);
        Task<ModelResult<IFormFile>> SaveImageAsync(IFormFile file, Guid imageId = default);
        Task<ModelResult<object>> DeleteImageAsync(Guid imageId);
    }
}
