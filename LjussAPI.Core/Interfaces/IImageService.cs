using Ljuss.Core.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Ljuss.Core.Domian;

namespace LjussAPI.Application.Interfaces
{
    public interface IImageService
    {
        public Task<Image> UploadImageAsync(ImageUploadRequestDto imageUpload, string userId);
        public Task<Image> UpdateImageAsync(Guid imageId, ImageUploadRequestDto imageUpload);
        public Task<bool> DeleteImageAsync(Guid imageId);
        public Task<List<Image>> GetImagesByOwnerAsync(Guid ownerId);
        public Task<Image> GetImageByIdAsync(Guid imageId);
    }
}
