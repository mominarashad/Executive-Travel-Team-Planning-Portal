using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TravelManagement.API.Infrastructure.Persistence.Entities;

namespace TravelManagement.API.Infrastructure.Persistence.Configurations;

public class MeetingMaterialConfiguration : IEntityTypeConfiguration<MeetingMaterial>
{
    public void Configure(EntityTypeBuilder<MeetingMaterial> builder)
    {
        builder.ToTable("MeetingMaterials");

        builder.Property(m => m.Description)
            .IsRequired()
            .HasMaxLength(300);

        builder.HasOne(m => m.Meeting)
            .WithMany(meeting => meeting.Materials)
            .HasForeignKey(m => m.MeetingId);

        builder.HasOne(m => m.Owner)
            .WithMany(u => u.OwnedMaterials)
            .HasForeignKey(m => m.OwnerId)
            .OnDelete(DeleteBehavior.SetNull);
    }
}