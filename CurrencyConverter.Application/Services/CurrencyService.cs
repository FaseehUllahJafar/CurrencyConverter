using CurrencyConverter.Application.Interfaces;
using CurrencyConverter.Domain.Entities;
using CurrencyConverter.Domain.Interfaces;

namespace CurrencyConverter.Application.Services;

public class CurrencyService : ICurrencyService
{
    private readonly ICurrencyProvider _provider;

    private static readonly string[] BlockedCurrencies =
        { "TRY", "PLN", "THB", "MXN" };

    public CurrencyService(ICurrencyProvider provider)
    {
        _provider = provider;
    }

    public async Task<ExchangeRate> GetLatestAsync(string baseCurrency)
    {
        return await _provider.GetLatestRatesAsync(baseCurrency);
    }

    public async Task<decimal> ConvertAsync(string from, string to, decimal amount)
    {
        if (BlockedCurrencies.Contains(from) || BlockedCurrencies.Contains(to))
            throw new InvalidOperationException("Currency not supported.");

        var rates = await _provider.GetLatestRatesAsync(from);

        if (!rates.Rates.ContainsKey(to))
            throw new Exception("Target currency not found.");

        return amount * rates.Rates[to];
    }

    public async Task<object> GetHistoricalAsync(HistoricalRequest request)
    {
        var data = await _provider.GetHistoricalRatesAsync(
            request.BaseCurrency,
            request.From,
            request.To);

        var paged = data.Rates
            .Skip((request.Page - 1) * request.PageSize)
            .Take(request.PageSize);

        return new
        {
            request.Page,
            request.PageSize,
            Total = data.Rates.Count,
            Data = paged
        };
    }

}
