using CurrencyConverter.Domain.Entities;
using CurrencyConverter.Domain.Interfaces;
using Microsoft.Extensions.Caching.Memory;
using System.Net.Http.Json;

namespace CurrencyConverter.Infrastructure.Providers;

public class FrankfurterProvider : ICurrencyProvider
{
    private readonly HttpClient _httpClient;
    private readonly IMemoryCache _cache;

    public FrankfurterProvider(HttpClient httpClient, IMemoryCache cache)
    {
        _httpClient = httpClient;
        _cache = cache;
    }

    public async Task<ExchangeRate> GetLatestRatesAsync(string baseCurrency)
    {
        string cacheKey = $"latest-{baseCurrency}";

        if (_cache.TryGetValue(cacheKey, out ExchangeRate cached))
            return cached;

        var response = await _httpClient.GetAsync($"latest?base={baseCurrency}");
        response.EnsureSuccessStatusCode();

        var data = await response.Content.ReadFromJsonAsync<ExchangeRate>();

        _cache.Set(cacheKey, data!, TimeSpan.FromMinutes(5));

        return data!;
    }

    public async Task<ExchangeRate> GetHistoricalRatesAsync(string baseCurrency, DateTime from, DateTime to)
    {
        var response = await _httpClient.GetAsync($"{from:yyyy-MM-dd}..{to:yyyy-MM-dd}?base={baseCurrency}");
        response.EnsureSuccessStatusCode();

        return await response.Content.ReadFromJsonAsync<ExchangeRate>() ?? new ExchangeRate();
    }
}
