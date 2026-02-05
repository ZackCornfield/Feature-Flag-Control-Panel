namespace FeatureFlagApi.Dtos;

public record class FeatureFlagOverrideDto
{
    public int Id { get; set; }
    public int FeatureFlagId { get; set; }
    public string UserId { get; set; } = null!; // Unique identifier for the user (e.g., email, username, or user ID)
    public bool IsEnabled { get; set; }
}
