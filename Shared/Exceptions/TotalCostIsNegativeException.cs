using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared.Exceptions
{
    public class TotalCostIsNegativeException : ArgumentException
    {
        public TotalCostIsNegativeException(decimal totalCost)
        : base($"Total cost cannot be lower than zero. Provided value: {totalCost}.")
        {
        }
    }
}
