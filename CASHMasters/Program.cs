using CASHMasters.Configuration;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Shared;

internal class Program
{
    private static void Main(string[] args)
    {
        Console.WriteLine("Hello, World!");

        var ServiceCollection = new ServiceCollection();
        var config = new ConfigurationBuilder()
            .AddJsonFile("appsettings.json")
            .Build();

        ServiceCollection.AddSingleton<IDenominationService>((getConcrete) => { 
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

        ServiceCollection.BuildServiceProvider();
    }
}
