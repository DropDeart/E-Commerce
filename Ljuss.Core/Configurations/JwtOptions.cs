using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ljuss.Core.Configurations
{
    public class JwtOptions
    {
        public string Key { get; set; }
        public string Issuer { get; set; }
        public string Audience { get; set; }
        public int ExpiryMinutes { get; set; } = 15; // Varsayılan 15 dakika
        public int RefreshTokenExpiryDays { get; set; } = 7; // Varsayılan 7 gün
    }
}
