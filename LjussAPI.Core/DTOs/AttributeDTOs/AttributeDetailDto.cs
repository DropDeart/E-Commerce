using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LjussAPI.Application.DTOs.AttributeDTOs
{
    public class AttributeDetailDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public List<AttributeDetailDto> Attributes { get; set; }

    }
}
