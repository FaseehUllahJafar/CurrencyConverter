using CurrencyConverter.Domain.Entities;

namespace CurrencyConverter.Domain.Interfaces;

public interface ICurrencyProvider
{
    Task<ExchangeRate> GetLatestRatesAsync(string baseCurrency);
    Task<ExchangeRate> GetHistoricalRatesAsync(string baseCurrency, DateTime from, DateTime to);
}
