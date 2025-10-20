using Ljuss.Core.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ljuss.Core.Entities
{
    public class ProductAttribute : BaseEntity
    {
        public Guid ProductId { get; set; }
        public Product Product { get; set; }

        public Guid AttributeId { get; set; }
        public Attribute Attribute { get; set; }

        public Guid AttributeValueId { get; set; }
        public AttributeValue AttributeValue { get; set; }
    }
}
