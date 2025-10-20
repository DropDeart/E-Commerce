using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LjussAPI.Application.DTOs.ProductDTOs
{
    public class ProductAttributeDto
    {
        public Guid AttributeId { get; set; }
        public string AttributeName { get; set; }
        public Guid AttributeValueId { get; set; }
        public string AttributeValue { get; set; }
    }
}
