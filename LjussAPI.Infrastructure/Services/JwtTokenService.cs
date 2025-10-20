using System.Security.Claims;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Microsoft.Extensions.Options;
using Ljuss.Core.Configurations; // JwtOptions için
using Ljuss.Core.Entities; // User ve RefreshToken modelleri için
using LjussAPI.Application.DTOs.AuthDTOs; // AuthResponseDto için
using LjussAPI.Application.Interfaces; // IJwtTokenService için
using LjussAPI.Persistence; // ApplicationDbContext
using Microsoft.EntityFrameworkCore; // Include, FirstOrDefaultAsync için
using Microsoft.Extensions.Logging; // ILogger için

namespace LjussAPI.Infrastructure.Services
{
    public class JwtTokenService : IJwtTokenService
    {
        private readonly JwtOptions _jwtOptions;
        private readonly AppDbContext _appDbContext; // DbContext'i inject ediyoruz
        private readonly ILogger<JwtTokenService> _logger; // Loglama için

        public JwtTokenService(IOptions<JwtOptions> jwtOptions, AppDbContext appDbContext, ILogger<JwtTokenService> logger)
        {
            _jwtOptions = jwtOptions.Value;
            _appDbContext = appDbContext;
            _logger = logger;
        }

        // Access ve Refresh Token oluşturma metodu
        public AuthResponseDto GenrateToken(User user, string ipAddress)
        {
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Email, user.Email),
                new Claim(ClaimTypes.Name, user.Name),
                new Claim(ClaimTypes.Role, user.Role.ToString())
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtOptions.Key));
            var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            // Access Token oluşturma
            var accessTokenExpires = DateTime.UtcNow.AddMinutes(_jwtOptions.ExpiryMinutes);
            var accessToken = new JwtSecurityToken(
                _jwtOptions.Issuer,
                _jwtOptions.Audience,
                claims,
                expires: accessTokenExpires,
                signingCredentials: credentials
            );

            // Refresh Token oluşturma ve kullanıcıya ekleme
            var refreshToken = GenrateRefreshToken(ipAddress);
            user.RefreshTokens.Add(refreshToken); // Kullanıcının koleksiyonuna ekle
            _appDbContext.Users.Update(user); // Kullanıcı objesini güncelle (change tracker)

            try
            {
                _appDbContext.SaveChanges();
                _logger.LogInformation($"Refresh token saved successfully for user: {user.Email}");
            }
            catch (DbUpdateException ex)
            {
                _logger.LogError(ex, "DbUpdateException occurred during SaveChanges in GenrateToken for user: {email}", user.Email);
                if (ex.InnerException != null)
                {
                    _logger.LogError(ex.InnerException, "Inner exception details: {innerExceptionMessage}", ex.InnerException.Message);
                }
                throw; // Hatanın propagasyonunu sağla
            }
            catch (Exception ex) // Genel hatalar için
            {
                _logger.LogError(ex, "An unexpected error occurred during SaveChanges in GenrateToken for user: {email}", user.Email);
                throw;
            }

            var authResponse = new AuthResponseDto
            {
                Token = new JwtSecurityTokenHandler().WriteToken(accessToken),
                RefreshToken = refreshToken.Token, // Refresh token'ı AuthResponseDto'ya ekle
                AccessTime = DateTime.UtcNow,
                ExpiresIn = (int)(accessTokenExpires - DateTime.UtcNow).TotalSeconds, // Access Token'ın saniye cinsinden kalan süresi
                User = new UserResponseDto
                {
                    Id = user.Id,
                    Name = user.Name,
                    Email = user.Email,
                    ProfilePictureUrl = user.ProfilePictureUrl,
                    isVerified = user.IsVerified // UserResponseDto'da bu alan varsa ekleyelim
                }
            };

            return authResponse;
        }

        // Yeni Metot: Refresh Token oluşturma
        public RefreshToken GenrateRefreshToken(string ipAddress)
        {
            var randomNumber = new byte[32]; // 32 byte = 256 bit random değer
            using var rng = System.Security.Cryptography.RandomNumberGenerator.Create();
            rng.GetBytes(randomNumber);
            return new RefreshToken
            {
                Token = Convert.ToBase64String(randomNumber),
                Expires = DateTime.UtcNow.AddDays(_jwtOptions.RefreshTokenExpiryDays), // Refresh Token süresi
                Created = DateTime.UtcNow,
                CreatedByIp = ipAddress
            };
        }

        // Yeni Metot: Refresh Token ile yeni Access/Refresh Token alma
        public async Task<AuthResponseDto> RefreshTokenAsync(string token, string ipAddress)
        {
            var refreshToken = await _appDbContext.RefreshTokens
                .Include(rt => rt.User) // Kullanıcıyı da dahil et
                .FirstOrDefaultAsync(rt => rt.Token == token);

            // 1. Refresh Token var mı?
            if (refreshToken == null)
            {
                _logger.LogWarning($"Attempt to refresh token with non-existent token.");
                throw new ApplicationException("Invalid refresh token.");
            }

            // 2. Süresi doldu mu?
            if (refreshToken.IsExpired)
            {
                _logger.LogWarning($"Attempt to refresh token with expired token: {token}");
                // Süresi dolmuş token'ları iptal et
                refreshToken.Revoked = DateTime.UtcNow;
                refreshToken.RevokedByIp = ipAddress;
                _appDbContext.RefreshTokens.Update(refreshToken);
                await _appDbContext.SaveChangesAsync();
                throw new ApplicationException("Refresh token expired. Please login again.");
            }

            // 3. İptal edildi mi? (Eğer önceden iptal edildiyse)
            // Bu kısım, Replay Attack Prevention için önemlidir (tek kullanımlık refresh tokenlar için)
            if (refreshToken.IsRevoked)
            {
                _logger.LogWarning($"Attempt to refresh token with revoked token by IP: {ipAddress}. Revoking all user tokens to prevent replay attacks.");
                // Eğer bu token daha önce iptal edilmişse, bu kullanıcının tüm aktif refresh token'larını iptal et.
                // Bu, bir saldırganın ele geçirdiği bir refresh token'ı tekrar kullanmasını engeller.
                var userRefreshTokens = await _appDbContext.RefreshTokens
                    .Where(rt => rt.UserId == refreshToken.UserId && rt.IsActive)
                    .ToListAsync();

                foreach (var userToken in userRefreshTokens)
                {
                    userToken.Revoked = DateTime.UtcNow;
                    userToken.RevokedByIp = ipAddress;
                    _appDbContext.RefreshTokens.Update(userToken);
                }
                await _appDbContext.SaveChangesAsync();

                throw new ApplicationException("Refresh token revoked. Please login again.");
            }

            // 4. Eğer refresh token geçerli ve aktif ise, onu iptal et (tek kullanımlık hale getir)
            // ve yeni bir refresh token oluştur.
            refreshToken.Revoked = DateTime.UtcNow;
            refreshToken.RevokedByIp = ipAddress;
            var newRefreshToken = GenrateRefreshToken(ipAddress);
            refreshToken.ReplacedByToken = newRefreshToken.Token; // Eski token'ın yerine yenisi geldiğini işaretle

            // Yeni refresh token'ı kullanıcıya ekle
            refreshToken.User.RefreshTokens.Add(newRefreshToken);

            // Veritabanı değişikliklerini kaydet
            _appDbContext.RefreshTokens.Update(refreshToken); // Eski token'ı güncelle
            _appDbContext.Users.Update(refreshToken.User); // Kullanıcıyı güncelle (yeni refresh token eklenmiş haliyle)
            await _appDbContext.SaveChangesAsync();


            // Yeni Access Token oluştur
            var accessTokenExpiresNew = DateTime.UtcNow.AddMinutes(_jwtOptions.ExpiryMinutes);
            var newAccessToken = new JwtSecurityTokenHandler().WriteToken(
                new JwtSecurityToken(
                    _jwtOptions.Issuer,
                    _jwtOptions.Audience,
                    // Bu noktada, eski Access Token'dan veya doğrudan User objesinden claim'leri almalıyız.
                    // GetClaimsFromExpiredToken metodu bunun için kullanılabilir.
                    GetClaimsForNewAccessToken(refreshToken.User),
                    expires: accessTokenExpiresNew,
                    signingCredentials: new SigningCredentials(new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtOptions.Key)), SecurityAlgorithms.HmacSha256)
                )
            );

            return new AuthResponseDto
            {
                Token = newAccessToken,
                RefreshToken = newRefreshToken.Token, // Yeni refresh token'ı dön
                AccessTime = DateTime.UtcNow,
                ExpiresIn = (int)(accessTokenExpiresNew - DateTime.UtcNow).TotalSeconds,
                User = new UserResponseDto
                {
                    Id = refreshToken.User.Id,
                    Name = refreshToken.User.Name,
                    Email = refreshToken.User.Email,
                    ProfilePictureUrl = refreshToken.User.ProfilePictureUrl,
                    isVerified = refreshToken.User.IsVerified
                }
            };
        }

        // Yardımcı Metot: Yeni Access Token için claim'leri oluşturma (Refresh işleminde kullanılır)
        private IEnumerable<Claim> GetClaimsForNewAccessToken(User user)
        {
            return new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Email, user.Email),
                new Claim(ClaimTypes.Name, user.Name),
                new Claim(ClaimTypes.Role, user.Role.ToString())
            };
        }

        // Eski bir Access Token'dan claim'leri almak için de kullanılabilirdi, ama user objesi mevcutken daha güvenli
        // private IEnumerable<Claim> GetClaimsFromExpiredToken(string token) { ... }
    }
}