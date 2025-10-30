
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ljuss.Core.Entities
{
    public class City
    {
        public int Id { get; set; } 
        public string Name { get; set; } = null!;
        public bool IsActive { get; set; }

        public ICollection<District>? Districts { get; set; }
    }
}
