using Ljuss.Core.Entities;
using LjussAPI.Application.DTOs.CategoryDTOs;
using LjussAPI.Application.DTOs.ProductDTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LjussAPI.Application.Interfaces
{
    public interface ICategoryService
    {
        Task<IEnumerable<CategoryListDto>> GetAllAsync(string? searchText = "");
        Task<CategoryDetailDto?> GetByIdAsync(Guid id);
        Task<bool> ExisitByName(string name);
        Task<Category?> CreateAsync(CategoryCreateDto dto);
        Task<bool> UpdateAsync(CategoryUpdateDto dto);
        Task<bool> DeleteAsync(Guid id);
    }
}
