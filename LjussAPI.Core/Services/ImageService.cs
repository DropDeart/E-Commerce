using Ljuss.Core.Domian;
using Ljuss.Applications.DTOs;
using Ljuss.Core.Repositories;
using LjussAPI.Application.Interfaces;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LjussAPI.Application.Services
{
    public class ImageService : IImageService
    {
        private readonly IImageRepository _imageRepository;
        private readonly IAuthService _authService;
        public ImageService(IImageRepository imageRepository, IAuthService authService)
        {
            _imageRepository = imageRepository;
            _authService = authService;
        }
        public async Task<Image> UploadImageAsync(ImageUploadRequestDto imageUpload, string userId)
        {
            try
            {
                var filePath = await SaveImageFileAsync(imageUpload.File);

                var image = new Image
                {
                    Id = Guid.NewGuid(),
                    FileName = imageUpload.FileName,
                    FileDescription = imageUpload.FileDescription,
                    FileExtension = Path.GetExtension(imageUpload.File.FileName),
                    FileSizeInBytes = imageUpload.File.Length,
                    FilePath = filePath,
                    OwnerID = userId,
                    Type = imageUpload.Type,
                };

                await _imageRepository.AddImageAsync(image);

                return image;
            }
            catch (Exception ex)
            {
                throw new ApplicationException("Resim yüklenirken bir hata oluştu.");
            }
        }
        public async Task<Image> UpdateImageAsync(Guid imageId, ImageUploadRequestDto imageUpload)
        {
            try
            {
                var existingImage = await _imageRepository.GetImageByIdAsync(imageId);
                if (existingImage == null)
                {
                    throw new KeyNotFoundException("Güncellenecek resim bulunamadı.");
                }

                if (imageUpload.File != null)
                {
                    DeleteImageFile(existingImage.FilePath);
                    existingImage.FilePath = await SaveImageFileAsync(imageUpload.File);
                    existingImage.FileSizeInBytes = imageUpload.File.Length;
                    existingImage.FileExtension = Path.GetExtension(imageUpload.File.FileName);
                }

                existingImage.FileName = imageUpload.FileName;
                existingImage.FileDescription = imageUpload.FileDescription;

                await _imageRepository.UpdateImageAsync(existingImage);

                return existingImage;
            }
            catch (Exception ex)
            {
                throw new ApplicationException("Resim güncellenirken bir hata oluştu.", ex);
            }
        }

        public async Task<bool> DeleteImageAsync(Guid imageId)
        {
            try
            {
                var image = await _imageRepository.GetImageByIdAsync(imageId);
                if (image == null)
                {
                    throw new KeyNotFoundException("Silinecek resim bulunamadı.");
                }

                if (File.Exists(image.FilePath))
                {
                    File.Delete(image.FilePath);
                }

                await _imageRepository.DeleteImageAsync(imageId);

                return true;
            }
            catch (Exception ex)
            {
                throw new ApplicationException("Resim silinirken bir hata oluştu.", ex);
            }
        }

        public async Task<List<Image>> GetImagesByOwnerAsync(Guid ownerId)
        {
            try
            {
                var images = await _imageRepository.GetImagesByOwnerIdAsync(ownerId);

                return images ?? new List<Image>();
            }
            catch (Exception ex)
            {
                throw new ApplicationException("Resimler getirilirken bir hata oluştu.", ex);
            }
        }

        private async Task<string> SaveImageFileAsync(IFormFile file)
        {
            try
            {
                var uploadFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads");
                if (!Directory.Exists(uploadFolder))
                {
                    Directory.CreateDirectory(uploadFolder);
                }

                var uniqueFileName = $"{Guid.NewGuid()}{Path.GetExtension(file.FileName)}";
                var filePath = Path.Combine(uploadFolder, uniqueFileName);

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await file.CopyToAsync(stream);
                }

                return filePath;
            }
            catch (Exception ex)
            {
                throw new ApplicationException("Dosya kaydedilirken bir hata oluştu.");
            }
        }
        private void DeleteImageFile(string filePath)
        {
            try
            {
                if (File.Exists(filePath))
                {
                    File.Delete(filePath);
                }
            }
            catch (Exception ex)
            {
                throw new ApplicationException("Dosya silinirken bir hata oluştu.", ex);
            }
        }

        public Task<Image> GetImageByIdAsync(Guid imageId)
        {
            try
            {
                if(imageId == Guid.Empty)
                {
                    throw new ApplicationException("Hatalı resim id !");
                }
                var image = _imageRepository.GetImageByIdAsync(imageId);

                return image;
            }
            catch(Exception ex)
            {
                throw new ApplicationException("Resim bulunamadı");
            }
        }
    }
}
