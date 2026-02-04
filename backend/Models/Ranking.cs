namespace backend.Models;

public class Ranking
{
<<<<<<< HEAD
    public Guid Id { get; set; }
=======
    public int Id { get; set; }
>>>>>>> 610df4e (fixes deployment script)
    public required int Rank { get; set; }
    public required string PlayerName { get; set; }
    public required string Team { get; set; }
    public required string Position { get; set; }
    public int Age { get; set; }
    public string? ETA { get; set; }
    public int Score { get; set; }
    public string Volatility { get; set; } = "";
    public int Consensus { get; set; }
<<<<<<< HEAD
}
=======
}
>>>>>>> 610df4e (fixes deployment script)
