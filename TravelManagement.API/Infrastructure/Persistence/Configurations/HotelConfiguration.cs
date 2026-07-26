using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TravelManagement.API.Infrastructure.Persistence.Entities;

namespace TravelManagement.API.Infrastructure.Persistence.Configurations;

public class HotelConfiguration : IEntityTypeConfiguration<Hotel>
{
    public void Configure(EntityTypeBuilder<Hotel> builder)
    {
        builder.Property(h => h.Name)
            .IsRequired()
            .HasMaxLength(200);

        builder.HasOne(h => h.City)
            .WithMany()
            .HasForeignKey(h => h.CityId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}