using Contracts;
using Shared;

namespace CASHMasters.PaymentServices
{
    public class PaymentMethodCash : PaymentMethod
    {
        public Dictionary<Denomination, int> CustomersGivenMoney { get; set; } = new();

        private readonly IChangeCalculator _changeCalculator;

        public PaymentMethodCash(decimal totalCost, Dictionary<Denomination, int> customersGivenMoney, IChangeCalculator changeCalculator)
        {
            TotalCost = totalCost;
            CustomersGivenMoney = customersGivenMoney;
            _changeCalculator = changeCalculator;
        }

        public void GetChange()
        {
            var change = _changeCalculator.CalculateChange(TotalCost, CustomersGivenMoney);

            if (change.Count == 0)
                Console.WriteLine("No change to give back.");
            else
            {
                Console.WriteLine("Change to give back:");
                foreach (var denomination in change)
                    Console.WriteLine($"{denomination.Value} of {denomination.Key.Name} ({denomination.Key.Value})");
            }
        }

    }
}
