using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LjussAPI.Application.DTOs.ShippingAddressDTOs
{
    public class UpdateShippingAddressDto
    {
        public Guid Id { get; set; }
        public string Title { get; set; } = null!;
        public string AddressLine1 { get; set; } = null!;
        public string? AddressLine2 { get; set; }
        public int CityId { get; set; }
        public int DistrictId { get; set; }
        public bool IsActive { get; set; }
    }
}
