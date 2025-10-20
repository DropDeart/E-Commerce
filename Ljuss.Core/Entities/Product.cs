using Ljuss.Core.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ljuss.Core.Entities
{
    public class Product : BaseEntity
    {
        public Product()
        {
            ProductAttributes = new List<ProductAttribute>();
            ProductImages = new List<Image>();
            Comments = new List<Comment>();
            Favorites = new List<Favorite>();
        }

        public string Name { get; set; }
        public string? Description { get; set; }
        public decimal Price { get; set; }
        public int Stock { get; set; }
        public decimal TaxRate { get; set; }
        public decimal? DiscountRate { get; set; } 
        public DateTime? DiscountStart { get; set; }
        public DateTime? DiscountEnd { get; set; }
        public Guid CategoryId { get; set; }
        public Category Category { get; set; }


        public ICollection<ProductAttribute> ProductAttributes { get; set; }
        public ICollection<Image> ProductImages { get; set; }
        public ICollection<Comment> Comments { get; set; }
        public ICollection<Favorite> Favorites { get; set; }
    }
}
