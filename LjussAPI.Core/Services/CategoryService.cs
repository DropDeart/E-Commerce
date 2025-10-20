using Ljuss.Core.Entities;
using Ljuss.Core.Repositories;
using LjussAPI.Application.DTOs.CategoryDTOs;
using LjussAPI.Application.DTOs.ProductDTOs;
using LjussAPI.Application.Interfaces;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LjussAPI.Application.Services
{
    public class CategoryService : ICategoryService
    {
        private readonly ICategoryRepository _categoryRepository;

        public CategoryService(ICategoryRepository categoryRepository)
        {
            _categoryRepository = categoryRepository;
        }
        public async Task<Category?> CreateAsync(CategoryCreateDto dto)
        {
            try
            {
                var category = new Category
                {
                    Id = Guid.NewGuid(),
                    Name = dto.Name,
                    Description = dto.Description,
                    CreatedDate = DateTime.UtcNow,
                };

                var result = await _categoryRepository.AddAsync(category);
                if (result)
                {
                    return category;
                }

                return null;
            }
            catch (Exception ex)
            {
                throw new ApplicationException("Kategori oluşturulurken bir hata oluştu", ex);
            }
        }

        public async Task<bool> DeleteAsync(Guid id)
        {
            try
            {
                var category = await _categoryRepository.GetByIdAsync(id);
                if (category == null)
                    throw new KeyNotFoundException("Kategori bulunamadı");

                var result = _categoryRepository.Remove(category);
                if (!result)
                    return false;

                return true;
            }
            catch (Exception ex)
            {
                throw new ApplicationException("Kategori silinirken bir hata oluştu", ex);
            }
        }

        public async Task<bool> ExisitByName(string name)
        {
            var exists = await _categoryRepository.GetWhere(p => p.Name == name).AnyAsync();
            return exists;
        }

        public async Task<IEnumerable<CategoryListDto>> GetAllAsync(string? searchText = "")
        {
            try
            {
                var categories = _categoryRepository.GetAll(searchText, false).ToList();

                var dtoList = categories.Select(c => new CategoryListDto
                {
                    Id = c.Id,
                    Name = c.Name,
                    Description = c.Description
                });

                return dtoList;
            }
            catch (Exception ex)
            {
                throw new ApplicationException("Kategori listesi alınırken bir hata oluştu", ex);
            }
        }

        public async Task<CategoryDetailDto?> GetByIdAsync(Guid id)
        {
            try
            {
                var category = await _categoryRepository.GetByIdAsync(id);
                if (category == null)
                    return null;

                var dto = new CategoryDetailDto
                {
                    Id = category.Id,
                    Name = category.Name,
                    Description = category.Description,
                    Products = category.Products?.Select(p => new ProductListDto
                    {
                        Id = p.Id,
                        Name = p.Name,
                        Price = p.Price,
                        Stock = p.Stock,
                        CategoryName = category.Name
                    }).ToList()
                };

                return dto;
            }
            catch (Exception ex)
            {
                throw new ApplicationException("Kategori detayları alınırken bir hata oluştu", ex);
            }
        }

        public async Task<bool> UpdateAsync(CategoryUpdateDto dto)
        {
            try
            {
                var category = await _categoryRepository.GetByIdAsync(dto.Id);
                if (category == null)
                    throw new KeyNotFoundException("Kategori bulunamadı");

                category.Name = dto.Name;
                category.Description = dto.Description;
                category.UpdatedDate = DateTime.UtcNow;

                var result = _categoryRepository.Update(category);
                if(!result)
                {
                    return false;
                }

                return result;
                
            }
            catch (Exception ex)
            {
                throw new ApplicationException("Kategori güncellenirken bir hata oluştu", ex);
            }
        }
    }
}
