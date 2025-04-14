using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ljuss.Applications.DTOs
{
    public class UserCreateDto
    {
        public string Name {  get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
    }
}
