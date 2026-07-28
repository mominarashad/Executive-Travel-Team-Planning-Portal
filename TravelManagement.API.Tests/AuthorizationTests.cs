using System.Net;
using System.Net.Http.Json;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using TravelManagement.API.Infrastructure.Persistence;

namespace TravelManagement.API.Tests;

public class AuthorizationTests : IClassFixture<CustomWebApplicationFactory>
{
    private readonly CustomWebApplicationFactory _factory;

    public AuthorizationTests(CustomWebApplicationFactory factory)
    {
        _factory = factory;
    }

    [Fact]
    public async Task UnauthenticatedRequest_Returns401()
    {
        var client = _factory.CreateClient();
        var response = await client.GetAsync("/api/trips");
        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task NonAdminUser_CreatingUser_Returns403()
    {
        

        var client = await TestAuthHelper.GetAuthenticatedClientAsync(
            _factory, "david@travelmanagement.com", "Password123");

        var response = await client.PostAsJsonAsync("/api/users", new
        {
            name = "Test User",
            email = $"test{Guid.NewGuid()}@example.com",
            password = "TestPass123!",
            title = "Tester",
            function = "QA",
            isCeo = false,
            roleId = Guid.NewGuid()
        });

        response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
    }

    [Fact]
    public async Task AdminUser_CreatingUser_Returns201()
    {
        

        var client = await TestAuthHelper.GetAuthenticatedClientAsync(
            _factory, "admin@travelmanagement.com", "Admin@123");

        Guid employeeRoleId;
        using (var scope = _factory.Services.CreateScope())
        {
            var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
            employeeRoleId = context.Roles.First(r => r.Name == "Employee").Id;
        }

        var response = await client.PostAsJsonAsync("/api/users", new
        {
            name = "Test User",
            email = $"test{Guid.NewGuid()}@example.com",
            password = "TestPass123!",
            title = "Tester",
            function = "QA",
            isCeo = false,
            roleId = employeeRoleId
        });

        response.StatusCode.Should().Be(HttpStatusCode.Created);
    }
}