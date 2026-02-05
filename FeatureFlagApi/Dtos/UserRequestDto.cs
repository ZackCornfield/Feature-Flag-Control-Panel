using System;

namespace FeatureFlagApi.Dtos;

public class UserRequestDto
{
    public Guid Id { get; set; }
    public string Email { get; set; } = null!;
    public string Password { get; set; } = null!;
}
