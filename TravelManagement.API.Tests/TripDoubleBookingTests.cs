using System.Net;
using System.Net.Http.Json;
using FluentAssertions;

namespace TravelManagement.API.Tests;

public class TripDoubleBookingTests : IClassFixture<CustomWebApplicationFactory>
{
    private readonly CustomWebApplicationFactory _factory;

    public TripDoubleBookingTests(CustomWebApplicationFactory factory)
    {
        _factory = factory;
    }

    private async Task<(Guid cityId, Guid userId)> GetTestFixturesAsync(HttpClient client)
    {
        var cities = await client.GetFromJsonAsync<List<CityResult>>("/api/directory/cities");
        var users = await client.GetFromJsonAsync<List<UserResult>>("/api/users");
        return (cities!.First().id, users!.First().id);
    }

    [Fact]
    public async Task ConfirmedTripOverlap_IsRejected()
    {
        _ = _factory.CreateClient(); // ensure app/seeding has started

        var client = await TestAuthHelper.GetAuthenticatedClientAsync(
            _factory, "admin@travelmanagement.com", "Admin@123");
        var (cityId, userId) = await GetTestFixturesAsync(client);
        var tripA = new
        {
            destinationCityId = cityId,
            startDate = "2027-09-10",
            endDate = "2027-09-15",
            projectId = (Guid?)null,
            businessEntityId = (Guid?)null,
            status = "Confirmed",
            hotel = "",
            transport = "",
            notes = "Trip A",
            teamMemberIds = new[] { userId }
        };

        var createA = await client.PostAsJsonAsync("/api/trips", tripA);
        createA.StatusCode.Should().Be(HttpStatusCode.Created);

        var tripB = tripA with { };
        var createB = await client.PostAsJsonAsync("/api/trips", new
        {
            destinationCityId = cityId,
            startDate = "2027-09-12",
            endDate = "2027-09-18",
            projectId = (Guid?)null,
            businessEntityId = (Guid?)null,
            status = "Confirmed",
            hotel = "",
            transport = "",
            notes = "Trip B - should conflict",
            teamMemberIds = new[] { userId }
        });

        createB.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task TentativeTripOverlap_IsAllowed()
    {
        _ = _factory.CreateClient(); // ensure app/seeding has started

        var client = await TestAuthHelper.GetAuthenticatedClientAsync(
            _factory, "admin@travelmanagement.com", "Admin@123");
        var (cityId, userId) = await GetTestFixturesAsync(client);
        var tripA = await client.PostAsJsonAsync("/api/trips", new
        {
            destinationCityId = cityId,
            startDate = "2027-10-01",
            endDate = "2027-10-05",
            projectId = (Guid?)null,
            businessEntityId = (Guid?)null,
            status = "Option",
            hotel = "",
            transport = "",
            notes = "Tentative 1",
            teamMemberIds = new[] { userId }
        });
        tripA.StatusCode.Should().Be(HttpStatusCode.Created);

        var tripB = await client.PostAsJsonAsync("/api/trips", new
        {
            destinationCityId = cityId,
            startDate = "2027-10-03",
            endDate = "2027-10-08",
            projectId = (Guid?)null,
            businessEntityId = (Guid?)null,
            status = "Option",
            hotel = "",
            transport = "",
            notes = "Tentative 2 - overlapping, should succeed",
            teamMemberIds = new[] { userId }
        });
        tripB.StatusCode.Should().Be(HttpStatusCode.Created);
    }

    private record CityResult(Guid id);
    private record UserResult(Guid id);
}