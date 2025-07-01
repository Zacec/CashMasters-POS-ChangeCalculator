
namespace Shared
{
    public interface IDenominationService
    {
        IReadOnlyList<Denomination> Denominations { get; }

        Denomination GetDenominationByName(string denominationName);
    }
}