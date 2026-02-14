namespace backend.Models;

public class Ranking
{
    public Guid Id { get; set; }
    public required int Rank { get; set; }
    public required string PlayerName { get; set; }
    public required string Team { get; set; }
    public required string Position { get; set; }
    public int Age { get; set; }
    public string? ETA { get; set; }
    public double Score { get; set; }
    public string Volatility { get; set; } = "";
    public int Consensus { get; set; }
    public double? Median { get; set; }
    public double? Sd { get; set; }
    public int Tier { get; set; } = 1;
    public int SourceCount { get; set; }
}
