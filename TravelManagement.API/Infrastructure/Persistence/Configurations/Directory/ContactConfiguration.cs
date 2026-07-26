using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TravelManagement.API.Infrastructure.Persistence.Entities;

namespace TravelManagement.API.Infrastructure.Persistence.Configurations;

public class ContactConfiguration : IEntityTypeConfiguration<Contact>
{
    public void Configure(EntityTypeBuilder<Contact> builder)
    {
        builder.ToTable("Contacts");

        builder.Property(c => c.Name)
            .IsRequired()
            .HasMaxLength(100);
        builder.Property(c => c.IsActive)
    .HasDefaultValue(true);

        builder.Property(c => c.Organization)
            .HasMaxLength(150);

        builder.Property(c => c.Role)
            .HasMaxLength(100);

        builder.Property(c => c.Email)
            .HasMaxLength(150);

        builder.Property(c => c.Phone)
            .HasMaxLength(30);
    }
}