using System;

namespace FeatureFlagApi.Dtos;

public record FeatureFlagToggleRequestDto
{
    public bool IsEnabled { get; init; }
}
