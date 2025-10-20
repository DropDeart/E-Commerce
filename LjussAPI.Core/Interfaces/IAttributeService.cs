using Ljuss.Core.Entities;
using LjussAPI.Application.DTOs.AttributeDTOs;
using LjussAPI.Application.DTOs.CategoryDTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Attribute = Ljuss.Core.Entities.Attribute;

namespace LjussAPI.Application.Interfaces
{
    public interface IAttributeService
    {
        Task<IEnumerable<AttributeListDto>> GetAllAsync(string? searchText = "");
        Task<AttributeDetailDto?> GetByIdAsync(Guid id);
        Task<bool> ExisitByName(string name);
        Task<Attribute?> CreateAsync(AttributeDetailDto dto);
        Task<bool> UpdateAsync(AttributeDetailDto dto);
        Task<bool> DeleteAsync(Guid id);
    }
}
