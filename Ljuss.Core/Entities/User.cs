using Ljuss.Core.Common;
using Ljuss.Core.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ljuss.Core.Entities
{
    public class User : BaseEntity
    {
        public string Name { get; set; }
        public string Email { get; set; }
        public string PasswordHash { get; set; }
        public string? ProfilePictureUrl { get; set; }
        public string? PhoneNumber { get; set; }
        public bool IsVerified { get; set; } = false;
        public RoleEnum Role { get; set; } = RoleEnum.User;

        public ICollection<VerificationCode> VerificationCodes { get; set; }

        public ICollection<ShippingAddress> ShippingAddresses { get; set; } = new List<ShippingAddress>();

        public ICollection<Order> Orders { get; set; } = new List<Order>();

        public ICollection<RefreshToken> RefreshTokens { get; set; } = new List<RefreshToken>();
    }
}
