using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TravelManagement.API.Infrastructure.Persistence.Entities;

namespace TravelManagement.API.Infrastructure.Persistence.Configurations;

public class MeetingAttendeeConfiguration : IEntityTypeConfiguration<MeetingAttendee>
{
    public void Configure(EntityTypeBuilder<MeetingAttendee> builder)
    {
        builder.ToTable("MeetingAttendees");

        builder.HasOne(ma => ma.Meeting)
            .WithMany(m => m.MeetingAttendees)
            .HasForeignKey(ma => ma.MeetingId);

        builder.HasOne(ma => ma.User)
            .WithMany(u => u.MeetingAttendees)
            .HasForeignKey(ma => ma.UserId);

        builder.HasIndex(ma => new { ma.MeetingId, ma.UserId })
            .IsUnique();
    }
}