using Ljuss.Core.Entities;
using Ljuss.Core.Repositories;
using LjussAPI.Application.DTOs.AuthDTOs;
using LjussAPI.Application.DTOs.ImageDTOs;
using LjussAPI.Application.DTOs.UserDTOs;
using LjussAPI.Application.Interfaces;
using LjussAPI.Helper;
using Microsoft.AspNetCore.Authorization;
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
        [Authorize(Roles = "User")]
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
        [Authorize(Roles = "User")]
        public async Task<IActionResult> UpdateUserProfileImage([FromForm] ImageUploadRequestDto request)
        {
            try
            {
                #region Token Kontrolü

                var authHeader = Request.Headers["Authorization"].ToString();
                if (String.IsNullOrEmpty(authHeader) || !authHeader.StartsWith("Bearer "))
                {
                    return Unauthorized(new { errorMessage = "Geçersiz token formatı." });
                }

                var token = authHeader.Replace("Bearer ", "");
                var verifiedUserId = _authService.GetUserIdFromToken(token);

                #endregion

                #region Dosya Validasyonu

                var imageHelper = new ImageHelper();
                imageHelper.ValidateFileUpload(request);

                #endregion

                #region Eski Resimleri Silme

                var images = await _imageService.GetImagesByOwnerAsync(Guid.Parse(verifiedUserId));
                if (images?.Count > 0)
                {
                    foreach (var image in images)
                    {
                        await _imageService.DeleteImageAsync(image.Id);
                    }
                }

                #endregion

                #region Yeni Resmi Yükleme

                var uploadedImage = await _imageService.UploadImageAsync(request, verifiedUserId);
                var user = await _userRepository.GetUserByIdAsync(Guid.Parse(verifiedUserId));
                user.ProfilePictureUrl = uploadedImage.FilePath;
                await _userRepository.UpdateUserAsync(user);

                #endregion

                #region URL Hazırlama

                string profilePictureUrl = null;
                if (!String.IsNullOrEmpty(user.ProfilePictureUrl))
                {
                    if (user.ProfilePictureUrl.StartsWith("http"))
                    {
                        profilePictureUrl = user.ProfilePictureUrl;
                    }
                    else if (_httpContextAccessor.HttpContext != null)
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

                #endregion

                return Ok(new
                {
                    imageUrl = profilePictureUrl,
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


        [HttpPost("updateUser")]
        [Authorize(Roles = "User")]
        public async Task<IActionResult> UpdateUserAsync([FromBody] UserUpdateDto userUpdateDto)
        {
            try
            {
                #region Token Kontrolu

                var authHeader = Request.Headers["Authorization"].ToString();
                if (String.IsNullOrEmpty(authHeader) || !authHeader.StartsWith("Bearer "))
                {
                    return Unauthorized(new { errorMessage = "Geçersiz token formatı." });
                }

                var token = authHeader.Replace("Bearer ", "");
                var verifiedUserId = _authService.GetUserIdFromToken(token);

                #endregion

                #region Kullanıcı Kontrolü

                var user = await _userRepository.GetUserByIdAsync(Guid.Parse(verifiedUserId));
                if (user == null)
                {
                    return NotFound(new { errorMessage = "Kullanıcı bulunamadı." });
                }

                #endregion

                #region Verilerin Güncellenmesi

                user.Name = userUpdateDto.Name ?? user.Name;
                user.PhoneNumber = userUpdateDto.PhoneNumber ?? user.PhoneNumber;

                await _userRepository.UpdateUserAsync(user);

                #endregion

                return Ok(new { message = "Kullanıcı bilgileri başarıyla güncellendi." });
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error: {ex.Message}");
                return StatusCode(500, new
                {
                    errorMessage = "Bir hata oluştu",
                    detail = ex.Message
                });
            }
        }

        [HttpPost("updateUserPassword")]
        [Authorize(Roles = "User")]
        public async Task<IActionResult> UpdateUserPassword([FromBody] UserPasswordUpdateDto userPasswordUpdateDto)
        {
            try
            {
                #region Token Kontrolu

                var authHeader = Request.Headers["Authorization"].ToString();
                if (String.IsNullOrEmpty(authHeader) || !authHeader.StartsWith("Bearer "))
                {
                    return Unauthorized(new { errorMessage = "Geçersiz token formatı." });
                }

                var token = authHeader.Replace("Bearer ", "");
                var verifiedUserId = _authService.GetUserIdFromToken(token);

                #endregion

                #region Kullanıcı Kontrolü

                var user = await _userRepository.GetUserByIdAsync(Guid.Parse(verifiedUserId));
                if (user == null)
                {
                    return NotFound(new { errorMessage = "Kullanıcı bulunamadı." });
                }

                #endregion

                #region Verilerin Kontolu

                if (!await _userRepository.CheckUserPasswordAsync(user.Id, userPasswordUpdateDto.CurrentPassword))
                {
                    return BadRequest(new { errorMessage = "Mevcut şifre yanlış." });
                }

                if (userPasswordUpdateDto.NewPassword != userPasswordUpdateDto.ConfirmPassword)
                {
                    return BadRequest(new { errorMessage = "Şifreler uyumsuz, Lütfen tekrar deneyin." });
                }    
                #endregion

                #region Verilerin Güncellenmesi

                var hashedPassword = BCrypt.Net.BCrypt.HashPassword(userPasswordUpdateDto.ConfirmPassword);

                user.PasswordHash = hashedPassword;

                await _userRepository.UpdateUserAsync(user);

                #endregion

                return Ok(new { message = "Kullanıcı şifresi başarıyla güncellendi." });

            }
            catch (Exception ex)
            {
                _logger.LogError($"Error:{ex.Message}");
                return StatusCode(500, new
                {
                    errorMessage = "Bir hata oluştu",
                    detail = ex.Message
                });
            }
        }

    }
}
