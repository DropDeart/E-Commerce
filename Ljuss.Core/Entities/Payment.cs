using Ljuss.Core.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ljuss.Core.Entities
{
    public class Payment : BaseEntity
    {
        public Guid OrderId { get; set; }
        public Order Order { get; set; }

        public string PaymentMethod { get; set; } // CreditCard, BankTransfer
        public string Status { get; set; } // Paid, Failed, Pending
        public DateTime PaymentDate { get; set; }
    }
}
