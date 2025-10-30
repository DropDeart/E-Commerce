using Ljuss.Core.Entities;
using Ljuss.Core.Repositories;
using LjussAPI.Application.DTOs.AttributeDTOs;
using LjussAPI.Application.DTOs.CategoryDTOs;
using LjussAPI.Application.DTOs.ProductDTOs;
using LjussAPI.Application.DTOs.ShippingAddressDTOs;
using LjussAPI.Application.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace LjussAPI.Application.Services
{
    public class ShippingAddressService : IShippingAddressService
    {
        private readonly IShippingAddressRepository _shippingAddressRepository;
        private readonly IHttpContextAccessor _httpContextAccessor;
        public ShippingAddressService(IShippingAddressRepository shippingAddressRepository, IHttpContextAccessor httpContextAccessor)
        {
            _shippingAddressRepository = shippingAddressRepository;
            _httpContextAccessor = httpContextAccessor;

        }
        public async Task<ShippingAddress?> CreateAsync(CreateShippingAddressDto dto)
        {
            try
            {
                var userId = _httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if(userId == null)
                {
                    return null;
                }

                var shippingAddress = new ShippingAddress
                {
                    Id = Guid.NewGuid(),
                    UserId = Guid.Parse(userId),
                    Title = dto.Title,
                    AddressLine1 = dto.AddressLine1,
                    AddressLine2 = dto.AddressLine2,
                    CityId = dto.CityId,
                    DistrictId = dto.DistrictId,
                    IsActive = dto.IsActive,
                    CreatedDate = DateTime.UtcNow,
                };

                var result = await _shippingAddressRepository.AddAsync(shippingAddress);
                if (result)
                {
                    return shippingAddress;
                }

                return null;
            }
            catch (Exception ex)
            {
                throw new ApplicationException("Adres oluşturulurken bir hata oluştu", ex);
            }
        }

        public async Task<bool> DeleteAsync(Guid id)
        {
            try
            {
                var category = await _shippingAddressRepository.GetByIdAsync(id);
                if (category == null)
                    throw new KeyNotFoundException("Adres bulunamadı");

                var result = _shippingAddressRepository.Remove(category);
                if (!result)
                    return false;

                return true;
            }
            catch (Exception ex)
            {
                throw new ApplicationException("Adres silinirken bir hata oluştu", ex);
            }
        }

        public async Task<bool> ExisitByName(string name)
        {
            var exists = await _shippingAddressRepository.GetWhere(p => p.Title == name).AnyAsync();
            return exists;
        }

        public async Task<IEnumerable<ShippingAddressListDto>> GetAllAsync(string? searchText = "")
        {
            try
            {
                var shippingAddresses = _shippingAddressRepository.GetAll(searchText, false).ToList();

                var dtoList = shippingAddresses.Select(c => new ShippingAddressListDto
                {
                    Id = c.Id,
                    Title = c.Title,
                    Address = c.AddressLine1 + c.AddressLine2
                });

                return dtoList;
            }
            catch (Exception ex)
            {
                throw new ApplicationException("Adres listesi alınırken bir hata oluştu", ex);
            }
        }

        public async Task<IEnumerable<ShippingAddressDto>> GetAllByUserIdAsync(Guid userId, string? searchText = null)
        {
            try
            {
                var userAddresses = await _shippingAddressRepository
                    .GetAll()
                    .Where(a => a.UserId == userId &&
                                (string.IsNullOrEmpty(searchText) ||
                                 a.Title.Contains(searchText) ||
                                 a.AddressLine1.Contains(searchText)))
                    .Include(a => a.City)
                    .Include(a => a.District)
                    .ToListAsync();

                var dtoList = userAddresses.Select(a => new ShippingAddressDto
                {
                    Id = a.Id,
                    Title = a.Title,
                    AddressLine1 = a.AddressLine1,
                    AddressLine2 = a.AddressLine2,
                    CityId = a.CityId,
                    CityName = a.City.Name,
                    DistrictId = a.DistrictId,
                    DistrictName = a.District.Name,
                    IsActive = a.IsActive
                }).ToList();

                return dtoList;
            }
            catch (Exception ex)
            {
                throw new ApplicationException("Kullanıcı adresleri alınırken bir hata oluştu", ex);
            }
        }

        public async Task<ShippingAddressDto?> GetByIdAsync(Guid id)
        {
            try
            {
                var shippingAddress = await _shippingAddressRepository.GetByIdAsync(id);
                if (shippingAddress == null)
                    return null;

                var dto = new ShippingAddressDto
                {
                    Id = shippingAddress.Id,
                    Title = shippingAddress.Title,
                    AddressLine1 = shippingAddress.AddressLine1,
                    AddressLine2 = shippingAddress.AddressLine2,
                    CityId = shippingAddress.CityId,
                    DistrictId = shippingAddress.DistrictId,
                    CityName = shippingAddress.City.Name,
                    DistrictName = shippingAddress.District.Name,
                    IsActive = shippingAddress.IsActive,
                    
                };

                return dto;
            }
            catch (Exception ex)
            {
                throw new ApplicationException("Adres detayları alınırken bir hata oluştu", ex);
            }
        }

        public async Task<bool> UpdateAsync(UpdateShippingAddressDto dto)
        {
            try
            {
                var shippingAddress = await _shippingAddressRepository.GetByIdAsync(dto.Id);
                if (shippingAddress == null)
                    throw new KeyNotFoundException("Adres bulunamadı");

                shippingAddress.Title = dto.Title;
                shippingAddress.AddressLine1 = dto.AddressLine1;
                shippingAddress.AddressLine2 = dto.AddressLine2;
                shippingAddress.CityId = dto.CityId;
                shippingAddress.DistrictId = dto.DistrictId;                
                shippingAddress.UpdatedDate = DateTime.UtcNow;

                var result = _shippingAddressRepository.Update(shippingAddress);
                if (!result)
                {
                    return false;
                }

                return result;

            }
            catch (Exception ex)
            {
                throw new ApplicationException("Adres güncellenirken bir hata oluştu", ex);
            }
        }
    }
}
