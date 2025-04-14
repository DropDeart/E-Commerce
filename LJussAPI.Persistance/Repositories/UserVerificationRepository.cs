using Ljuss.Core.Domian;
using Ljuss.Core.Repositories;
using LjussAPI.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LjussAPI.Persistence
{
    public class UserVerificationRepository : IUserVerificationRepository
    {
        private readonly AppDbContext _appDbContext;
        private readonly ILogger<UserVerificationRepository> _logger;
        public UserVerificationRepository(AppDbContext appDbContext, ILogger<UserVerificationRepository> logger)
        {
            _appDbContext = appDbContext;
            _logger = logger;
        }
        public async Task AddVerificationCodeAsync(VerificationCode verificationCode)
        {
            if (verificationCode == null)
            {
                throw new ArgumentNullException(nameof(verificationCode));
            }

            try
            {              
                await _appDbContext.VerificationCodes.AddAsync(verificationCode);
                await _appDbContext.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                throw;                
            }
            
        }

        public async Task<VerificationCode> GetLastVerificationCodeAsync(Guid userId)
        {
            return await _appDbContext.VerificationCodes
                .Where(v => v.UserId == userId)
                .OrderByDescending(v => v.Expiration)
                .FirstOrDefaultAsync();
        }

        public async Task<VerificationCode?> GetVerificationCodeAsync(string code)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(code))
                {
                    throw new ArgumentNullException(nameof(code));
                }

                return await _appDbContext.VerificationCodes
                    .Where(x => x.Code == code && !x.IsVerified)
                    .OrderByDescending(x => x.CreatedAt) // En güncel kodu almak için sıralama ekledik.
                    .FirstOrDefaultAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching verification code.");
                throw;
            }
        }

        public async Task UpdateVerificationCodeAsync(VerificationCode verificationCode)
        {
            if(verificationCode == null)
            {
                throw new ArgumentNullException(nameof(verificationCode));
            }

            try
            {
                _appDbContext.VerificationCodes.Update(verificationCode);
                await _appDbContext.SaveChangesAsync();
            }
            catch(Exception ex)
            {
                _logger.LogError(ex.Message);
                throw;
            }
            

        }
    }
}
