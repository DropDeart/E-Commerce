using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ljuss.Core.DTOs
{
    public class AuthResponseDto
    {
        public string Token { get; set; }
        public DateTime AccessTime { get; set; }
        public bool IsVerified => User?.isVerified ?? false;
        public UserResponseDto User { get; set; }
    }
}
