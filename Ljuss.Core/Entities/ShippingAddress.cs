using Ljuss.Core.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ljuss.Core.Entities
{
    public class ShippingAddress : BaseEntity
    {
        public Guid UserId { get; set; }
        public User User { get; set; }

        public string Title { get; set; } = null!;
        public string AddressLine1 { get; set; }
        public string? AddressLine2 { get; set; }
        public int CityId { get; set; }
        public City City { get; set; } = null!;
        public int DistrictId { get; set; }
        public District District { get; set; } = null!;
        public bool IsActive { get; set; } = true;
    }
}
