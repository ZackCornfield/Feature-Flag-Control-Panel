using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using FeatureFlagApi.Data;
using FeatureFlagApi.Dtos;
using FeatureFlagApi.Helpers;
using FeatureFlagApi.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

namespace FeatureFlagApi.Services;

public interface IAuthService
{
    public Task<UserDto?> RegisterAsync(UserRequestDto userDto);
    public Task<UserDto?> LoginAsync(UserRequestDto userDto);
    public Task<string?> GenerateJwtTokenAsync(User user);
    public Task<UserDto?> GetUserByIdAsync(Guid userId);
}

public class AuthService(FeatureFlagDbContext dbContext, JwtSettings jwtSettings) : IAuthService
{
    public async Task<UserDto?> RegisterAsync(UserRequestDto userDto)
    {
        if (userDto == null)
        {
            throw new ArgumentException("User data cannot be null.");
        }

        var existingUser = await dbContext
            .Users.Where(u => u.Email == userDto.Email)
            .FirstOrDefaultAsync();

        // If a user with the same Email already exists, do not register a new one.
        if (existingUser != null)
        {
            return null;
        }

        // Create a new user from the request DTO.
        var newUser = new User { Email = userDto.Email, CreatedAt = DateTime.UtcNow };

        newUser.Password = new PasswordHasher<User>().HashPassword(newUser, userDto.Password);

        dbContext.Users.Add(newUser);
        await dbContext.SaveChangesAsync();

        // Generate JWT Token for the newly registered user.
        var token = await GenerateJwtTokenAsync(newUser);

        // Map the created user entity to a DTO to return.
        var result = new UserDto
        {
            Id = newUser.Id,
            Email = newUser.Email,
            Token = token,
        };

        return result;
    }

    public async Task<UserDto?> LoginAsync(UserRequestDto userDto)
    {
        if (userDto is null)
        {
            throw new ArgumentException("User data cannot be null.");
        }

        var existingUser = await dbContext
            .Users.Where(u => u.Email == userDto.Email)
            .FirstOrDefaultAsync();

        if (existingUser is null)
        {
            throw new ArgumentException($"User with Email {userDto.Email} does not exist.");
        }

        if (
            new PasswordHasher<User>().VerifyHashedPassword(
                existingUser,
                existingUser.Password,
                userDto.Password
            ) == PasswordVerificationResult.Failed
        )
        {
            throw new ArgumentException("Invalid password.");
        }

        var token = await GenerateJwtTokenAsync(existingUser);

        return new UserDto
        {
            Id = existingUser.Id,
            Email = existingUser.Email,
            CreatedAt = existingUser.CreatedAt,
            Token = token,
        };
    }

    public async Task<string?> GenerateJwtTokenAsync(User user)
    {
        var key = new SymmetricSecurityKey(
            System.Text.Encoding.UTF8.GetBytes(jwtSettings.SecretKey)
        );
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var claims = new List<Claim>
        {
            new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
            new Claim(JwtRegisteredClaimNames.UniqueName, user.Email),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
        };

        var token = new JwtSecurityToken(
            issuer: jwtSettings.Issuer,
            audience: jwtSettings.Audience,
            claims: claims,
            expires: DateTime.UtcNow.AddHours(2),
            signingCredentials: creds
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }

    public async Task<UserDto?> GetUserByIdAsync(Guid userId)
    {
        var existingUser = await dbContext.Users.FindAsync(userId);
        if (existingUser is null)
        {
            throw new ArgumentException($"User with ID {userId} doesn't exist.");
        }

        return new UserDto
        {
            Id = existingUser.Id,
            Email = existingUser.Email,
            CreatedAt = existingUser.CreatedAt,
        };
    }
}
