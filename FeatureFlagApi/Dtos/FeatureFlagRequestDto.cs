namespace FeatureFlagApi.Dtos;

public record class FeatureFlagRequestDto
{
    public string Key { get; set; } = null!; // "NewDashboard"
    public bool IsEnabled { get; set; } // Global enable/disable for the feature flag
    public string Environment { get; set; } = "Dev"; // Dev | Test | Prod
}
