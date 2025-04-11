using Ljuss.Core.Domian;
using Ljuss.Core.Repositories;
using LjussAPI.Application.Interfaces;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LjussAPI.Infrastructure.Services
{
    public class VerificationService : IVerificationService
    {
        private readonly IEmailService _emailService;
        private readonly IUserVerificationRepository _userVerificationRepository;
        private readonly ILogger<VerificationService> _logger;
        public VerificationService(IEmailService emailService, IUserVerificationRepository userVerificationRepository, ILogger<VerificationService> logger)
        {
            _emailService = emailService;
            _userVerificationRepository = userVerificationRepository;
            _logger = logger;
        }

        public async Task<bool> SendVerificationCodeAsync(Guid userId, string email)
        {
            if(String.IsNullOrWhiteSpace(email) || String.IsNullOrWhiteSpace(userId.ToString()))
            {
                throw new ArgumentNullException(nameof(email));
            }

            var lastCode = await _userVerificationRepository.GetLastVerificationCodeAsync(userId);

            if (lastCode != null && (DateTime.UtcNow - lastCode.CreatedAt).TotalMinutes < 3)
            {
                throw new InvalidOperationException("Yeni bir doğrulama kodu talep etmek için lütfen 3 dakika bekleyin.");
            }

            try
            {
                var verificationCode = GenerateVerificationCode();

                var code = new VerificationCode
                {
                    UserId = userId,
                    Code = verificationCode,
                    Email = email,
                    Expiration = DateTime.UtcNow.AddHours(1),
                    IsVerified = false
                };

                await _userVerificationRepository.AddVerificationCodeAsync(code);

                #region Email Properties

                var subject = "E-posta Doğrulama Kodu";
                var body = $"Lütfen doğrulama kodunu giriniz : {verificationCode}";

                #endregion

                await _emailService.SendEmailAsync(email, subject, body);

                return true;

            }
            catch (Exception ex) 
            {
                _logger.LogError(ex.Message);
                throw;
            }           
        }
        public async Task<bool?> VerifyCodeAsync(Guid userId, string code)
        {
            var verification = await _userVerificationRepository.GetVerificationCodeAsync(code);

            if(verification == null || verification.Code != code)
            {
                return null;
            }

            if (verification.Expiration < DateTime.UtcNow)
            {
                throw new Exception("Aktivasyon süresi dolmuştur, lütfen tekrar deneyin.");
            }

            verification.IsVerified = true;

            await _userVerificationRepository.UpdateVerificationCodeAsync(verification);
            return true;
        }
                
        public string GenerateVerificationCode()
        {
            Random random = new Random();
            return random.Next(100000, 999999).ToString();
        }

        
    }
}
