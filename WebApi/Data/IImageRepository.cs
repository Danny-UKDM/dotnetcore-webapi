﻿using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using WebApi.Models;

namespace WebApi.Data
{
    public interface IImageRepository
    {
        Task<ReadModelResult<byte[]>> GetImageAsync(Guid imageId);
        Task<CreateModelResult<IFormFile>> SaveImageAsync(IFormFile file, Guid imageId = default);
        Task<DeleteModelResult> DeleteImageAsync(Guid imageId);
    }
}