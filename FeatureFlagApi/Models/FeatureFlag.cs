namespace FeatureFlagApi.Models;

public class FeatureFlag()
{
    public int Id { get; set; }
    public string Key { get; set; } = null!; // "NewDashboard"
    public bool IsEnabled { get; set; } // Global enable/disable for the feature flag
    public string Environment { get; set; } = "development"; // development | test | production
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}
