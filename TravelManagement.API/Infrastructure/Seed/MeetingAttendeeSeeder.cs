using Microsoft.EntityFrameworkCore;
using TravelManagement.API.Infrastructure.Persistence;
using TravelManagement.API.Infrastructure.Persistence.Entities;

namespace TravelManagement.API.Infrastructure.Seed;

public static class MeetingAttendeeSeeder
{
    public static async Task SeedAsync(ApplicationDbContext context)
    {
        if (await context.MeetingAttendees.AnyAsync())
            return;

        var ceo = await context.Users.FirstAsync(u => u.Name == "Alex Morgan");
        var cfo = await context.Users.FirstAsync(u => u.Name == "Sarah Ahmed");
        var cto = await context.Users.FirstAsync(u => u.Name == "John Williams");
        var assistant = await context.Users.FirstAsync(u => u.Name == "Maria Garcia");

        var meetings = await context.Meetings
            .Include(m => m.Project)
            .OrderBy(m => m.DisplayOrder)
            .ToListAsync();

        foreach (var meeting in meetings)
        {
            switch (meeting.Project?.Name)
            {
                case "AI Expansion":

                    context.MeetingAttendees.AddRange(

                        new MeetingAttendee
                        {
                            Id = Guid.NewGuid(),
                            MeetingId = meeting.Id,
                            UserId = ceo.Id
                        },

                        new MeetingAttendee
                        {
                            Id = Guid.NewGuid(),
                            MeetingId = meeting.Id,
                            UserId = cto.Id
                        }

                    );

                    break;

                case "Client Visit":

                    context.MeetingAttendees.AddRange(

                        new MeetingAttendee
                        {
                            Id = Guid.NewGuid(),
                            MeetingId = meeting.Id,
                            UserId = ceo.Id
                        },

                        new MeetingAttendee
                        {
                            Id = Guid.NewGuid(),
                            MeetingId = meeting.Id,
                            UserId = assistant.Id
                        }

                    );

                    break;

                case "Recruitment":

                    context.MeetingAttendees.AddRange(

                        new MeetingAttendee
                        {
                            Id = Guid.NewGuid(),
                            MeetingId = meeting.Id,
                            UserId = assistant.Id
                        }

                    );

                    break;

                case "Partnership":

                    context.MeetingAttendees.AddRange(

                        new MeetingAttendee
                        {
                            Id = Guid.NewGuid(),
                            MeetingId = meeting.Id,
                            UserId = ceo.Id
                        },

                        new MeetingAttendee
                        {
                            Id = Guid.NewGuid(),
                            MeetingId = meeting.Id,
                            UserId = cfo.Id
                        },

                        new MeetingAttendee
                        {
                            Id = Guid.NewGuid(),
                            MeetingId = meeting.Id,
                            UserId = cto.Id
                        }

                    );

                    break;
            }
        }

        await context.SaveChangesAsync();
    }
}