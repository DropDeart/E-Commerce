using Ljuss.Core.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ljuss.Core.Domian
{
    public class User
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public string PasswordHash { get; set; }
        public string? ProfilePictureUrl { get; set; }
        public bool IsVerified { get; set; } = false;
        public RoleEnum Role { get; set; } = RoleEnum.User;

        public ICollection<VerificationCode> VerificationCodes { get; set; }

    }
}
