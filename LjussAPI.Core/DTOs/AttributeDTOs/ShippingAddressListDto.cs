using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LjussAPI.Application.DTOs.AttributeDTOs
{
    public class ShippingAddressListDto
    {
        public Guid Id { get; set; }
        public string Title {  get; set; }
        public string Address { get; set; }
    }
}
