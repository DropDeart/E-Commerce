using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ljuss.Core.Entities
{
    public class RefreshToken
    {
        public Guid Id { get; set; }
        public string Token { get; set; } // Refresh token değeri
        public DateTime Expires { get; set; } // Geçerlilik süresi
        public DateTime Created { get; set; } // Oluşturulma zamanı
        public string CreatedByIp { get; set; } // Oluşturan IP adresi (opsiyonel, güvenlik takibi için)
        public DateTime? Revoked { get; set; } // İptal edilme zamanı (null ise aktif)
        public string? RevokedByIp { get; set; } // İptal eden IP adresi (opsiyonel)
        public string? ReplacedByToken { get; set; } // Yeni bir token ile değiştirildiyse (single-use token rotation için)

        // Foreign Key
        public Guid UserId { get; set; }
        public User User { get; set; } // Navigation property

        public bool IsExpired => DateTime.UtcNow >= Expires; // Süresi doldu mu?
        public bool IsRevoked => Revoked != null; // İptal edildi mi?
        public bool IsActive => !IsRevoked && !IsExpired; // Aktif mi?
    }
}
