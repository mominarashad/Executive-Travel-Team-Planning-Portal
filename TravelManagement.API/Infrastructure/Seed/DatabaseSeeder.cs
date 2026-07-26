using Microsoft.EntityFrameworkCore;
using TravelManagement.API.Infrastructure.Persistence;

namespace TravelManagement.API.Infrastructure.Seed;

public static class DatabaseSeeder
{
    public static async Task SeedAsync(ApplicationDbContext context)
    {
        await context.Database.MigrateAsync();

        await RoleSeeder.SeedAsync(context);
        await UserSeeder.SeedAsync(context);
        await DirectorySeeder.SeedAsync(context);
        await ProjectSeeder.SeedAsync(context);
        await BusinessEntitySeeder.SeedAsync(context);
        await TripSeeder.SeedAsync(context);
        await TripMemberSeeder.SeedAsync(context);
        await FlightSeeder.SeedAsync(context);
        await TeamPlanSeeder.SeedAsync(context);
        await MeetingSeeder.SeedAsync(context);
        await MeetingAttendeeSeeder.SeedAsync(context);
        await MeetingMaterialSeeder.SeedAsync(context);
    }
}