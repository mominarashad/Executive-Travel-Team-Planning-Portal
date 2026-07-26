using BCrypt.Net;
using Microsoft.EntityFrameworkCore;
using TravelManagement.API.Infrastructure.Persistence;
using TravelManagement.API.Infrastructure.Persistence.Entities;

namespace TravelManagement.API.Infrastructure.Seed;

public static class UserSeeder
{
    public static async Task SeedAsync(ApplicationDbContext context)
    {
        if (await context.Users.AnyAsync())
            return;

        var adminRole = await context.Roles.FirstAsync(r => r.Name == "Admin");
var employeeRole = await context.Roles.FirstAsync(r => r.Name == "Employee");

context.Users.AddRange(

    new User
    {
        Id = Guid.NewGuid(),
        Name = "System Administrator",
        Email = "admin@travelmanagement.com",
        PasswordHash = BCrypt.Net.BCrypt.HashPassword("Admin@123"),
        Title = "Administrator",
        Function = "IT",
        IsCeo = false,
        RoleId = adminRole.Id
    },

    new User
    {
        Id = Guid.NewGuid(),
        Name = "Alex Morgan",
        Email = "alex@travelmanagement.com",
        PasswordHash = BCrypt.Net.BCrypt.HashPassword("Password123"),
        Title = "Chief Executive Officer",
        Function = "Executive",
        IsCeo = true,
        RoleId = employeeRole.Id
    },

    new User
    {
        Id = Guid.NewGuid(),
        Name = "Sarah Ahmed",
        Email = "sarah@travelmanagement.com",
        PasswordHash = BCrypt.Net.BCrypt.HashPassword("Password123"),
        Title = "Chief Financial Officer",
        Function = "Finance",
        IsCeo = false,
        RoleId = employeeRole.Id
    },

    new User
    {
        Id = Guid.NewGuid(),
        Name = "John Williams",
        Email = "john@travelmanagement.com",
        PasswordHash = BCrypt.Net.BCrypt.HashPassword("Password123"),
        Title = "Chief Technology Officer",
        Function = "Technology",
        IsCeo = false,
        RoleId = employeeRole.Id
    },

    new User
    {
        Id = Guid.NewGuid(),
        Name = "Maria Garcia",
        Email = "maria@travelmanagement.com",
        PasswordHash = BCrypt.Net.BCrypt.HashPassword("Password123"),
        Title = "Executive Assistant",
        Function = "Executive Office",
        IsCeo = false,
        RoleId = employeeRole.Id
    },

    new User
    {
        Id = Guid.NewGuid(),
        Name = "David Chen",
        Email = "david@travelmanagement.com",
        PasswordHash = BCrypt.Net.BCrypt.HashPassword("Password123"),
        Title = "Finance Manager",
        Function = "Finance",
        IsCeo = false,
        RoleId = employeeRole.Id
    }
);

await context.SaveChangesAsync();
}}