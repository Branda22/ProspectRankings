using System.Text.Json.Serialization;

namespace backend.Models;

public class Source
{
    public int Id { get; set; }
    public required string Name { get; set; }

    [JsonIgnore]
    public ICollection<Prospect> Prospects { get; set; } = [];
}
