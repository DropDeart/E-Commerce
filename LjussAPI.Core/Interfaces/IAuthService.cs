using Ljuss.Core.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LjussAPI.Application.Interfaces
{
    public interface IAuthService
    {
        Task<AuthResponseDto?> AuthenticateAsync(LoginRequestDto loginDto);
        Task<AuthResponseDto?> RegisterAsync(UserCreateDto userCreateDto);
        public string GetUserIdFromToken(string token);
    }
}
