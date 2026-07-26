using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TravelManagement.API.Infrastructure.Persistence.Entities.Flights;

namespace TravelManagement.API.Infrastructure.Persistence.Configurations;

public class FlightConfiguration : IEntityTypeConfiguration<Flight>
{
    public void Configure(EntityTypeBuilder<Flight> builder)
    {
        builder.ToTable("Flights");

        builder.Property(x => x.Airline)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(x => x.FlightNumber)
            .IsRequired()
            .HasMaxLength(50);

        builder.Property(x => x.DepartureAirport)
            .HasMaxLength(100);

        builder.Property(x => x.ArrivalAirport)
            .HasMaxLength(100);

        builder.Property(x => x.Aircraft)
            .HasMaxLength(100);

        builder.Property(x => x.BookingReference)
            .HasMaxLength(100);

        builder.Property(x => x.IsActive)
            .HasDefaultValue(true);

        builder.HasOne(x => x.Trip)
            .WithMany(t => t.Flights)
            .HasForeignKey(x => x.TripId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(x => x.User)
            .WithMany(u => u.Flights)
            .HasForeignKey(x => x.UserId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}