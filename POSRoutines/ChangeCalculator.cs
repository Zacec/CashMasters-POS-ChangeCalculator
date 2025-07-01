using Shared;

namespace POSRoutines
{
    public class ChangeCalculator
    {
        private readonly IDenominationService _denominationService;

        public ChangeCalculator(IDenominationService denominationService)
        {
            _denominationService = denominationService ?? throw new ArgumentNullException(nameof(denominationService));
        }

        public Dictionary<Denomination, int> CalculateChange(decimal totalCost, Dictionary<Denomination, int> totalGiven)
        {
            Dictionary<Denomination, int> changeInDenominations = new Dictionary<Denomination, int>();

            var totalGivenByCustomer = GetTotalGivenByCustomer(totalGiven);

            if (totalGivenByCustomer < totalCost)
                throw new ArgumentException("Total given by customer must be greater than or equal to the total cost.");

            decimal change = totalGivenByCustomer - totalCost;

            if (change == 0)
                return changeInDenominations;

            var a = _denominationService.Denominations.OrderByDescending(d => d.Value);

            foreach (var denomination in _denominationService.Denominations.OrderByDescending(d => d.Value))
            {
                var count = (int)(change / denomination.Value);
                if (count < 0)
                    continue;

                changeInDenominations[denomination] = count;
                change -= count * denomination.Value;
                
                if (change <= 0)
                    break;
            }

            return changeInDenominations;
        }

        private decimal GetTotalGivenByCustomer(Dictionary<Denomination, int> totalGiven)
        {
            decimal total = 0;
            
            foreach (var denomination in totalGiven)
            {
                total += denomination.Key.Value * denomination.Value;
            }
            
            return total;
        }
    }
}
