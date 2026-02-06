namespace backend.Models;

public class Prospect
{
    public Guid Id { get; set; }
    public required string PlayerName { get; set; }
    public required string Team { get; set; }
    public required string Position { get; set; }
    public int Age { get; set; }
    public string? ETA { get; set; }
    public int Rank { get; set; }
    public string? Source { get; set; }
}
