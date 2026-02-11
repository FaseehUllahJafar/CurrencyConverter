namespace CurrencyConverter.Application.Exceptions;

public class UnsupportedCurrencyException : Exception
{
    public UnsupportedCurrencyException(string message) : base(message)
    {
    }
}
