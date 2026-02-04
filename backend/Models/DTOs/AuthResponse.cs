namespace backend.Models.DTOs;

public class AuthResponse
{
    public required string Token { get; set; }
    public required string Email { get; set; }
    public string? FirstName { get; set; }
    public string? LastName { get; set; }
}
