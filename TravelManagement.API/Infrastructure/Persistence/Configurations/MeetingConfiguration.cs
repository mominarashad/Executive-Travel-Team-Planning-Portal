using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TravelManagement.API.Infrastructure.Persistence.Entities;

namespace TravelManagement.API.Infrastructure.Persistence.Configurations;

public class MeetingConfiguration : IEntityTypeConfiguration<Meeting>
{
    public void Configure(EntityTypeBuilder<Meeting> builder)
    {
        builder.ToTable("Meetings");

        builder.Property(x => x.Priority)
            .IsRequired()
            .HasMaxLength(20);

        builder.Property(x => x.Status)
            .IsRequired()
            .HasMaxLength(30);

        builder.Property(x => x.Agenda)
            .HasMaxLength(2000);

        builder.HasOne(x => x.Trip)
            .WithMany(x => x.Meetings)
            .HasForeignKey(x => x.TripId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(x => x.Contact)
            .WithMany(x => x.Meetings)
            .HasForeignKey(x => x.ContactId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(x => x.Project)
            .WithMany(p => p.Meetings)
            .HasForeignKey(x => x.ProjectId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(x => x.BusinessEntity)
            .WithMany(e => e.Meetings)
            .HasForeignKey(x => x.BusinessEntityId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasMany(m => m.MeetingAttendees)
            .WithOne(a => a.Meeting)
            .HasForeignKey(a => a.MeetingId);

        builder.HasMany(m => m.Materials)
            .WithOne(m => m.Meeting)
            .HasForeignKey(m => m.MeetingId);

        builder.HasIndex(m => new { m.TripId, m.DisplayOrder })
    .IsUnique()
    .HasFilter("\"IsActive\" = true");
    }
}