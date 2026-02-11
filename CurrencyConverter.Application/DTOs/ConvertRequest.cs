namespace CurrencyConverter.Application.DTOs;

public class ConvertRequest
{
    public string From { get; set; } = default!;
    public string To { get; set; } = default!;
    public decimal Amount { get; set; }
}
