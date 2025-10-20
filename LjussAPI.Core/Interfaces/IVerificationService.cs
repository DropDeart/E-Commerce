using Ljuss.Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LjussAPI.Application.Interfaces
{
    public interface IVerificationService
    {
        public string GenerateVerificationCode();
        public Task<bool> SendVerificationCodeAsync(Guid userId, string email);
        public Task<bool?> VerifyCodeAsync(Guid userId, string code);

    }
}
