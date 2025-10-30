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
        NotAgainSendVerificationCode = 1005,
        InvalidUser = 1006,
        ProductNotExist = 1007,
        ProductCanNotCreate = 1008,
        ProductCanNotUpdate = 1009,
        ProductCanNotDelete = 1010,
        CategoryNoxExist = 1011,
        CategoryCanNotCreate = 1012,
        CategoryCanNotUpdate = 1013,
        CategoryCanNotDelete = 1014,
        ShippinAddresNoExist = 1015,
        ShippinCanNotCreate = 1016,
        ShippinCanNotUpdate = 1017,
        ShippinCanNotDelete = 1018
    }
}
