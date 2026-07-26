using Microsoft.EntityFrameworkCore;
using TravelManagement.API.Infrastructure.Persistence;
using TravelManagement.API.Infrastructure.Persistence.Entities;

namespace TravelManagement.API.Infrastructure.Seed;

public static class MeetingMaterialSeeder
{
    public static async Task SeedAsync(ApplicationDbContext context)
    {
        if (await context.MeetingMaterials.AnyAsync())
            return;

        var ceo = await context.Users.FirstAsync(u => u.Name == "Alex Morgan");
        var cfo = await context.Users.FirstAsync(u => u.Name == "Sarah Ahmed");
        var cto = await context.Users.FirstAsync(u => u.Name == "John Williams");
        var assistant = await context.Users.FirstAsync(u => u.Name == "Maria Garcia");

        var meetings = await context.Meetings
            .Include(m => m.Project)
            .ToListAsync();

        foreach (var meeting in meetings)
        {
            switch (meeting.Project?.Name)
            {
                case "AI Expansion":

                    context.MeetingMaterials.AddRange(

                        new MeetingMaterial
                        {
                            Id = Guid.NewGuid(),
                            MeetingId = meeting.Id,
                            Description = "AI Strategy Deck",
                            OwnerId = cto.Id
                        },

                        new MeetingMaterial
                        {
                            Id = Guid.NewGuid(),
                            MeetingId = meeting.Id,
                            Description = "Architecture Overview",
                            OwnerId = cto.Id
                        }

                    );

                    break;

                case "Client Visit":

                    context.MeetingMaterials.AddRange(

                        new MeetingMaterial
                        {
                            Id = Guid.NewGuid(),
                            MeetingId = meeting.Id,
                            Description = "Customer Presentation",
                            OwnerId = assistant.Id
                        },

                        new MeetingMaterial
                        {
                            Id = Guid.NewGuid(),
                            MeetingId = meeting.Id,
                            Description = "Meeting Agenda",
                            OwnerId = ceo.Id
                        }

                    );

                    break;

                case "Recruitment":

                    context.MeetingMaterials.AddRange(

                        new MeetingMaterial
                        {
                            Id = Guid.NewGuid(),
                            MeetingId = meeting.Id,
                            Description = "Candidate Profiles",
                            OwnerId = assistant.Id
                        },

                        new MeetingMaterial
                        {
                            Id = Guid.NewGuid(),
                            MeetingId = meeting.Id,
                            Description = "Interview Schedule",
                            OwnerId = assistant.Id
                        }

                    );

                    break;

                case "Partnership":

                    context.MeetingMaterials.AddRange(

                        new MeetingMaterial
                        {
                            Id = Guid.NewGuid(),
                            MeetingId = meeting.Id,
                            Description = "Financial Report",
                            OwnerId = cfo.Id
                        },

                        new MeetingMaterial
                        {
                            Id = Guid.NewGuid(),
                            MeetingId = meeting.Id,
                            Description = "Partnership Proposal",
                            OwnerId = ceo.Id
                        },

                        new MeetingMaterial
                        {
                            Id = Guid.NewGuid(),
                            MeetingId = meeting.Id,
                            Description = "Technical Architecture",
                            OwnerId = cto.Id
                        }

                    );

                    break;
            }
        }

        await context.SaveChangesAsync();
    }
}