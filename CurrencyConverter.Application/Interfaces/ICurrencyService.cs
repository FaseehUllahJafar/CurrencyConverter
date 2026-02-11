using CurrencyConverter.Domain.Entities;

namespace CurrencyConverter.Application.Interfaces;

public interface ICurrencyService
{
    Task<ExchangeRate> GetLatestAsync(string baseCurrency);
    Task<decimal> ConvertAsync(string from, string to, decimal amount);
    Task<object> GetHistoricalAsync(HistoricalRequest request);

}
