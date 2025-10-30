using Ljuss.Core.Entities;
using LjussAPI.Application.DTOs.AttributeDTOs;
using LjussAPI.Application.DTOs.CategoryDTOs;
using LjussAPI.Application.DTOs.ShippingAddressDTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LjussAPI.Application.Interfaces
{
    public interface IShippingAddressService
    {
        Task<IEnumerable<ShippingAddressListDto>> GetAllAsync(string? searchText = "");
        Task<ShippingAddressDto?> GetByIdAsync(Guid id);
        Task<IEnumerable<ShippingAddressDto>> GetAllByUserIdAsync(Guid userId, string? searchTest = "");
        Task<bool> ExisitByName(string name);
        Task<ShippingAddress?> CreateAsync(CreateShippingAddressDto dto);
        Task<bool> UpdateAsync(UpdateShippingAddressDto dto);
        Task<bool> DeleteAsync(Guid id);
    }
}
