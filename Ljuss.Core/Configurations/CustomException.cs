using Ljuss.Core.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ljuss.Core.Configurations
{
    public class CustomException : Exception
    {
        public ErrorCodesEnum ErrorCodes { get; set; }
        public CustomException(string message, ErrorCodesEnum errorCode)
        : base(message)
        {
            ErrorCodes = errorCode;
        }
    }
}
