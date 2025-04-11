using Ljuss.Core.Domian;
using Ljuss.Core.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LjussAPI.Application.Interfaces
{
    public interface IJwtTokenService
    {
        public AuthResponseDto GenrateToken (User user);
    }
}
