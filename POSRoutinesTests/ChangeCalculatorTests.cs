using POSRoutines;
using Shared;
using Shared.Exceptions;
using System.Collections;

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
            }, "USD");

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

            var result = Assert.Throws<PaymentLessThanTotalException>(() =>
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

        [Fact]
        public void CalculateChange_GivenMoneyIsZero_ReturnsException()
        {
            decimal totalCost = 15.00M;
            var customerGivenMoney = new Dictionary<Denomination, int>();
            var result = Assert.Throws<PaymentLessThanTotalException>(() =>
                _changeCalculator.CalculateChange(totalCost, customerGivenMoney));
        }

        [Fact]
        public void CalculateChange_TotalCostIsZero_ReturnsTheSameMoneyGivenByCustomer()
        {
            decimal totalCost = 0;
            var customerGivenMoney = new Dictionary<Denomination, int>
            {
                { _denominationService.GetDenominationByName("Five Dollar"), 2 },
                { _denominationService.GetDenominationByName("One Dollar"), 5 }
            };          

            var result = _changeCalculator.CalculateChange(totalCost, customerGivenMoney);

            Assert.Equal(customerGivenMoney, result);
        }

        [Fact]
        public void CalculateChange_GivenMoneyIsLowerThanZero_ReturnsException()
        {
            decimal totalCost = -1.10M;
            var customerGivenMoney = new Dictionary<Denomination, int>();
            var result = Assert.Throws<TotalCostIsNegativeException>(() =>
                _changeCalculator.CalculateChange(totalCost, customerGivenMoney));
        }

        [Fact]
        public void CalculateChange_GivenMoneyIsNotValid_ReturnsException()
        {
            decimal totalCost = 15.00M;
            Dictionary<Denomination, int> customerGivenMoney = new Dictionary<Denomination, int>
            {
                { new Denomination(200.00M, "Two Hundred Dollar"), 2 }                
            };
            var result = Assert.Throws<DenominationNotValidException>(() =>
                _changeCalculator.CalculateChange(totalCost, customerGivenMoney));
        }

        [Theory]
        [ClassData(typeof(Get_CalculateChange_GivenMoneyIsGreaterThanTotalCost_ReturnExpectedChange_Data))]
        public void CalculateChange_GivenMoneyIsGreaterThanTotalCost_ReturnExpectedChange(decimal totalCost, Dictionary<Denomination, int> givenMoney, Dictionary<Denomination, int> change)
        {
            var result = _changeCalculator.CalculateChange(totalCost, givenMoney);

            Assert.Equal(change, result);
        }
    }


    public class Get_CalculateChange_GivenMoneyIsGreaterThanTotalCost_ReturnExpectedChange_Data : IEnumerable<object[]>
    {
        public IEnumerator<object[]> GetEnumerator()
        {
            yield return new object[]
            {
                    15.00M,
                    new Dictionary<Denomination, int>
                    {
                        { new Denomination(20.00M, "Twenty Dollar"), 1 }
                    },
                    new Dictionary<Denomination, int>
                    {
                        { new Denomination(5.00M, "Five Dollar"), 1 }
                    }
            };
            yield return new object[]
            {
                    10.55M,
                    new Dictionary<Denomination, int>
                    {
                        { new Denomination(10.00M, "Ten Dollar"), 1 },
                        { new Denomination(1.00M, "One Dollar"), 1 }
                    },
                    new Dictionary<Denomination, int>
                    {
                        { new Denomination(0.25M, "Quarter"), 1 },
                        { new Denomination(0.10M, "Dime"), 2 }
                    }
            };
            yield return new object[]
            {
                    77.15M,
                    new Dictionary<Denomination, int>
                    {
                        { new Denomination(100.00M, "Hundred Dollar"), 1 }
                    },
                    new Dictionary<Denomination, int>
                    {
                        { new Denomination(20.00M, "Twenty Dollar"), 1 },
                        { new Denomination(2.00M, "Two Dollar"), 1 },
                        { new Denomination(0.50M, "Half Dollar"), 1 },
                        { new Denomination(0.25M, "Quarter"), 1 },
                        { new Denomination(0.10M, "Dime"), 1 }
                    }
            };
            yield return new object[]
            {
                    77.15M,
                    new Dictionary<Denomination, int>
                    {
                        { new Denomination(50.00M, "Fifty Dollar"), 2 }
                    },
                    new Dictionary<Denomination, int>
                    {
                        { new Denomination(20.00M, "Twenty Dollar"), 1 },
                        { new Denomination(2.00M, "Two Dollar"), 1 },
                        { new Denomination(0.50M, "Half Dollar"), 1 },
                        { new Denomination(0.25M, "Quarter"), 1 },
                        { new Denomination(0.10M, "Dime"), 1 }
                    }
            };
            yield return new object[]
            {
                    0.15M,
                    new Dictionary<Denomination, int>
                    {
                        { new Denomination(100.00M, "Hundred Dollar"), 2 }
                    },
                    new Dictionary<Denomination, int>
                    {
                        { new Denomination(100.00M, "Hundred Dollar"), 1 },
                        { new Denomination(50.00M, "Fifty Dollar"), 1 },
                        { new Denomination(20.00M, "Twenty Dollar"), 2 },
                        { new Denomination(5.00M, "Five Dollar"), 1 },
                        { new Denomination(2.00M, "Two Dollar"), 2 },
                        { new Denomination(0.50M, "Half Dollar"), 1 },
                        { new Denomination(0.25M, "Quarter"), 1 },
                        { new Denomination(0.10M, "Dime"), 1 }
                    }
            };
        }
        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

    }
}