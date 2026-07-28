using System.Net;
using System.Net.Http.Json;
using FluentAssertions;

namespace TravelManagement.API.Tests;

public class DataManagementRollbackTests : IClassFixture<CustomWebApplicationFactory>
{
    private readonly CustomWebApplicationFactory _factory;

    public DataManagementRollbackTests(CustomWebApplicationFactory factory)
    {
        _factory = factory;
    }

    [Fact]
    public async Task CorruptedImport_RollsBackWithoutLosingExistingData()
    {
        _ = _factory.CreateClient(); // ensure app/seeding has started

        var client = await TestAuthHelper.GetAuthenticatedClientAsync(
            _factory, "admin@travelmanagement.com", "Admin@123");
        var beforeTrips = await client.GetFromJsonAsync<List<object>>("/api/trips");
        var beforeCount = beforeTrips!.Count;

        var corruptedPayload = new
        {
            exportVersion = "1.0",
            exportedAt = DateTime.UtcNow,
            roles = Array.Empty<object>(),
            users = Array.Empty<object>(),
            cities = Array.Empty<object>(),
            contacts = Array.Empty<object>(),
            projects = Array.Empty<object>(),
            businessEntities = Array.Empty<object>(),
            hotels = Array.Empty<object>(),
            trips = new[]
            {
                new
                {
                    id = Guid.NewGuid(),
                    destinationCityId = Guid.NewGuid(), // references a city that won't exist
                    startDate = "2027-01-01",
                    endDate = "2027-01-02",
                    projectId = (Guid?)null,
                    businessEntityId = (Guid?)null,
                    status = "Confirmed",
                    hotel = "",
                    transport = "",
                    flightInfo = "",
                    notes = "Corrupt",
                    isActive = true
                }
            },
            tripMembers = Array.Empty<object>(),
            meetings = Array.Empty<object>(),
            meetingAttendees = Array.Empty<object>(),
            meetingMaterials = Array.Empty<object>(),
            flights = Array.Empty<object>(),
            teamPlanEntries = Array.Empty<object>()
        };

        var importResponse = await client.PostAsJsonAsync("/api/data/import", corruptedPayload);
        importResponse.StatusCode.Should().Be(HttpStatusCode.BadRequest);

        var afterTrips = await client.GetFromJsonAsync<List<object>>("/api/trips");
        afterTrips!.Count.Should().Be(beforeCount);
    }
}