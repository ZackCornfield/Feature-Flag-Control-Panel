using System;

namespace FeatureFlagApi.Dtos;

public record FeatureFlagToggleRequest
{
    public bool IsEnabled { get; init; }
}
