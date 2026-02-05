using System.ComponentModel.DataAnnotations;

namespace FeatureFlagApi.Dtos;

public record class FeatureFlagDto
{
    public int Id { get; set; }
    public string Key { get; set; } = null!; // "NewDashboard"
    public bool IsEnabled { get; set; } // Global enable/disable for the feature flag

    [Required]
    [RegularExpression(
        "^(development|test|production)$",
        ErrorMessage = "Environment must be one of: development, test, production."
    )]
    public string Environment { get; set; } = "development"; // development | test | production
}
