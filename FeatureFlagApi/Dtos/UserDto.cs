namespace FeatureFlagApi.Dtos;

public record class UserDto
{
    public Guid Id { get; set; }
    public string Email { get; set; } = null!;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public string? Token { get; set; }
}
