using Contracts;
using Shared;
using Shared.Exceptions;

namespace POSRoutines
{
    public class ChangeCalculator : IChangeCalculator
    {
        private readonly IDenominationService _denominationService;

        public ChangeCalculator(IDenominationService denominationService)
        {
            _denominationService = denominationService ?? throw new ArgumentNullException(nameof(denominationService));
        }

        public Dictionary<Denomination, int> CalculateChange(decimal totalCost, Dictionary<Denomination, int> customersGivenMoney)
        {
            if (!ValidateCustomersGivenMoney(customersGivenMoney, out var failedDenomination))
                throw new DenominationNotValidException(failedDenomination, _denominationService.Currency);

            if (totalCost == 0)
                return customersGivenMoney;

            if (totalCost < 0)
                throw new TotalCostIsNegativeException(totalCost);

            var totalGivenByCustomer = customersGivenMoney.Sum(kv => kv.Key.Value * kv.Value);

            if (totalGivenByCustomer < totalCost)
                throw new PaymentLessThanTotalException(totalCost, totalGivenByCustomer);

            decimal change = totalGivenByCustomer - totalCost;

            if (change == 0)
                return new Dictionary<Denomination, int>();
            
            var changeInDenominations = GetChange(change);

            return changeInDenominations;
        }

        private Dictionary<Denomination, int> GetChange(decimal change)
        {
            Dictionary<Denomination, int> changeInDenominations = new();

            foreach (var denomination in _denominationService.Denominations.OrderByDescending(d => d.Value))
            {
                var count = (int)(change / denomination.Value);
                if (count <= 0)
                    continue;

                changeInDenominations[denomination] = count;
                change -= count * denomination.Value;

                if (change <= 0)
                    break;
            }

            return changeInDenominations;
        }

        private bool ValidateCustomersGivenMoney(Dictionary<Denomination, int> customersGivenMoney, out Denomination? failedDenomination)
        {
            failedDenomination = null;

            foreach (var denomination in customersGivenMoney)
                if (_denominationService.IsValidDenomination(denomination.Key.Value) is not true)
                {
                    failedDenomination = denomination.Key;
                    return false;
                }

            return true;
        }
    }
}
