using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LjussAPI.Application.DTOs.AttributeDTOs
{
    public class AttributeValueDto
    {
        public Guid Id { get; set; }
        public string Value { get; set; }
        public Guid AttributeId { get; set; }
    }
}
