using LjussAPI.Application.DTOs.ProductDTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LjussAPI.Application.Interfaces
{
    public interface IProductService
    {
        Task<IEnumerable<ProductListDto>> GetAllAsync(string? searchText = "");
        Task<ProductDetailDto?> GetByIdAsync(Guid id);

        Task<bool> CreateAsync(ProductCreateDto dto);
        Task<bool> UpdateAsync(ProductUpdateDto dto);
        Task<bool> DeleteAsync(Guid id);
    }
}
