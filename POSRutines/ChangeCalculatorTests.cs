using POSRoutines;
using Shared;

namespace POSRoutinesTests
{
    public class ChangeCalculatorTests
    {
        private readonly IDenominationService _denominationService;
        private readonly ChangeCalculator _changeCalculator;

        public ChangeCalculatorTests()
        {
            _denominationService = new DenominationService(new List<Denomination> {
                new Denomination(0.01M, "Penny"),
                new Denomination(0.05M, "Nickel"),
                new Denomination(0.10M, "Dime"),
                new Denomination(0.25M, "Quarter"),
                new Denomination(0.50M, "Half Dollar"),
                new Denomination(1.00M, "One Dollar"),
                new Denomination(2.00M, "Two Dollar"),
                new Denomination(5.00M, "Five Dollar"),
                new Denomination(10.00M, "Ten Dollar"),
                new Denomination(20.00M, "Twenty Dollar"),
                new Denomination(50.00M, "Fifty Dollar"),
                new Denomination(100.00M, "Hundred Dollar")
            });

            _changeCalculator = new ChangeCalculator(_denominationService);
        }

        [Fact]
        public void CalculateChange_GivenMoneyIsLessThanTotalCost_ReturnsException()
        {
            decimal totalCost = 15.50M;
            var customerGivenMoney = new Dictionary<Denomination, int>
            {
                { _denominationService.GetDenominationByName("Five Dollar"), 1 },
                { _denominationService.GetDenominationByName("One Dollar"), 2 }
            };

            var result = Assert.Throws<ArgumentException>(() =>
                _changeCalculator.CalculateChange(totalCost, customerGivenMoney));
        }

        [Fact]
        public void CalculateChange_GivenMoneyEqualsTotalCost_ReturnsEmptyDictionary()
        {
            decimal totalCost = 15.00M;
            var customerGivenMoney = new Dictionary<Denomination, int>
            {
                { _denominationService.GetDenominationByName("Five Dollar"), 2 },
                { _denominationService.GetDenominationByName("One Dollar"), 5 }
            };

            var result = _changeCalculator.CalculateChange(totalCost, customerGivenMoney);

            Assert.Empty(result);
        }

    }
}