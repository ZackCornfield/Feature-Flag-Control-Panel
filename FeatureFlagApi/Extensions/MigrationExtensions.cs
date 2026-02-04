using System;
using Microsoft.EntityFrameworkCore;

namespace TaskFlowLiteApi.Extensions;

public static class MigrationExtensions
{
    public static async Task MigrateDatabaseAsync(this IApplicationBuilder app)
    {
        using var scope = app.ApplicationServices.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<FeatureFlagDbContext>();
        await dbContext.Database.MigrateAsync();
    }
}
