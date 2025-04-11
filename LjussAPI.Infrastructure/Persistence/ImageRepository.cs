using Ljuss.Core.Domian;
using Ljuss.Core.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using NLog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Net.Mime.MediaTypeNames;
using Image = Ljuss.Core.Domian.Image;

namespace LjussAPI.Infrastructure.Persistence
{
    public class ImageRepository : IImageRepository
    {
        private readonly AppDbContext _appDbContext;
        private readonly ILogger<ImageRepository> _logger;
        public ImageRepository(AppDbContext appDbContext, ILogger<ImageRepository> logger)

        {
            _appDbContext = appDbContext;
            _logger = logger;
        }

        public async Task<Image> GetImageByIdAsync(Guid imageId)
        {
            try
            {
                if (imageId == Guid.Empty)
                {
                    throw new ArgumentException("Image Id cannot be null or empty", nameof(imageId));
                }

                var image = await _appDbContext.Images.FirstOrDefaultAsync(p => p.Id == imageId);

                return image;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while getting image by id.");
                throw new Exception("Something went wrong!");
            }
        }

        public async Task<List<Image>> GetImagesByOwnerIdAsync(Guid ownerId)
        {
            try
            {
                if (ownerId == Guid.Empty)
                {
                    throw new ArgumentException("Image Id cannot be null or empty", nameof(ownerId));
                }

                var imageList = await _appDbContext.Images.Where(p => p.OwnerID == ownerId.ToString()).ToListAsync();
                return imageList;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while getting image by ownerid.");
                throw new Exception("Something went wrong!");
            }
            
        }

        public async Task AddImageAsync(Image image)
        {
            try
            {
                if (image == null)
                {
                    throw new ArgumentNullException(nameof(image), "Image cannot be null.");
                }

                await _appDbContext.Images.AddAsync(image);
                await _appDbContext.SaveChangesAsync();
            }

            catch (Exception ex)
            {
                _logger.LogError(ex, "Image does not saving");
                throw new Exception("Something went wrong!");
            }
        }

        public async Task UpdateImageAsync(Image image)
        {
            try
            {
                if (image == null)
                {
                    throw new ArgumentNullException(nameof(image), "Güncellenecek resim boş olamaz.");
                }

                _appDbContext.Images.Update(image);
                await _appDbContext.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Resim güncellenirken bir hata oluştu.");
                throw new Exception("Bir şeyler yanlış gitti!");
            }
        }

        public async Task DeleteImageAsync(Guid imageId)
        {
            try
            {
                if (imageId == Guid.Empty)
                {
                    throw new ArgumentNullException(nameof(imageId), "Image Id cannot be null.");
                }

                var image = await _appDbContext.Images.FindAsync(imageId);
                if (image != null)
                {
                    _appDbContext.Images.Remove(image);
                    await _appDbContext.SaveChangesAsync();
                }
            }

            catch (Exception ex)
            {
                _logger.LogError(ex, "Image does not deleting");
                throw new Exception("Something went wrong!");
            }
        }
    }
}
