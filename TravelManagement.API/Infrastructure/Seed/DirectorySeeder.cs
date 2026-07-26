using Microsoft.EntityFrameworkCore;
using TravelManagement.API.Infrastructure.Persistence;
using TravelManagement.API.Infrastructure.Persistence.Entities;

namespace TravelManagement.API.Infrastructure.Seed;

public static class DirectorySeeder
{
    public static async Task SeedAsync(ApplicationDbContext context)
    {
        // Seed Cities
        if (!await context.Cities.AnyAsync())
        {
            context.Cities.AddRange(

                new City
                {
                    Id = Guid.NewGuid(),
                    Name = "Lahore",
                    IsActive = true
                },

                new City
                {
                    Id = Guid.NewGuid(),
                    Name = "Karachi",
                    IsActive = true
                },

                new City
                {
                    Id = Guid.NewGuid(),
                    Name = "Dubai",
                    IsActive = true
                }

            );

            await context.SaveChangesAsync();
        }

        // Seed Contacts
        if (!await context.Contacts.AnyAsync())
        {
            var lahore = await context.Cities.FirstAsync(c => c.Name == "Lahore");
            var karachi = await context.Cities.FirstAsync(c => c.Name == "Karachi");
            var dubai = await context.Cities.FirstAsync(c => c.Name == "Dubai");

            context.Contacts.AddRange(

                new Contact
                {
                    Id = Guid.NewGuid(),
                    Name = "Ahmed Khan",
                    Organization = "NETSOL",
                    Role = "CEO",
                    Email = "ahmed@netsol.com",
                    Phone = "+92-300-1111111",
                    SortOrder = 1,
                    IsActive = true,
                    CityId = lahore.Id
                },

                new Contact
                {
                    Id = Guid.NewGuid(),
                    Name = "Sara Ali",
                    Organization = "Arbisoft",
                    Role = "HR Manager",
                    Email = "sara@arbisoft.com",
                    Phone = "+92-300-2222222",
                    SortOrder = 2,
                    IsActive = true,
                    CityId = lahore.Id
                },

                new Contact
                {
                    Id = Guid.NewGuid(),
                    Name = "Ali Raza",
                    Organization = "Systems Ltd",
                    Role = "CTO",
                    Email = "ali@systems.com",
                    Phone = "+92-300-3333333",
                    SortOrder = 1,
                    IsActive = true,
                    CityId = karachi.Id
                },

                new Contact
                {
                    Id = Guid.NewGuid(),
                    Name = "John Smith",
                    Organization = "Travel Partner",
                    Role = "Director",
                    Email = "john@travelpartner.com",
                    Phone = "+971-50-1234567",
                    SortOrder = 1,
                    IsActive = true,
                    CityId = dubai.Id
                }

            );

            await context.SaveChangesAsync();
        }
    }
}