using LjussAPI.Application.DTOs.CommentDTOs;
using LjussAPI.Application.DTOs.ImageDTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LjussAPI.Application.DTOs.ProductDTOs
{
    public class ProductDetailDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string? Description { get; set; }
        public decimal Price { get; set; }
        public int Stock { get; set; }
        public Guid CategoryId { get; set; }
        public string? CategoryName { get; set; }
        public decimal TaxRate { get; set; }
        public decimal? DiscountRate { get; set; }
        public DateTime? DiscountStart { get; set; }
        public DateTime? DiscountEnd { get; set; }

        public List<ProductAttributeDto>? Attributes { get; set; }
        public List<ImageDto>? Images { get; set; }
        public List<CommentDto>? Comments { get; set; }
    }
}
