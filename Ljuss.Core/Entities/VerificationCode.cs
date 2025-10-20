using Ljuss.Core.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ljuss.Core.Entities
{
    public class VerificationCode : BaseEntity
    {
        public Guid UserId { get; set; }
        public string Code { get; set; }
        public string Email { get; set; }
        public DateTime Expiration { get; set; }
        public bool IsVerified { get; set; }

        public User User { get; set; }
    }
}
