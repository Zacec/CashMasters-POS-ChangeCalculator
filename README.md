# CashMasters POS Change Calculator

A point-of-sale (POS) utility for calculating the optimum (i.e. minimum) number of bills and coins to return to the customer, implemented in the `POSRoutines.ChangeCalculator` class.

## Overview

The primary component is the `POSRoutines.ChangeCalculator` class, which is responsible for determining the most efficient way to return change to customers in a cash transaction.

## Features

- Calculates the minimal number of coins and bills for transactions.
- Configurable denominations for different currencies in appsettings.

## Usage

### Main API: `POSRoutines.ChangeCalculator`

The `ChangeCalculator` main logic is in the implementaion of `CalculateChange` method, this method takes two inputs:

- Price of the item(s) being purchased
- All bills and coins provided by the customer to pay for the item(s) as a Dictionary<Denomination, int>

The denominations are taken and validated against of a DenominationService injected to the class, which loads the denomination from the aplication settings. 
The function returns a Dictionary<Denomination, int> with the optimal distribution of change.

**Example usage:**

The ChangeCalculator can be used as a service:
```csharp
ServiceCollection.AddScoped<IChangeCalculator, ChangeCalculator>();
```

then can be injected in a bigger component:
```csharp
private readonly IChangeCalculator _changeCalculator;
.
.
.
var change = _changeCalculator.CalculateChange(TotalCost, CustomersGivenMoney);
```

## Configuration

You can configure the available denominations by modifying the appsettings.json file, from which the DenominationService loads it's configuration.

```json
{
  "LocalCurrency": "MXN",
  "Currencies": [
    {
      "Name": "MXN",
      "Denominations": [
        {
          "Value": 0.05,
          "Name": "Cinco Centavos"
        },
        {
          "Value": 0.10,
          "Name": "Diez Centavos"
        }
```
...

The DenominationService can be configured in the DI to load the available denominations as follows:

```csharp
ServiceCollection.AddSingleton<IDenominationService>((getConcrete) =>
{
    var currenciesConfig = new List<CurrencyConfiguration>();
    config.GetSection("Currencies").Bind(currenciesConfig);
    var localCurrencyConfig = config.GetValue<string>("LocalCurrency");

    return new DenominationService(
        currenciesConfig.Where(c => c.Name.Equals(localCurrencyConfig))
                        .SelectMany(c => c.Denominations)
                        .ToList(),
        localCurrencyConfig
    );
});
```
- `LocalCurrency`: Determines the local currencie from the country the POS is installed, since the POS can be configured for several countries.
- `Currencies`: Contains configuration about the available denominations per currency.
- The corresponding denominations are loaded to the DenominationService depending on the local currency and the it's configuration.
