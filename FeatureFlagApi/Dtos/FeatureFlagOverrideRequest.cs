using System;

namespace FeatureFlagApi.Dtos;

public record FeatureFlagOverrideRequest
{
    public bool IsEnabled { get; init; }
}
