using System;
using FeatureFlagApi.Data;
using FeatureFlagApi.Dtos;
using FeatureFlagApi.Models;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Conventions;

namespace FeatureFlagApi.Services;

public interface IFeatureFlagService
{
    public Task<List<FeatureFlagDto>> GetAllFeatureFlagsAsync();

    public Task<FeatureFlagDto> GetFeatureFlagByIdAsync(int id);

    public Task<FeatureFlagDto> AddFeatureFlagAsync(FeatureFlagRequestDto request);

    public Task<FeatureFlagDto> UpdateFeatureFlagAsync(int id, FeatureFlagRequestDto request);

    public Task<bool> DeleteFeatureFlagAsync(int id);

    public Task<bool> ToggleFeatureFlagAsync(int id, bool isEnabled);

    public Task<List<FeatureFlagOverrideDto>> GetFeatureFlagOverridesAsync();

    public Task<FeatureFlagOverrideDto> AddFeatureFlagOverrideForUserAsync(
        int featureFlagId,
        string userId,
        bool isEnabled
    );

    public Task<bool> RemoveFeatureFlagOverrideForUserAsync(int featureFlagId, string userId);
    public Task<bool> ToggleFeatureFlagOverrideForUserAsync(
        int featureFlagId,
        string userId,
        bool isEnabled
    );

    public Task<bool> EvaluateFeatureFlagForUserAsync(
        string key,
        string userId,
        string? environment = null
    );

    public Task<Dictionary<string, bool>> EvaluateAllFeatureFlagsForUserAsync(
        string userId,
        string? environment = null
    );
}

public class FeatureFlagService(FeatureFlagDbContext dbContext) : IFeatureFlagService
{
    private readonly string defaultEnvironment = "development";

    public async Task<List<FeatureFlagDto>> GetAllFeatureFlagsAsync()
    {
        var featureFlags = await dbContext.FeatureFlags.ToListAsync();
        return featureFlags
            .Select(ff => new FeatureFlagDto
            {
                Id = ff.Id,
                Key = ff.Key,
                IsEnabled = ff.IsEnabled,
                Environment = ff.Environment,
            })
            .ToList();
    }

    public async Task<FeatureFlagDto> GetFeatureFlagByIdAsync(int id)
    {
        var featureFlag = await dbContext.FeatureFlags.FindAsync(id);
        if (featureFlag is null)
            return null!;

        return new FeatureFlagDto
        {
            Id = featureFlag.Id,
            Key = featureFlag.Key,
            IsEnabled = featureFlag.IsEnabled,
            Environment = featureFlag.Environment,
        };
    }

    public async Task<FeatureFlagDto> AddFeatureFlagAsync(FeatureFlagRequestDto request)
    {
        Console.WriteLine(
            $"Received request to add feature flag with Key: {request.Key}, Environment: {request.Environment}"
        );
        var FeatureFlag = new FeatureFlag
        {
            Key = request.Key,
            IsEnabled = request.IsEnabled,
            Environment = request.Environment,
            CreatedAt = DateTime.UtcNow,
        };

        Console.WriteLine(
            $"Adding feature flag: {FeatureFlag.Key} in environment: {FeatureFlag.Environment}"
        );

        var result = await dbContext.FeatureFlags.AddAsync(FeatureFlag);

        if (result is null)
            return null!;

        Console.WriteLine(
            $"Feature flag added with ID: {result.Entity.Id}, Key: {result.Entity.Key}, Environment: {result.Entity.Environment}"
        );

        await dbContext.SaveChangesAsync();
        return new FeatureFlagDto
        {
            Id = result.Entity.Id,
            Key = result.Entity.Key,
            IsEnabled = result.Entity.IsEnabled,
            Environment = result.Entity.Environment,
        };
    }

    //UpdateFeatureFlagAsync
    public async Task<FeatureFlagDto> UpdateFeatureFlagAsync(int id, FeatureFlagRequestDto request)
    {
        var existingFlag = await dbContext.FeatureFlags.FindAsync(id);
        if (existingFlag is null)
            return null!;

        existingFlag.Key = request.Key;
        existingFlag.IsEnabled = request.IsEnabled;
        existingFlag.Environment = request.Environment;
        dbContext.FeatureFlags.Update(existingFlag);
        await dbContext.SaveChangesAsync();
        return new FeatureFlagDto
        {
            Id = existingFlag.Id,
            Key = existingFlag.Key,
            IsEnabled = existingFlag.IsEnabled,
            Environment = existingFlag.Environment,
        };
    }

    public async Task<bool> DeleteFeatureFlagAsync(int id)
    {
        var existingFlag = await dbContext.FeatureFlags.FindAsync(id);
        if (existingFlag is null)
        {
            return false;
        }

        dbContext.FeatureFlags.Remove(existingFlag);
        await dbContext.SaveChangesAsync();
        return true;
    }

