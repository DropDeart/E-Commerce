using Ljuss.Core.Entities;
using Ljuss.Core.Repositories;
using LjussAPI.Application.Interfaces;
using Microsoft.AspNetCore.Http; // IHttpContextAccessor için
using System;
using System.Security.Claims;
using System.IdentityModel.Tokens.Jwt; // JwtSecurityTokenHandler için
using LjussAPI.Application.DTOs.UserDTOs;
using LjussAPI.Application.DTOs.AuthDTOs;
using Ljuss.Core.Enums; // RoleEnum için
using Microsoft.Extensions.Logging; // ILogger<T> için

namespace LjussAPI.Application.Services
{
    public class AuthService : IAuthService
    {
        private readonly IUserRepository _userRepository;
        private readonly IJwtTokenService _jwtTokenService;
        private readonly IVerificationService _verificationService;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ILogger<AuthService> _logger; // Microsoft.Extensions.Logging.ILogger<T> kullanıyoruz

        public AuthService(
            IUserRepository userRepository,
            IJwtTokenService jwtTokenService,
            IVerificationService verificationService,
            IHttpContextAccessor httpContextAccessor,
            ILogger<AuthService> logger) // ILogger<AuthService> enjekte ediliyor
        {
            _jwtTokenService = jwtTokenService;
            _userRepository = userRepository;
            _verificationService = verificationService;
            _httpContextAccessor = httpContextAccessor;
            _logger = logger; // Atama doğru tipte yapılıyor
        }

        public async Task<AuthResponseDto?> AuthenticateAsync(LoginRequestDto loginDto)
        {
            var user = await _userRepository.GetUserByEmailAsync(loginDto.Email);

            if (user == null || !BCrypt.Net.BCrypt.Verify(loginDto.Password, user.PasswordHash))
            {
                _logger.LogWarning($"Authentication failed for email: {loginDto.Email} - Invalid credentials.");
                return null;
            }

            // Kullanıcı doğrulanmamışsa, Access Token oluşturmadan dön
            if (!user.IsVerified)
            {
                _logger.LogInformation($"User {user.Email} is not verified. Authentication successful but email verification required.");
                // Controller'ın doğrulamaya yönlendirmesi için token'sız bir response dönüyoruz
                return new AuthResponseDto
                {
                    Token = null,
                    RefreshToken = null,
                    AccessTime = DateTime.UtcNow,
                    ExpiresIn = 0,
                    User = new UserResponseDto
                    {
                        Id = user.Id,
                        Name = user.Name,
                        Email = user.Email,
                        isVerified = user.IsVerified,
                        Role = user.Role,
                        ProfilePictureUrl = BuildFileUrl(user.ProfilePictureUrl)
                    }
                };
            }

            // IP Adresini al (IP, HttpContextAccessor'dan alınır)
            var ipAddress = _httpContextAccessor.HttpContext?.Connection.RemoteIpAddress?.ToString() ?? "N/A";

            // JWT ve Refresh Token'ı oluştur (JwtTokenService'in tam AuthResponseDto döndürdüğünü varsayıyoruz)
            var authResponse = _jwtTokenService.GenrateToken(user, ipAddress);

            // JwtTokenService'in User bilgilerini AuthResponseDto'ya zaten eklediğini varsayıyoruz.
            // Eğer eklemiyorsa, burada manuel olarak eklemeniz gerekebilir.
            if (authResponse.User == null)
            {
                authResponse.User = new UserResponseDto
                {
                    Id = user.Id,
                    Name = user.Name,
                    Email = user.Email,
                    isVerified = user.IsVerified,
                    Role = user.Role,
                    ProfilePictureUrl = BuildFileUrl(user.ProfilePictureUrl)
                };
            }
            else
            {
                // Mevcut UserResponseDto'yu güncelle
                authResponse.User.isVerified = user.IsVerified;
                authResponse.User.Role = user.Role;
                authResponse.User.ProfilePictureUrl = BuildFileUrl(user.ProfilePictureUrl);
            }

            _logger.LogInformation($"User {user.Email} authenticated successfully. Tokens generated.");
            return authResponse;
        }

