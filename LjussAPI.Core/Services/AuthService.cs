using Ljuss.Core.Domian;
using Ljuss.Applications.DTOs;
using Ljuss.Core.Repositories;
using LjussAPI.Application.Interfaces;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace LjussAPI.Application.Services
{
    public class AuthService : IAuthService
    {
        private readonly IUserRepository _userRepository;
        private readonly IJwtTokenService _jwtTokenService;
        private readonly IVerificationService _verificationService;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public AuthService(IUserRepository userRespository, IJwtTokenService jwtTokenService, IVerificationService verificationService, IHttpContextAccessor httpContextAccessor)
        {
            _jwtTokenService = jwtTokenService;
            _userRepository = userRespository;
            _verificationService = verificationService;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task<AuthResponseDto?> AuthenticateAsync(LoginRequestDto loginDto)
        {
            var user = await _userRepository.GetUserByEmailAsync(loginDto.Email);

            if (user == null || !BCrypt.Net.BCrypt.Verify(loginDto.Password, user.PasswordHash))
            {
                return null;
            }

            string profilePictureUrl = null;
            if (!String.IsNullOrEmpty(user.ProfilePictureUrl))
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
                }
            }

            var userResponse = new UserResponseDto
            {
                Id = user.Id,
                Name = user.Name,
                Email = user.Email,
                isVerified = user.IsVerified,
                Role = user.Role,
                ProfilePictureUrl = profilePictureUrl // Dönüştürülmüş URL burada kullanılıyor
            };

            var token = _jwtTokenService.GenrateToken(user);

            return new AuthResponseDto
            {
                Token = token.Token,
                AccessTime = DateTime.Now,
                User = userResponse
            };
        }

        public async Task<AuthResponseDto?> RegisterAsync(UserCreateDto userCreateDto)
        {
            var existingUser = await _userRepository.GetUserByEmailAsync(userCreateDto.Email);
            if (existingUser != null)
            {
                return null;
            }

            var hashedPassword = BCrypt.Net.BCrypt.HashPassword(userCreateDto.Password);

            var user = new User
            {
                Name = userCreateDto.Name,
                Email = userCreateDto.Email,
                PasswordHash = hashedPassword
            };

            var createdUser = await _userRepository.AddUserAsync(user);

            await _verificationService.SendVerificationCodeAsync(createdUser.Id, createdUser.Email);

            return _jwtTokenService.GenrateToken(createdUser);
        }

        public string GetUserIdFromToken(string token)
        {
            if(String.IsNullOrWhiteSpace(token))
            {
                throw new ArgumentNullException("Token geçersiz" , nameof(token));
            }
            try
            {
                var jwtTokenHandler = new JwtSecurityTokenHandler();
                var tokenS = token.Replace("Bearer ", "");
                var tokenSDecoded = jwtTokenHandler.ReadJwtToken(tokenS);

                var claims = tokenSDecoded?.Claims?.ToList();
                var userId = claims?.FirstOrDefault(p => p.Type == ClaimTypes.NameIdentifier)?.Value;

                if (String.IsNullOrWhiteSpace(userId))
                {
                    throw new UnauthorizedAccessException("User ID bulunamadı.");
                }

                return userId;
            }
            catch (Exception ex)
            {
                throw new UnauthorizedAccessException("Geçersiz token.");
            }
        }
    }
}
