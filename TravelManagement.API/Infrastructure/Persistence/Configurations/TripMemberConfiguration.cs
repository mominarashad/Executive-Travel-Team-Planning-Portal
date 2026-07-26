using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TravelManagement.API.Infrastructure.Persistence.Entities;

namespace TravelManagement.API.Infrastructure.Persistence.Configurations;

public class TripMemberConfiguration : IEntityTypeConfiguration<TripMember>
{
    public void Configure(EntityTypeBuilder<TripMember> builder)
    {
        builder.ToTable("TripMembers");

        builder.HasOne(tm => tm.Trip)
            .WithMany(t => t.TripMembers)
            .HasForeignKey(tm => tm.TripId);

        builder.HasOne(tm => tm.User)
            .WithMany(u => u.TripMembers)
            .HasForeignKey(tm => tm.UserId);

        builder.HasIndex(tm => new { tm.TripId, tm.UserId })
            .IsUnique();
    }
}