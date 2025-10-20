using Ljuss.Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ljuss.Core.Repositories
{
    public interface IUserVerificationRepository
    {
        public Task AddVerificationCodeAsync(VerificationCode verificationCode);
        public Task<VerificationCode?> GetVerificationCodeAsync(string code);
        public Task<VerificationCode> GetLastVerificationCodeAsync(Guid userId);
        public Task UpdateVerificationCodeAsync(VerificationCode verificationCode);
    }
}
