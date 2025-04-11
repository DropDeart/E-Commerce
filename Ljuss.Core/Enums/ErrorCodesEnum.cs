using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ljuss.Core.Enums
{
    public enum ErrorCodesEnum
    {
        InternalServerError = 5000,
        WrongEmailAndPassword = 1001,
        EmailAlreadyExist = 1002,
        WrongVerificationCode = 1003,
        UserNotVerified = 1004,
        NotAgainSendVerificationCode = 1005
    }
}
