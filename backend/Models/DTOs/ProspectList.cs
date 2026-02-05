using System.Text.Json.Serialization;

namespace backend.Models.DTOs;

public class ProspectListDto
{
    public string Source { get; set; }
    public List<ProspectDto> List { get; set; }
}

public class ProspectDto
{
    [JsonPropertyName("player_name")]
    public string PlayerName { get; set; }
    
    public string Team { get; set; }
    
    public string Position { get; set; }
    
    public int Age { get; set; }
    
    public string ETA { get; set; }
    
    public int Rank { get; set; }
}