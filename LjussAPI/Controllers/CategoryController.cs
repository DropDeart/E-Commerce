using Ljuss.Core.Configurations;
using Ljuss.Core.Enums;
using LjussAPI.Application.DTOs.CategoryDTOs;
using LjussAPI.Application.DTOs.ImageDTOs;
using LjussAPI.Application.DTOs.ProductDTOs;
using LjussAPI.Application.Interfaces;
using LjussAPI.Helper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace LjussAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CategoryController : ControllerBase
    {
        private readonly ICategoryService _categoryService;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public CategoryController(ICategoryService categoryService, IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
            _categoryService = categoryService;
        }

        [HttpGet("GetAllCategories")]
        [AllowAnonymous]
        public async Task<IActionResult> GetAll(string? searchText = "")
        {
            var result = await _categoryService.GetAllAsync(searchText);
            return Ok(result);
        }

        [HttpGet("{id}")]
        [AllowAnonymous]
        public async Task<IActionResult> GetById(Guid id)
        {
            var result = await _categoryService.GetByIdAsync(id);
            if (result == null)
            {
                throw new CustomException("Kategori bulunamadı", ErrorCodesEnum.ProductNotExist);
            }
            return Ok(result);
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Create([FromBody] CategoryCreateDto dto)
        {
            if(await _categoryService.ExisitByName(dto.Name)){
                return BadRequest(new { errorMessage = "Kategori zaten mevcut, lütfen başka isim kullanınız." });
            }

            var category = await _categoryService.CreateAsync(dto); 
            if (category == null)
            {
                return BadRequest(new { errorMessage = "Kategori oluşturulamadı" });
            }
            return Ok(category);
        }

        [HttpPut]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Update([FromBody] CategoryUpdateDto dto)
        {
            var updated = await _categoryService.UpdateAsync(dto);
            if (!updated)
            {
                return BadRequest(new { errorMessage = "Kategori güncellenmedi" });
            }
            return Ok(dto);
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(Guid id)
        {
            var deleted = await _categoryService.DeleteAsync(id);
            if (!deleted)
            {
                throw new CustomException("Kateogri silinemedi", ErrorCodesEnum.CategoryCanNotDelete);
            }
            return Ok("Kategori başarıyla silindi");
        }

    }
}
