using Ljuss.Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LjussAPI.Application.DTOs.ShippingAddressDTOs
{
    public class ShippingAddressDto
    {
        public Guid Id { get; set; }
        public string Title { get; set; } = null!;
        public string AddressLine1 { get; set; } = null!;
        public string? AddressLine2 { get; set; }
        public int CityId { get; set; }
        public string CityName { get; set; } = null!;
        public int DistrictId { get; set; }
        public string DistrictName { get; set; } = null!;
        public bool IsActive { get; set; }
    }
}
