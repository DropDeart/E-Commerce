using Ljuss.Core.Domian;
using Ljuss.Applications.DTOs;
using Ljuss.Core.Enums;
using Ljuss.Core.Repositories;
using LjussAPI.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;
using NLog;
using ILogger = NLog.ILogger;

namespace LjussAPI.Controllers
{
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;
        private readonly ILogger _logger;
        private readonly IVerificationService _verificationService;
        private readonly IUserRepository _userRepository;
        public AuthController(IAuthService authService, IVerificationService verificationService, IUserRepository userRepository)
        {
            _authService = authService;      
            _verificationService = verificationService;
            _userRepository = userRepository;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] UserCreateDto userCreateDto)
        {
            try
            {
                // Kullanıcıyı kaydet
                var token = await _authService.RegisterAsync(userCreateDto);

                if (token == null)
                {
                    var errorResponse = new ErrorResponseDto(
                        "Email already exist!",
                        "Email already exist. Please use to new email and password!",
                        ErrorCodesEnum.EmailAlreadyExist
                    );
                    return BadRequest(new
                    {
                        errorMessage = "Email zaten kayıtlı",
                    });
                }

                return Ok(new { Token = token });
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "An error occurred while registering the user.");
                return StatusCode(500, "Internal server error. Please try again later.");
            }
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequestDto loginRequestDto)
        {
            try
            {
                var token = await _authService.AuthenticateAsync(loginRequestDto);

                if (token == null)
                {
                    var errorResponse = new ErrorResponseDto(
                        "Email Verification Required",
                        "Please verify your email address. A new verification code has been sent.",
                        ErrorCodesEnum.EmailAlreadyExist
                    );
                    return Unauthorized(new
                    {
                        errorMessage = "E-posta veya şifre hatalı.",
                    });
                }

                var user = await _userRepository.GetUserByEmailAsync(loginRequestDto.Email);

                if (!user.IsVerified)
                {
                    await _verificationService.SendVerificationCodeAsync(user.Id, user.Email);
                    var errorResponse = new ErrorResponseDto(
                        "Email Verification Required",
                        "Please verify your email address. A new verification code has been sent.",
                        ErrorCodesEnum.UserNotVerified
                    );

                    return BadRequest(new
                    {
                        redirectUrl = "/kullanici-dogrulama",
                        errorMessage = "E-posta doğrulaması gerekiyor.",
                        user = new
                        {
                            user.Id,
                            user.Email,
                            user.Name,
                        }
                    });
                }

                return Ok(new { Token = token });
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "An error occurred while logging in.");
                return StatusCode(500, "Internal server error. Please try again later.");
            }
        }


        [HttpPost("verify")]
        public async Task<IActionResult> VerifyCode([FromBody] VerifyCodeDto verifyCodeDto)
        {
            try
            {               
                var verificationResult = await _verificationService.VerifyCodeAsync(verifyCodeDto.UserId, verifyCodeDto.Code);

                if ((bool)!verificationResult)
                {
                    return BadRequest("Invalid verification code.");
                }

                var user = await _userRepository.GetUserByIdAsync(verifyCodeDto.UserId);
                if (user == null)
                {
                    _logger.Error($"User with ID {verifyCodeDto.UserId} not found.");
                    return NotFound("User not found.");
                }

                if (user != null)
                {
                    user.IsVerified = true;
                    await _userRepository.UpdateUserAsync(user);
                }

                return Ok(new { Message = "Verification successful" });
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "An error occurred during code verification.");
                return StatusCode(500, "Internal server error. Please try again later.");
            }
        }       

    }

}

