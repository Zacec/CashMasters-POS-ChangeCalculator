using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared.Exceptions
{
    public class DenominationNotValidException : ArgumentException
    {
        public DenominationNotValidException(Denomination denomination, string currency) 
            : base($"The denomination {denomination.Name} with value {denomination.Value}, is not valid in {currency} currency")
        {
        }
        
    }
}
