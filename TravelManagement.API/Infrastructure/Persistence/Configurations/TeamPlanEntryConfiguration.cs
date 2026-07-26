using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TravelManagement.API.Infrastructure.Persistence.Entities;

namespace TravelManagement.API.Infrastructure.Persistence.Configurations;

public class TeamPlanEntryConfiguration : IEntityTypeConfiguration<TeamPlanEntry>
{
    public void Configure(EntityTypeBuilder<TeamPlanEntry> builder)
    {
        builder.ToTable("TeamPlanEntries", t =>
        {
            t.HasCheckConstraint("CK_TeamPlanEntries_DateOrder", "\"ToDate\" >= \"FromDate\"");
            t.HasCheckConstraint("CK_TeamPlanEntries_ApprovalOnlyVacation",
                "\"ApprovalStatus\" = '' OR \"Type\" = 'Vacation'");
        });

        builder.Property(x => x.IsActive)
            .HasDefaultValue(true);
    }
}