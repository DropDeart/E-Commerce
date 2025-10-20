using Ljuss.Core.Entities;
using Ljuss.Core.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LjussAPI.Application.DTOs.AuthDTOs
{
    public class UserResponseDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public string? ProfilePictureUrl { get; set; }
        public bool isVerified { get; set; } = false;
        public RoleEnum Role { get; set; } = RoleEnum.User;

        public static implicit operator UserResponseDto(User v)
        {
            throw new NotImplementedException();
        }
    }
}
