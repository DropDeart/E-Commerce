using Ljuss.Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LjussAPI.Application.DTOs.AuthDTOs;

namespace LjussAPI.Application.Interfaces
{
    public interface IJwtTokenService
    {
        // IP adresi parametresi eklendi
        AuthResponseDto GenrateToken(User user, string ipAddress);

        // Yeni: Refresh Token oluşturma
        RefreshToken GenrateRefreshToken(string ipAddress);

        // Yeni: Refresh Token ile yeni Access/Refresh Token alma
        // Task<AuthResponseDto> dönecek çünkü DB işlemleri async
        Task<AuthResponseDto> RefreshTokenAsync(string token, string ipAddress); // Metodun adı RefreshTokenAsync oldu
    }
}
