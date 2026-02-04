using System;

namespace FeatureFlagApi.Models;

public class FeatureFlagOverride
{
    public int Id { get; set; }
    public int FeatureFlagId { get; set; }
    public Guid UserId { get; set; }
    public bool isEnabled { get; set; }
}
