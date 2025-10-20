using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LjussAPI.Application.DTOs.AuthDTOs
{
    public class AuthResponseDto
    {
        public string Token { get; set; }
        public DateTime AccessTime { get; set; }
        public string RefreshToken { get; set; }
        public int ExpiresIn { get; set; }

        public bool IsVerified => User?.isVerified ?? false;
        public UserResponseDto User { get; set; }
    }
}
