using Shared;

namespace Contracts
{
    public interface IChangeCalculator
    {
        Dictionary<Denomination, int> CalculateChange(decimal totalCost, Dictionary<Denomination, int> customersGivenMoney);
    }
}