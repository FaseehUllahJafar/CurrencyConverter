using CurrencyConverter.Domain.Interfaces;
using Microsoft.Extensions.DependencyInjection;

namespace CurrencyConverter.Infrastructure.Factory;

public class CurrencyProviderFactory : ICurrencyProviderFactory
{
    private readonly IServiceProvider _serviceProvider;

    public CurrencyProviderFactory(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    public ICurrencyProvider Create(string providerName)
    {
        return providerName switch
        {
            "Frankfurter" => _serviceProvider.GetRequiredService<ICurrencyProvider>(),
            _ => throw new NotSupportedException("Provider not supported")
        };
    }
}
