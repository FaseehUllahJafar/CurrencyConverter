using CurrencyConverter.Domain.Interfaces;

namespace CurrencyConverter.Infrastructure.Factory;

public interface ICurrencyProviderFactory
{
    ICurrencyProvider Create(string providerName);
}
