using Microsoft.EntityFrameworkCore;
using TravelManagement.API.Infrastructure.Persistence.Entities;
using TravelManagement.API.Infrastructure.Persistence.Entities.Flights;
namespace TravelManagement.API.Infrastructure.Persistence;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    public DbSet<User> Users => Set<User>();

    public DbSet<Role> Roles => Set<Role>();

    public DbSet<City> Cities => Set<City>();

    public DbSet<Contact> Contacts => Set<Contact>();

    // NEW
    public DbSet<Project> Projects => Set<Project>();

    // NEW
    public DbSet<BusinessEntity> BusinessEntities => Set<BusinessEntity>();

    public DbSet<Trip> Trips => Set<Trip>();

    public DbSet<Meeting> Meetings => Set<Meeting>();

    public DbSet<Hotel> Hotels => Set<Hotel>();

    public DbSet<TripMember> TripMembers => Set<TripMember>();

    public DbSet<MeetingAttendee> MeetingAttendees => Set<MeetingAttendee>();

    public DbSet<MeetingMaterial> MeetingMaterials => Set<MeetingMaterial>();

    public DbSet<Flight> Flights => Set<Flight>();

    public DbSet<TeamPlanEntry> TeamPlanEntries => Set<TeamPlanEntry>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.ApplyConfigurationsFromAssembly(typeof(ApplicationDbContext).Assembly);
    }
}