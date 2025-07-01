namespace Shared
{
    public sealed class DenominationService : IDenominationService
    {
        public IReadOnlyList<Denomination> Denominations { get; }

        public DenominationService(List<Denomination> denominations)
        {
            Denominations = denominations;
        }

        public Denomination GetDenominationByName(string denominationName)
        {
            var denomination = Denominations.FirstOrDefault(d => d.Name.Equals(denominationName, StringComparison.OrdinalIgnoreCase));
            if (denomination == null)
            {
                throw new KeyNotFoundException($"Denomination with ID '{denominationName}' not found.");
            }

            return new Denomination(denomination.Value, denomination.Name);
        }
    }
}
