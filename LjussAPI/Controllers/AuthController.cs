using Microsoft.AspNetCore.Mvc;
using LjussAPI.Application.DTOs.AuthDTOs;
using Ljuss.Core.Entities;
using LjussAPI.Application.Interfaces;
using Microsoft.Extensions.Logging; // ILogger<T> için
using Microsoft.Extensions.Options; // JwtOptions için
using Ljuss.Core.Configurations; // JwtOptions sınıfınız için
using LjussAPI.Application.DTOs.UserDTOs; // UserCreateDto için
using LjussAPI.Application.DTOs.MiddlewareDTOs;
using Ljuss.Core.Repositories; // ErrorResponseDto için

namespace LjussAPI.Api.Controllers // veya LjussAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;
        private readonly ILogger<AuthController> _logger; // ILogger<AuthController> kullanıyoruz
        private readonly IVerificationService _verificationService; // Eğer burada kullanılıyorsa
        private readonly IUserRepository _userRepository; // Eğer burada kullanılıyorsa
        private readonly JwtOptions _jwtOptions; // Cookie ayarları için JWT seçenekleri
        private readonly IWebHostEnvironment _env;

        public AuthController(
            IAuthService authService,
            IVerificationService verificationService, // Eğer controller'da doğrudan kullanılıyorsa
            IUserRepository userRepository, // Eğer controller'da doğrudan kullanılıyorsa
            ILogger<AuthController> logger, // ILogger<AuthController> enjekte ediliyor
            IOptions<JwtOptions> jwtOptions,
            IWebHostEnvironment env
        )
        {
            _authService = authService;
            _verificationService = verificationService;
            _userRepository = userRepository;
            _logger = logger; // Atama doğru tipte yapılıyor
            _jwtOptions = jwtOptions.Value; // JwtOptions değerini alıyoruz
            _env = env;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] UserCreateDto userCreateDto)
        {
            try
            {
                var authResponse = await _authService.RegisterAsync(userCreateDto);

                if (authResponse == null)
                {
                    _logger.LogWarning($"Registration failed for email: {userCreateDto.Email} - Email already exists.");
                    // FRONTEND: `res.error` olarak yakalanacak, `errorCode` ve `errorMessage` ile işlenecek.
                    return BadRequest(new { errorMessage = "Email zaten kayıtlı", errorCode = Ljuss.Core.Enums.ErrorCodesEnum.EmailAlreadyExist });
                }

                if (authResponse.Token == null && !authResponse.User.isVerified)
                {
                    
                    _logger.LogInformation($"New user registered but not verified: {userCreateDto.Email}");
                    return Ok(authResponse);
                }

                _logger.LogInformation($"User {userCreateDto.Email} registered and authenticated successfully.");
                SetRefreshTokenCookie(authResponse.RefreshToken);
                authResponse.RefreshToken = null; 

                return Ok(authResponse);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while registering the user with email: {email}", userCreateDto.Email);
                return StatusCode(500, new { errorMessage = "Sunucu hatası oluştu. Lütfen daha sonra tekrar deneyin.", errorCode = Ljuss.Core.Enums.ErrorCodesEnum.InternalServerError });
            }
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequestDto loginRequestDto)
        {
            try
            {
                var authResponse = await _authService.AuthenticateAsync(loginRequestDto);

                // 1. Senaryo: Geçersiz kimlik bilgileri (e-posta yok veya şifre yanlış)
                if (authResponse == null)
                {
                    _logger.LogWarning($"Login failed for email: {loginRequestDto.Email} - Invalid credentials.");
                    // Unauthorized (401) dönmek daha uygun, çünkü kimlik doğrulama başarısız.
                    return Unauthorized(new { errorMessage = "E-posta veya şifre hatalı.", errorCode = Ljuss.Core.Enums.ErrorCodesEnum.InvalidUser });
                }

                // 2. Senaryo: Kullanıcı doğrulanmamış (Servis zaten token=null, isVerified=false döndürüyor)
                // authResponse.Token'ın null olması zaten doğrulanmamış olduğunu gösterir.
                if (!authResponse.User.isVerified) // Veya authResponse.Token == null
                {
                    // Doğrulama kodu gönderme işlemi burada kalmalı
                    await _verificationService.SendVerificationCodeAsync(authResponse.User.Id, authResponse.User.Email);
                    _logger.LogInformation($"User {authResponse.User.Email} is not verified. Sent new verification code.");

                    // **** DÜZELTME BURADA ****
                    // Artık BadRequest ve anonim obje dönmüyoruz.
                    // Servisimizden gelen AuthResponseDto'yu Ok ile döndürüyoruz.
                    // Frontend (NextAuth'ın authorize callback'i) bu AuthResponseDto'yu alıp işleyecek.
                    // authResponse zaten token=null, isVerified=false içeriyor.
                    return Ok(authResponse); // Doğrulanmamış kullanıcı için AuthResponseDto'yu döndür
                                             // ************************
                }

                // 3. Senaryo: Başarılı ve doğrulanmış giriş
                // Bu noktaya geldiysek, authResponse.Token null değildir ve authResponse.User.isVerified true'dur.
                _logger.LogInformation($"User {authResponse.User.Email} authenticated successfully. Tokens issued.");
                SetRefreshTokenCookie(authResponse.RefreshToken);
                authResponse.RefreshToken = null; // Güvenlik için response body'sinde null'a set et

                return Ok(authResponse); // Token'ları içeren AuthResponseDto'yu döndür
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while logging in for email: {email}", loginRequestDto.Email);
                return StatusCode(500, new { errorMessage = "Sunucu hatası oluştu. Lütfen tekrar deneyin.", errorCode = Ljuss.Core.Enums.ErrorCodesEnum.InternalServerError });
            }
        }

        [HttpPost("refreshToken")]
        public async Task<IActionResult> RefreshToken()
        {
            var refreshToken = Request.Cookies["refreshToken"]; // Çerezden refresh token'ı al

            if (string.IsNullOrEmpty(refreshToken))
            {
                _logger.LogWarning("Refresh token not found in cookies during refresh request.");
                ClearRefreshTokenCookie(); // Eğer çerez yoksa, temizlemeyi denemekte fayda var
                return Unauthorized(new { message = "Refresh token bulunamadı. Lütfen tekrar giriş yapın." });
            }

            var ipAddress = HttpContext.Connection.RemoteIpAddress?.ToString() ?? "N/A";

            try
            {
                // AuthService üzerinden Refresh Token işlemini yap
                var response = await _authService.RefreshTokenAsync(refreshToken, ipAddress);

                // Yeni Refresh Token'ı çerez olarak ayarla (önceki refresh token'ı değiştirir)
                SetRefreshTokenCookie(response.RefreshToken);
                response.RefreshToken = null; // Güvenlik için response body'sinde null'a set et

                _logger.LogInformation($"Token refreshed successfully for IP: {ipAddress}");
                return Ok(response);
            }
            catch (ApplicationException ex) // Uygulama özelindeki beklenen hatalar
            {
                _logger.LogWarning(ex, "Refresh token işlemi başarısız: {message}", ex.Message);
                ClearRefreshTokenCookie(); // Refresh token geçersizse, süresi dolduysa veya iptal edildiyse, kullanıcıyı log out et
                return Unauthorized(new { message = ex.Message });
            }
            catch (Exception ex) // Beklenmedik genel hatalar
            {
                _logger.LogError(ex, "Beklenmedik bir hata oluştu refresh token sırasında.");
                ClearRefreshTokenCookie(); // Herhangi bir hata durumunda da çerezi temizle
                return StatusCode(500, new { message = "Beklenmedik bir hata oluştu. Lütfen daha sonra tekrar deneyin." });
            }
        }

        [HttpPost("verify")]
        public async Task<IActionResult> VerifyCode([FromBody] VerifyCodeDto verifyCodeDto)
        {
            try
            {
                var verificationResult = await _verificationService.VerifyCodeAsync(verifyCodeDto.UserId, verifyCodeDto.Code);

                if (!(bool)verificationResult) // Eğer doğrulama başarısızsa (false veya null)
                {
                    _logger.LogWarning($"Invalid verification code '{verifyCodeDto.Code}' for user ID: {verifyCodeDto.UserId}");
                    return BadRequest("Invalid verification code.");
                }

                var user = await _userRepository.GetUserByIdAsync(verifyCodeDto.UserId);
                if (user == null)
                {
                    _logger.LogError($"User with ID {verifyCodeDto.UserId} not found during verification.");
                    return NotFound("User not found.");
                }

                user.IsVerified = true;
                await _userRepository.UpdateUserAsync(user);

                _logger.LogInformation($"User {user.Email} (ID: {user.Id}) successfully verified.");
                return Ok(new { Message = "Verification successful" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred during code verification for user ID: {userId}", verifyCodeDto.UserId);
                return StatusCode(500, "Internal server error. Please try again later.");
            }
        }

        // --- Helper Metotlar ---
        private void SetRefreshTokenCookie(string? token) // token nullable olabilir
        {
            if (string.IsNullOrEmpty(token))
            {
                _logger.LogWarning("Attempted to set null or empty refresh token cookie.");
                return;
            }

            var cookieOptions = new CookieOptions
            {
                HttpOnly = true,
                Secure = !_env.IsDevelopment(), // Geliştirmede false, prodda true
                Expires = DateTime.UtcNow.AddDays(_jwtOptions.RefreshTokenExpiryDays),
                SameSite = SameSiteMode.None
            };
            Response.Cookies.Append("refreshToken", token, cookieOptions);
            _logger.LogDebug("Refresh token cookie set successfully.");
        }

        private void ClearRefreshTokenCookie()
        {
            Response.Cookies.Delete("refreshToken", new CookieOptions
            {
                HttpOnly = true,
                Secure = true, // Güvenli çerez silerken de Secure=true olmalı
                SameSite = SameSiteMode.None
            });
            _logger.LogDebug("Refresh token cookie cleared.");
        }
    }
}