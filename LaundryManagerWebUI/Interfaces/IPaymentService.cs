using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LaundryManagerWebUI.Interfaces
{
    public  interface IPaymentService
    {
        Task<string> InitiazlizePayment();
    }
}