    public async Task<bool> ToggleFeatureFlagAsync(int id, bool isEnabled)
    {
        var existingFlag = await dbContext.FeatureFlags.FindAsync(id);
        if (existingFlag is null)
        {
            throw new KeyNotFoundException("Feature flag not found.");
        }

        existingFlag.IsEnabled = isEnabled;
        await dbContext.SaveChangesAsync();
        return existingFlag.IsEnabled;
    }

    public async Task<List<FeatureFlagOverrideDto>> GetFeatureFlagOverridesAsync()
    {
        return await dbContext
            .FeatureFlagOverrides.Select(o => new FeatureFlagOverrideDto
            {
                Id = o.Id,
                FeatureFlagId = o.FeatureFlagId,
                UserId = o.UserId,
                IsEnabled = o.IsEnabled,
            })
            .ToListAsync();
    }

    public async Task<FeatureFlagOverrideDto> AddFeatureFlagOverrideForUserAsync(
        int featureFlagId,
        string userId,
        bool isEnabled
    )
    {
        var existingOverride = await dbContext.FeatureFlagOverrides.FirstOrDefaultAsync(o =>
            o.FeatureFlagId == featureFlagId && o.UserId == userId
        );

        if (existingOverride is not null)
        {
            existingOverride.IsEnabled = isEnabled;
            dbContext.FeatureFlagOverrides.Update(existingOverride);
            await dbContext.SaveChangesAsync();
            return new FeatureFlagOverrideDto
            {
                Id = existingOverride.Id,
                FeatureFlagId = existingOverride.FeatureFlagId,
                UserId = existingOverride.UserId,
                IsEnabled = existingOverride.IsEnabled,
            };
        }

        var newOverride = new FeatureFlagOverride
        {
            FeatureFlagId = featureFlagId,
            UserId = userId,
            IsEnabled = isEnabled,
        };

        var result = await dbContext.FeatureFlagOverrides.AddAsync(newOverride);
        await dbContext.SaveChangesAsync();
        return new FeatureFlagOverrideDto
        {
            Id = result.Entity.Id,
            FeatureFlagId = result.Entity.FeatureFlagId,
            UserId = result.Entity.UserId,
            IsEnabled = result.Entity.IsEnabled,
        };
    }

    public async Task<bool> RemoveFeatureFlagOverrideForUserAsync(int featureFlagId, string userId)
    {
        var existingOverride = await dbContext.FeatureFlagOverrides.FirstOrDefaultAsync(o =>
            o.FeatureFlagId == featureFlagId && o.UserId == userId
        );

        if (existingOverride is null)
        {
            return false;
        }

        dbContext.FeatureFlagOverrides.Remove(existingOverride);
        await dbContext.SaveChangesAsync();
        return true;
    }

    public async Task<bool> ToggleFeatureFlagOverrideForUserAsync(
        int featureFlagId,
        string userId,
        bool isEnabled
    )
    {
        var existingOverride = await dbContext.FeatureFlagOverrides.FirstOrDefaultAsync(o =>
            o.FeatureFlagId == featureFlagId && o.UserId == userId
        );

        if (existingOverride is null)
        {
            throw new KeyNotFoundException("Feature flag override not found for user.");
        }

        existingOverride.IsEnabled = isEnabled;
        dbContext.FeatureFlagOverrides.Update(existingOverride);
        await dbContext.SaveChangesAsync();
        return existingOverride.IsEnabled;
    }

    public async Task<bool> EvaluateFeatureFlagForUserAsync(
        string key,
        string userId,
        string? environment = null
    )
    {
        var featureFlag = await dbContext.FeatureFlags.FirstOrDefaultAsync(f =>
            f.Key == key
            && (
                environment == null
                    ? f.Environment == defaultEnvironment
                    : f.Environment == environment
            )
        );

        if (featureFlag is null)
        {
            throw new KeyNotFoundException("Feature flag not found.");
        }

        var overrideEntry = await dbContext.FeatureFlagOverrides.FirstOrDefaultAsync(o =>
            o.FeatureFlagId == featureFlag.Id && o.UserId == userId
        );

        return overrideEntry?.IsEnabled ?? featureFlag.IsEnabled;
    }

    public async Task<Dictionary<string, bool>> EvaluateAllFeatureFlagsForUserAsync(
        string userId,
        string? environment = null
    )
    {
        var featureFlags = await dbContext
            .FeatureFlags.Where(f =>
                environment == null
                    ? f.Environment == defaultEnvironment
                    : f.Environment == environment
            )
            .ToListAsync();

        var overrides = await dbContext
            .FeatureFlagOverrides.Where(o => o.UserId == userId)
            .ToListAsync();

        var result = new Dictionary<string, bool>();
        foreach (var flag in featureFlags)
        {
            var overrideEntry = overrides.FirstOrDefault(o => o.FeatureFlagId == flag.Id);
            result[flag.Key] = overrideEntry?.IsEnabled ?? flag.IsEnabled;
        }

        return result;
    }
}
