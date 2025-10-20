using Ljuss.Core.Entities;
using Ljuss.Core.Repositories;
using LjussAPI.Application.DTOs.CommentDTOs;
using LjussAPI.Application.DTOs.ImageDTOs;
using LjussAPI.Application.DTOs.ProductDTOs;
using LjussAPI.Application.Interfaces;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using NLog.Targets.Wrappers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace LjussAPI.Application.Services
{
    public class ProductService : IProductService
    {
        private readonly IProductRepository _repository;
        private readonly IImageService _imageService;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public ProductService(IProductRepository repository, IImageService imageService, IHttpContextAccessor httpContextAccessor)
        {
            _repository = repository;
            _imageService = imageService;
            _httpContextAccessor = httpContextAccessor;
        }
        public async Task<bool> CreateAsync(ProductCreateDto dto)
        {
            try
            {
                var product = new Product
                {
                    Id = Guid.NewGuid(),
                    Name = dto.Name,
                    Description = dto.Description,
                    Price = dto.Price,
                    Stock = dto.Stock,
                    CategoryId = dto.CategoryId,
                    TaxRate = dto.TaxRate,
                    DiscountRate = dto.DiscountRate,
                    DiscountStart = dto.DiscountStart.Value.ToUniversalTime(),
                    DiscountEnd = dto.DiscountEnd.Value.ToUniversalTime(),
                    CreatedDate = DateTime.UtcNow,
                    
                };

                if (dto.Images != null && dto.Images.Count > 0)
                {
                    foreach (var imageDto in dto.Images)
                    {
                        var savedImage = await _imageService.UploadImageAsync(imageDto, product.Id.ToString());
                        product.ProductImages.Add(savedImage);
                    }
                }

                var added = await _repository.AddAsync(product);
                if (!added) 
                    return false;

                return added;
                
            }
            catch (Exception ex)
            {
                throw new ApplicationException("Product oluşturulurken bir hata oluştu.", ex);
            }
        }

        public async Task<bool> UpdateAsync(ProductUpdateDto dto)
        {
            try
            {
                var product = await _repository.GetByIdAsync(dto.Id);
                if (product == null)
                    throw new KeyNotFoundException("Product bulunamadı.");

                product.Name = dto.Name;
                product.Description = dto.Description;
                product.Price = dto.Price;
                product.Stock = dto.Stock;
                product.CategoryId = dto.CategoryId;
                product.TaxRate = dto.TaxRate;
                if(dto.DiscountRate != null)
                {
                    product.DiscountRate = dto.DiscountRate;
                }

                if(dto.DiscountStart != null) {
                    product.DiscountStart = dto.DiscountStart.Value.ToUniversalTime();
                }

                if( dto.DiscountEnd != null && dto.DiscountEnd > dto.DiscountStart) {
                    product.DiscountEnd = dto.DiscountEnd.Value.ToUniversalTime();
                }

                product.UpdatedDate = dto.UpdateDate.ToUniversalTime();
                                

                var deletedImageGuids = JsonConvert.DeserializeObject<List<Guid>>(dto.DeletedImageIds);

                if (deletedImageGuids != null && deletedImageGuids.Count > 0)
                {
                    foreach(var deleteImage in deletedImageGuids)
                    {
                        var delete = await _imageService.DeleteImageAsync(deleteImage);
                    }
                }

                if (dto.Images != null && dto.Images.Count > 0)
                {
                    foreach (var imageDto in dto.Images)
                    {
                        var savedImage = await _imageService.UploadImageAsync(imageDto, product.Id.ToString());
                        product.ProductImages.Add(savedImage);
                    }
                }

                var updated = _repository.Update(product);
                if (!updated) return false;

                return updated;
            }
            catch (Exception ex)
            {
                throw new ApplicationException("Product güncellenirken bir hata oluştu.", ex);
            }
        }

        public async Task<IEnumerable<ProductListDto>> GetAllAsync(string? searchText = "")
        {
            try
            {
                var products = _repository.GetAll(searchText, false).ToList();

                var result = new List<ProductListDto>();

                foreach (var product in products)
                {
                    var image = (await _imageService.GetImagesByOwnerAsync(product.Id)).FirstOrDefault();

                    result.Add(new ProductListDto
                    {
                        Id = product.Id,
                        Name = product.Name,
                        Price = product.Price,
                        Stock = product.Stock,
                        Description = product.Description,
                        CategoryName = product.Category?.Name ?? string.Empty,
                        imageUrl = image != null ? BuildFileUrl(image.FilePath) : null
                    });
                }

                return result;
            }
            catch (Exception ex)
            {
                throw new ApplicationException("Product listesi alınırken bir hata oluştu.", ex);
            }
        }

        public async Task<ProductDetailDto?> GetByIdAsync(Guid id)
        {
            try
            {
                var product = await _repository.GetByIdAsync(id);
                if (product == null) return null;

                var dto = new ProductDetailDto
                {
                    Id = product.Id,
                    Name = product.Name,
                    Description = product.Description,
                    Price = product.Price,
                    Stock = product.Stock,
                    CategoryId = product.CategoryId,
                    CategoryName = product.Category?.Name,
                    TaxRate = product.TaxRate,
                    DiscountRate = product.DiscountRate,
                    DiscountStart = product.DiscountStart,
                    DiscountEnd = product.DiscountEnd,
                    Attributes = product.ProductAttributes?.Select(pa => new ProductAttributeDto
                    {
                        AttributeId = pa.AttributeId,
                        AttributeName = pa.Attribute.Name,
                        AttributeValueId = pa.AttributeValueId,
                        AttributeValue = pa.AttributeValue.Value
                    }).ToList(),
                    Comments = product.Comments?.Select(c => new CommentDto
                    {
                        Id = c.Id,
                        UserId = c.UserId,
                        UserName = c.User.Name,
                        Content = c.Content,
                        CreatedDate = c.CreatedDate
                    }).ToList()
                };

                // Image servisinden resimleri çek
                var images = await _imageService.GetImagesByOwnerAsync(product.Id);
                dto.Images = images.Select(i => new ImageDto
                {
                    Id = i.Id,
                    FileName = i.FileName,
                    FileDescription = i.FileDescription,
                    FileUrl = BuildFileUrl(i.FilePath),
                    Type = i.Type
                }).ToList();

                return dto;
            }
            catch (Exception ex)
            {
                throw new ApplicationException("Product detayları getirilirken bir hata oluştu.", ex);
            }
        }

        public async Task<bool> DeleteAsync(Guid id)
        {
            try
            {
                var product = await _repository.GetByIdAsync(id);
                if (product == null)
                    throw new KeyNotFoundException("Product bulunamadı.");

                var removed = _repository.Remove(product);
                if (!removed) return false;

                return removed;
            }
            catch (Exception ex)
            {
                throw new ApplicationException("Product silinirken bir hata oluştu.", ex);
            }
        }
        private string? BuildFileUrl(string? path, string virtualFolder = "uploads")
        {
            if (string.IsNullOrWhiteSpace(path))
                return null;

            // Zaten tam URL ise aynen döner
            if (path.StartsWith("http", StringComparison.OrdinalIgnoreCase))
                return path;          

            var fileName = Path.GetFileName(path);
            var scheme = _httpContextAccessor.HttpContext.Request.Scheme;
            var host = _httpContextAccessor.HttpContext.Request.Host;

            return $"{scheme}://{host}/{virtualFolder}/{fileName}";
        }
    }
}
