public class HistoricalRequest
{
    public string BaseCurrency { get; set; } = default!;
    public DateTime From { get; set; }
    public DateTime To { get; set; }
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 10;
}
