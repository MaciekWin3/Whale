﻿using Whale.Models;
using Whale.Utils;

namespace Whale.Services
{
    public interface IDockerService
    {
        Task<Result<List<ContainerDTO>>> GetContainerListAsync(CancellationToken token = default);
        Task<Result<List<VolumeDTO>>> GetVolumeListAsync(CancellationToken token = default);
        Task<Result<List<ImageDTO>>> GetImageListAsync(CancellationToken token = default);
        Task<Result<T>> GetDockerObjectInfoAsync<T>(string id, CancellationToken token = default);
    }
}