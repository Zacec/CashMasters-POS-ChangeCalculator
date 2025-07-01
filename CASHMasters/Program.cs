using CASHMasters.Configuration;
using CASHMasters.PaymentServices;
using Contracts;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using POSRoutines;
using Shared;

internal class Program
{
    private static void Main(string[] args)
    {
        /// Configuration
        /// 
        var ServiceCollection = new ServiceCollection();
        var config = new ConfigurationBuilder()
            .AddJsonFile("appsettings.json")
            .Build();

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
        ServiceCollection.AddScoped<IChangeCalculator, ChangeCalculator>();

        var serviceProvider = ServiceCollection.BuildServiceProvider();
        var denominationService = serviceProvider.GetRequiredService<IDenominationService>();


        /// Application
        /// 
        bool exit = false;
        while (!exit)
        {
            try
            {
                Console.WriteLine("\n\rCASHMasters POS Change Calculator \n\r");

                decimal totalCost = CaptureTotalCost();
            
                var customersGivenMoney = CaptureDenominationsInput(denominationService.Denominations);            

                Console.WriteLine();

                var payment = new PaymentMethodCash(totalCost, customersGivenMoney, serviceProvider.GetRequiredService<IChangeCalculator>());
                payment.GetChange();

                Console.WriteLine("*******************************************************\n\rTransaction completed");
                Console.WriteLine("1. Start another transaction");
                Console.WriteLine("2. Exit");

                var input = Console.ReadLine();

                if (input == "2")
                    exit = true;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }
    }


    private static decimal CaptureTotalCost()
    {
        Console.WriteLine("Enter the total cost of the items purchased as a decimal: \n\r* in case of invalid input the TotalCost will be set to 0");
        decimal totalCost = 0;
        var input = Console.ReadLine();

        if (!decimal.TryParse(input, out totalCost))
            totalCost = 0;

        return totalCost;
    }

    private static Dictionary<Denomination, int> CaptureDenominationsInput(IEnumerable<Denomination> denominations)
    {
        var givenMoney = new Dictionary<Denomination, int>();

        Console.WriteLine("Enter the quantity for each denomination (leave blank for 0):");
        foreach (var denom in denominations)
        {
            int amount = 0;
            
            Console.Write($"{denom.Name} ({denom.Value}): ");                
            var input = Console.ReadLine();
                
            if (string.IsNullOrWhiteSpace(input))
                amount = 0;

            if(!int.TryParse(input, out amount))
                amount = 0;
            
            if (amount > 0)
                givenMoney[denom] = amount;
        }
        return givenMoney;
    }
}
