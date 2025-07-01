
namespace Shared
{
    public interface IDenominationService
    {
        IReadOnlyList<Denomination> Denominations { get; }
        string Currency { get; init; }

        Denomination GetDenominationByName(string denominationName);
        bool IsValidDenomination(string denominationName);
        bool IsValidDenomination(decimal denominationValue);
    }
}