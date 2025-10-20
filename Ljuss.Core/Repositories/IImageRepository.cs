using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Ljuss.Core.Entities;

namespace Ljuss.Core.Repositories
{
    public interface IImageRepository
    {
        public Task<Image> GetImageByIdAsync(Guid imageId);
        public Task<List<Image>> GetImagesByOwnerIdAsync(Guid ownerId);
        public Task AddImageAsync(Image image);
        public Task DeleteImageAsync(Guid imageId);
        public Task UpdateImageAsync(Image image);
    }
}
