using Ljuss.Core.Configurations;
using Ljuss.Core.Enums;
using LjussAPI.Application.DTOs.ImageDTOs;
using LjussAPI.Application.DTOs.ProductDTOs;
using LjussAPI.Application.Interfaces;
using LjussAPI.Helper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LjussAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ProductController : ControllerBase
    {
        private readonly IProductService _productService;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public ProductController(IProductService productService, IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
            _productService = productService;
        }

        [HttpGet("GetAllProducts")]
        [AllowAnonymous]
        public async Task<IActionResult> GetAll(string? searchText = "")
        {
            var result = await _productService.GetAllAsync(searchText);
            return Ok(result);
        }

        [HttpGet("{id}")]
        [AllowAnonymous]
        public async Task<IActionResult> GetById(Guid id)
        {
            var result = await _productService.GetByIdAsync(id);
            if (result == null)
            {
                throw new CustomException("Ürün bulunamadı", ErrorCodesEnum.ProductNotExist);
            }
            return Ok(result);
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Create([FromForm] ProductCreateDto dto)
        {
            var imageHelper = new ImageHelper();
            foreach (var img in dto.Images ?? new List<ImageUploadRequestDto>())
            {
                imageHelper.ValidateFileUpload(img);
            }

            var created = await _productService.CreateAsync(dto);
            if (!created)
            {
                throw new CustomException("Ürün oluşturulamadı", ErrorCodesEnum.ProductCanNotCreate);
            }
            return Ok(new
            {
                message = "Ürün başarı ile oluşturuldu."
            });
        }

        [HttpPut]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Update([FromForm] ProductUpdateDto dto)
        {
            var imageHelper = new ImageHelper();
            foreach (var img in dto.Images ?? new List<ImageUploadRequestDto>())
            {
                imageHelper.ValidateFileUpload(img);
            }

            var updated = await _productService.UpdateAsync(dto);
            if (!updated)
            {
                throw new CustomException("Ürün güncellenemedi", ErrorCodesEnum.ProductCanNotUpdate);
            }
            return Ok(new
            {
                message = "Ürün başarı ile güncellendi."
            });
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(Guid id)
        {
            var deleted = await _productService.DeleteAsync(id);
            if (!deleted)
            {
                throw new CustomException("Ürün silinemedi", ErrorCodesEnum.ProductCanNotDelete);
            }
            return Ok(new
            {
                message = "Ürün başarı ile silindi."
            });
        }

    }
}
