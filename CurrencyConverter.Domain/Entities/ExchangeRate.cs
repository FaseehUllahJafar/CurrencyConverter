namespace CurrencyConverter.Domain.Entities
{
    public class ExchangeRate
    {
        public string BaseCurrency { get; set; } = string.Empty;
        public DateTime Date { get; set; }
        public Dictionary<string, decimal> Rates { get; set; } = new();
    }
}
