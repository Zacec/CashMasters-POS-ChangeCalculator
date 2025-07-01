namespace Shared
{
    public sealed class DenominationService : IDenominationService
    {
        public IReadOnlyList<Denomination> Denominations { get; init; }

        public string Currency { get; init; }

        
        public DenominationService(List<Denomination> denominations, string currency)
        {
            Denominations = denominations;
            Currency = currency;
        }

        
        public bool IsValidDenomination(string denominationName)
            => Denominations.Any(d => d.Name.Equals(denominationName, StringComparison.OrdinalIgnoreCase));

        public bool IsValidDenomination(decimal denominationValue)
            => Denominations.Any(d => d.Value == denominationValue);

        public Denomination GetDenominationByName(string denominationName)
        {
            var denomination = Denominations.FirstOrDefault(d => d.Name.Equals(denominationName, StringComparison.OrdinalIgnoreCase));
            if (denomination == null)
                throw new KeyNotFoundException($"Denomination with ID '{denominationName}' not found.");

            return new Denomination(denomination.Value, denomination.Name);
        }
    }
}
