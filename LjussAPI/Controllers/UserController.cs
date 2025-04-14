using Ljuss.Applications.DTOs;
using Ljuss.Core.Repositories;
using LjussAPI.Application.Interfaces;
using LjussAPI.Helper;
using Microsoft.AspNetCore.Mvc;

namespace LjussAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UserController : ControllerBase
    {
        private readonly IUserRepository _userRepository;
        private readonly IAuthService _authService;
        private readonly ILogger<UserController> _logger;
        private readonly IImageService _imageService;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public UserController(IUserRepository userRepository, ILogger<UserController> logger, IAuthService authService, IImageService imageService, IHttpContextAccessor httpContextAccessor)
        {
            _userRepository = userRepository;
            _authService = authService;
            _logger = logger;
            _imageService = imageService;
            _httpContextAccessor = httpContextAccessor;
        }

        [HttpGet("getUserById/{userId}")]
        public async Task<IActionResult> GetUserById(Guid userId)
        {
            try
            {
                // Kullanıcıyı ID ile al
                var user = await _userRepository.GetUserByIdAsync(userId);

                if (user == null)
                {
                    _logger.LogWarning($"User with ID {userId} not found.");
                    return NotFound("User not found");
                }

                return Ok(user); // Kullanıcıyı döndür
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while getting user.");
                return StatusCode(500, "Internal server error.");
            }
        }

        [HttpPost("updateProfileImage")]
        [Consumes("multipart/form-data")]
        public async Task<IActionResult> UpdateUserProfileImage([FromForm] ImageUploadRequestDto request,[FromForm] string token,[FromForm] string userId)
        {
            try
            {
                #region Token Kontrolü

                var verifiedUserId = _authService.GetUserIdFromToken(token);
                if (verifiedUserId != userId)
                {
                    return Unauthorized(new { errorMessage = "İşlem yetkiniz bulunmamaktadır." });
                }
                #endregion

                #region Dosya Validasyonu

                var imageHelper = new ImageHelper();
                imageHelper.ValidateFileUpload(request);                

                #endregion

                var images = await _imageService.GetImagesByOwnerAsync(Guid.Parse(userId));
                if (images?.Count > 0)
                {
                    foreach (var image in images)
                    {
                        await _imageService.DeleteImageAsync(image.Id);
                    }
                }

                var uploadedImage = await _imageService.UploadImageAsync(request, userId);

                var user = await _userRepository.GetUserByIdAsync(Guid.Parse(userId));

                user.ProfilePictureUrl = uploadedImage.FilePath;               

                await _userRepository.UpdateUserAsync(user);

                string profilePictureUrl = null;
                if (!string.IsNullOrEmpty(user.ProfilePictureUrl))
                {
                    if (user.ProfilePictureUrl.StartsWith("http"))
                    {
                        profilePictureUrl = user.ProfilePictureUrl;
                    }
                    else
                    {
                        if (_httpContextAccessor.HttpContext != null)
                        {
                            var fileName = Path.GetFileName(user.ProfilePictureUrl);
                            profilePictureUrl = $"{_httpContextAccessor.HttpContext.Request.Scheme}://{_httpContextAccessor.HttpContext.Request.Host}/uploads/{fileName}";
                        }
                        else
                        {
                            _logger.LogError("HttpContext null geldi!");
                            return StatusCode(500, new { errorMessage = "Bir hata oluştu" });
                        }
                    }
                }

                return Ok(new
                {
                    imageUrl = profilePictureUrl, // Doğru URL'yi döndürün
                    message = "Profil resmi güncellendi"
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Profil resmi güncelleme hatası");
                return StatusCode(500, new
                {
                    errorMessage = "Bir hata oluştu",
                    detail = ex.Message
                });
            }
        }

    }
}
