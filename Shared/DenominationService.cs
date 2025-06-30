namespace Shared
{
    public sealed class DenominationService : IDenominationService
    {
        public IReadOnlyList<Denomination> Denominations { get; }

        public DenominationService(List<Denomination> denominations)
        {
            Denominations = denominations;
        }
    }
}
