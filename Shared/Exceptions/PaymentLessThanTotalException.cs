using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared.Exceptions
{
    public class PaymentLessThanTotalException : ArgumentException
    {
        public PaymentLessThanTotalException(decimal totalCost, decimal totalGivenByCustomer)
            : base($"The total given by customer ({totalGivenByCustomer}) is less than the total cost ({totalCost}).")
        { }
    }
}
