using Ljuss.Core.Configurations;
using Ljuss.Core.Enums;
using LjussAPI.Application.DTOs.CategoryDTOs;
using LjussAPI.Application.DTOs.ShippingAddressDTOs;
using LjussAPI.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace LjussAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ShippingAddressController : ControllerBase
    {
        private readonly IShippingAddressService _shippingAddressService;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public ShippingAddressController(IShippingAddressService shippingAddressService, IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
            _shippingAddressService = shippingAddressService;
        }

        [HttpGet("GetAllShippingAddress")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetAll(string? searchText = "")
        {
            var result = await _shippingAddressService.GetAllAsync(searchText);
            return Ok(result);
        }

        [HttpGet("GetAllUserShippingAddress")]
        [Authorize(Roles = "User")]
        public async Task<IActionResult> GetAllByUserId(Guid id, string? searchText = "")
        {
            var result = await _shippingAddressService.GetAllByUserIdAsync(id,searchText);
            return Ok(result);
        }

        [HttpGet("{id}")]
        [Authorize(Roles = "User")]
        public async Task<IActionResult> GetById(Guid id)
        {
            var result = await _shippingAddressService.GetByIdAsync(id);
            if (result == null)
            {
                throw new CustomException("Kullanıcı Adresi bulunamadı", ErrorCodesEnum.ProductNotExist);
            }
            return Ok(result);
        }

        [HttpPost]
        [Authorize(Roles = "Admin , User")]
        public async Task<IActionResult> Create([FromBody] CreateShippingAddressDto dto)
        {
            if (await _shippingAddressService.ExisitByName(dto.Title))
            {
                return BadRequest(new { errorMessage = "Adres zaten mevcut, lütfen başka isim kullanınız." });
            }

            var shippingAddress = await _shippingAddressService.CreateAsync(dto);
            if (shippingAddress == null)
            {
                return BadRequest(new { errorMessage = "Adres oluşturulamadı" });
            }
            return Ok(shippingAddress);
        }

        [HttpPut]
        [Authorize(Roles = "Admin , User")]
        public async Task<IActionResult> Update([FromBody] UpdateShippingAddressDto dto)
        {
            var updated = await _shippingAddressService.UpdateAsync(dto);
            if (!updated)
            {
                return BadRequest(new { errorMessage = "Adres güncellenmedi" });
            }
            return Ok(dto);
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin , User")]
        public async Task<IActionResult> Delete(Guid id)
        {
            var deleted = await _shippingAddressService.DeleteAsync(id);
            if (!deleted)
            {
                throw new CustomException("Adres silinemedi", ErrorCodesEnum.CategoryCanNotDelete);
            }
            return Ok("Adres başarıyla silindi");
        }

    }
}