        public async Task<AuthResponseDto?> RegisterAsync(UserCreateDto userCreateDto)
        {
            var existingUser = await _userRepository.GetUserByEmailAsync(userCreateDto.Email);
            if (existingUser != null)
            {
                _logger.LogWarning($"Registration attempt with existing email: {userCreateDto.Email}");
                return null; // E-posta zaten mevcut
            }

            var hashedPassword = BCrypt.Net.BCrypt.HashPassword(userCreateDto.Password);

            var newUser = new User
            {
                Name = userCreateDto.Name,
                Email = userCreateDto.Email,
                PasswordHash = hashedPassword,
                IsVerified = false, // Yeni kullanıcı başlangıçta doğrulanmamış
                Role = RoleEnum.User // Varsayılan rol
            };

            var createdUser = await _userRepository.AddUserAsync(newUser);
            _logger.LogInformation($"New user registered: {createdUser.Email}");

            // Doğrulama kodu gönder
            await _verificationService.SendVerificationCodeAsync(createdUser.Id, createdUser.Email);
            _logger.LogInformation($"Verification code sent to new user: {createdUser.Email}");

            // --- BURADA DEĞİŞİKLİK YAPIYORUZ ---
            // Doğrulanmamış kullanıcıya TOKEN oluşturma!
            // Bunun yerine, frontend'in ihtiyaç duyduğu bilgileri içeren bir AuthResponseDto döndürelim,
            // ancak Token ve ExpiresIn alanları boş (null) olsun.

            var authResponse = new AuthResponseDto
            {
                // Token ve ExpiresIn burada boş bırakılacak
                Token = null,
                ExpiresIn = 0, // veya başka bir varsayılan değer

                User = new UserResponseDto
                {
                    Id = createdUser.Id,
                    Name = createdUser.Name,
                    Email = createdUser.Email,
                    isVerified = createdUser.IsVerified, // False olacak
                    Role = createdUser.Role,
                    ProfilePictureUrl = BuildFileUrl(createdUser.ProfilePictureUrl)
                }
            };

            _logger.LogInformation($"Registration completed for new user: {createdUser.Email}. No tokens issued until verification.");
            return authResponse;
        }

        public async Task<AuthResponseDto> RefreshTokenAsync(string refreshToken, string ipAddress)
        {
            _logger.LogInformation($"Attempting to refresh token for IP: {ipAddress}");
            // Doğrudan IJwtTokenService'e delege ediyoruz
            return await _jwtTokenService.RefreshTokenAsync(refreshToken, ipAddress);
        }

        public string GetUserIdFromToken(string token)
        {
            if (string.IsNullOrWhiteSpace(token))
            {
                _logger.LogWarning("Attempted to get user ID from a null or whitespace token.");
                throw new ArgumentNullException("Token geçersiz", nameof(token));
            }
            try
            {
                var jwtTokenHandler = new JwtSecurityTokenHandler();
                var tokenS = token.Replace("Bearer ", ""); // "Bearer " ön ekini kaldır
                var tokenSDecoded = jwtTokenHandler.ReadJwtToken(tokenS);

                var userId = tokenSDecoded?.Claims?.FirstOrDefault(p => p.Type == ClaimTypes.NameIdentifier)?.Value;

                if (string.IsNullOrWhiteSpace(userId))
                {
                    _logger.LogWarning($"User ID claim not found in token: {tokenS}");
                    throw new UnauthorizedAccessException("User ID bulunamadı.");
                }

                _logger.LogDebug($"User ID {userId} extracted from token.");
                return userId;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Geçersiz token. Failed to get user ID from token.");
                throw new UnauthorizedAccessException("Geçersiz token.");
            }
        }

        // Yardımcı Metot: Profil resmi URL'sini oluşturmak için
        private string? BuildFileUrl(string? path, string virtualFolder = "uploads")
        {
            if (string.IsNullOrWhiteSpace(path))
                return null;

            // Zaten tam URL ise aynen döner
            if (path.StartsWith("http", StringComparison.OrdinalIgnoreCase))
                return path;

            if (_httpContextAccessor.HttpContext == null)
            {
                _logger.LogWarning("HttpContext is null. Cannot build file URL for local path.");
                return null;
            }

            var fileName = Path.GetFileName(path);
            var scheme = _httpContextAccessor.HttpContext.Request.Scheme;
            var host = _httpContextAccessor.HttpContext.Request.Host;

            return $"{scheme}://{host}/{virtualFolder}/{fileName}";
        }
    }
}