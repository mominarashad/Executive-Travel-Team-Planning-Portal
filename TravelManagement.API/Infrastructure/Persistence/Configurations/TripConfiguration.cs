using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TravelManagement.API.Infrastructure.Persistence.Entities;

namespace TravelManagement.API.Infrastructure.Persistence.Configurations;

public class TripConfiguration : IEntityTypeConfiguration<Trip>
{
    public void Configure(EntityTypeBuilder<Trip> builder)
    {
        builder.ToTable("Trips", t =>
{
    t.HasCheckConstraint("CK_Trips_DateOrder", "\"EndDate\" >= \"StartDate\"");
});

        builder.HasOne(t => t.DestinationCity)
            .WithMany(c => c.Trips)
            .HasForeignKey(t => t.DestinationCityId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(t => t.Project)
            .WithMany(p => p.Trips)
            .HasForeignKey(t => t.ProjectId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(t => t.BusinessEntity)
            .WithMany(e => e.Trips)
            .HasForeignKey(t => t.BusinessEntityId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.Property(x => x.Status)
            .HasMaxLength(50);

        builder.Property(x => x.Hotel)
            .HasMaxLength(150);

        builder.Property(x => x.Transport)
            .HasMaxLength(100);

        builder.Property(x => x.FlightInfo)
            .HasMaxLength(500);

        builder.Property(x => x.Notes)
            .HasMaxLength(1000);

        builder.Property(x => x.IsActive)
            .HasDefaultValue(true);

        builder.HasMany(t => t.Meetings)
            .WithOne(m => m.Trip)
            .HasForeignKey(m => m.TripId);

        builder.HasMany(t => t.TripMembers)
            .WithOne(tm => tm.Trip)
            .HasForeignKey(tm => tm.TripId);
    }
}