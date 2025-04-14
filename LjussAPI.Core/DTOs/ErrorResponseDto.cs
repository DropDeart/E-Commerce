using Ljuss.Core.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ljuss.Applications.DTOs
{
    public class ErrorResponseDto
    {
        public string Title { get; set; }
        public string ErrorMessage { get; set; }
        public ErrorCodesEnum ErrorCodes { get; set; }

        public ErrorResponseDto(string title, string errorMessage, ErrorCodesEnum errorCodes)
        {
            Title = title;
            ErrorMessage = errorMessage;
            ErrorCodes = errorCodes;
        }

    }
}
