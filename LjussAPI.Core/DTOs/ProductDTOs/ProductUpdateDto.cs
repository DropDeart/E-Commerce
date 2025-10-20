using LjussAPI.Application.DTOs.ImageDTOs;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LjussAPI.Application.DTOs.ProductDTOs
{
    public class ProductUpdateDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string? Description { get; set; }
        public decimal Price { get; set; }
        public int Stock { get; set; }
        public Guid CategoryId { get; set; }
        public string? DeletedImageIds { get; set; }
        public decimal TaxRate { get; set; }
        public decimal? DiscountRate { get; set; }
        public DateTime? DiscountStart { get; set; }
        public DateTime? DiscountEnd { get; set; }  
        public DateTime UpdateDate { get; set; }

        public List<ImageUploadRequestDto>? Images { get; set; }
    }
}
