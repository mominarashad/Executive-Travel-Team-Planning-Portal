using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.AspNetCore.Authorization;
using System.Text;
using TravelManagement.API.Infrastructure.Identity;
using TravelManagement.API.Infrastructure.Persistence;
using TravelManagement.API.Infrastructure.Seed;
using TravelManagement.API.Features.Auth.Interfaces;
using TravelManagement.API.Features.Auth.Services;
using Microsoft.OpenApi.Models;
using TravelManagement.API.Features.Auth.Repositories;
using TravelManagement.API.Common.Extensions;
using TravelManagement.API.Infrastructure.Validation;
using TravelManagement.API.Features.Users.Interfaces;
using TravelManagement.API.Features.Users.Repositories;

using TravelManagement.API.Features.Users.Services;
using TravelManagement.API.Features.Directory.Interfaces;
using TravelManagement.API.Features.Directory.Repositories;
using TravelManagement.API.Features.Directory.Services;

using TravelManagement.API.Features.Trips.Interfaces;
using TravelManagement.API.Features.Trips.Repositories;
using TravelManagement.API.Features.Trips.Services;

using TravelManagement.API.Features.Meetings.Interfaces;
using TravelManagement.API.Features.Meetings.Repositories;
using TravelManagement.API.Features.Meetings.Services;

using TravelManagement.API.Features.Flights.Interfaces;
using TravelManagement.API.Features.Flights.Repositories;
using TravelManagement.API.Features.Flights.Services;

using TravelManagement.API.Features.TeamPlans.Interfaces;
using TravelManagement.API.Features.TeamPlans.Repositories;
using TravelManagement.API.Features.TeamPlans.Services;

using TravelManagement.API.Features.Hotels.Interfaces;
using TravelManagement.API.Features.Hotels.Repositories;
using TravelManagement.API.Features.Hotels.Services;

using TravelManagement.API.Features.Projects.Interfaces;
using TravelManagement.API.Features.Projects.Repositories;
using TravelManagement.API.Features.Projects.Services;

using TravelManagement.API.Features.Entities.Interfaces;
using TravelManagement.API.Features.Entities.Repositories;
using TravelManagement.API.Features.Entities.Services;

using TravelManagement.API.Features.Calendar.Interfaces;
using TravelManagement.API.Features.Calendar.Repositories;
using TravelManagement.API.Features.Calendar.Services;

using TravelManagement.API.Features.Dashboard.Interfaces;
using TravelManagement.API.Features.Dashboard.Repositories;
using TravelManagement.API.Features.Dashboard.Services;

using TravelManagement.API.Features.OnePager.Interfaces;
using TravelManagement.API.Features.OnePager.Repositories;
using TravelManagement.API.Features.OnePager.Services;

using TravelManagement.API.Features.DataManagement.Interfaces;
using TravelManagement.API.Features.DataManagement.Repositories;
using TravelManagement.API.Features.DataManagement.Services;

using TravelManagement.API.Features.Email;
using TravelManagement.API.Features.Email.Interfaces;
using TravelManagement.API.Features.Email.Services;

AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);
var builder = WebApplication.CreateBuilder(args);
builder.Services.AddCors(options =>
{
    options.AddPolicy("FrontendPolicy", policy =>
    {
        policy
            .WithOrigins("http://localhost:3000")
            .AllowAnyHeader()
            .AllowAnyMethod();
    });
});

// Add services
builder.Services
    .AddControllers()
    .ConfigureApiBehaviorOptions(options =>
    {
        options.InvalidModelStateResponseFactory =
            ValidationResponseFactory.Create;
    });
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new()
    {
        Title = "Travel Management API",
        Version = "v1"
    });

    options.AddSecurityDefinition("Bearer", new Microsoft.OpenApi.Models.OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = Microsoft.OpenApi.Models.SecuritySchemeType.Http,
        Scheme = "bearer",
        BearerFormat = "JWT",
        In = Microsoft.OpenApi.Models.ParameterLocation.Header,
        Description = "Enter: Bearer {your JWT token}"
    });

    options.AddSecurityRequirement(new Microsoft.OpenApi.Models.OpenApiSecurityRequirement
    {
        {
            new Microsoft.OpenApi.Models.OpenApiSecurityScheme
            {
                Reference = new Microsoft.OpenApi.Models.OpenApiReference
                {
                    Type = Microsoft.OpenApi.Models.ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            Array.Empty<string>()
        }
    });
});
// Database
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseNpgsql(
        builder.Configuration.GetConnectionString("DefaultConnection")));

// JWT Settings
builder.Services.Configure<JwtSettings>(
    builder.Configuration.GetSection(JwtSettings.SectionName));

builder.Services.Configure<EmailSettings>(
    builder.Configuration.GetSection(EmailSettings.SectionName));
builder.Services.AddScoped<IEmailService, EmailService>();

var jwtSettings = builder.Configuration
    .GetSection(JwtSettings.SectionName)
    .Get<JwtSettings>();

if (jwtSettings is null)
{
    throw new InvalidOperationException("JWT settings are missing.");
}

// Authentication
builder.Services
    .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,

            ValidIssuer = jwtSettings.Issuer,
            ValidAudience = jwtSettings.Audience,

            IssuerSigningKey = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(jwtSettings.SecretKey))
        };
    });

builder.Services.AddAuthorization(options =>
{
    options.FallbackPolicy = new AuthorizationPolicyBuilder()
        .RequireAuthenticatedUser()
        .Build();
});
builder.Services.AddScoped<IJwtTokenGenerator, JwtTokenGenerator>();
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IAuthRepository, AuthRepository>();
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<ICityRepository, CityRepository>();
builder.Services.AddScoped<ICityService, CityService>();
builder.Services.AddScoped<IContactRepository, ContactRepository>();
builder.Services.AddScoped<IContactService, ContactService>();
builder.Services.AddScoped<ITripRepository, TripRepository>();
builder.Services.AddScoped<ITripService, TripService>();
builder.Services.AddScoped<IMeetingRepository, MeetingRepository>();
builder.Services.AddScoped<IMeetingService, MeetingService>();
builder.Services.AddScoped<IFlightRepository, FlightRepository>();

builder.Services.AddScoped<IFlightService, FlightService>();

builder.Services.AddScoped<ITeamPlanRepository, TeamPlanRepository>();
builder.Services.AddScoped<ITeamPlanService, TeamPlanService>();

builder.Services.AddScoped<IHotelRepository, HotelRepository>();
builder.Services.AddScoped<IHotelService, HotelService>();

builder.Services.AddScoped<IProjectRepository, ProjectRepository>();
builder.Services.AddScoped<IProjectService, ProjectService>();
builder.Services.AddScoped<IBusinessEntityRepository, BusinessEntityRepository>();
builder.Services.AddScoped<IBusinessEntityService, BusinessEntityService>();

builder.Services.AddScoped<ICalendarRepository, CalendarRepository>();
builder.Services.AddScoped<ICalendarService, CalendarService>();

builder.Services.AddScoped<IDashboardRepository, DashboardRepository>();
builder.Services.AddScoped<IDashboardService, DashboardService>();

builder.Services.AddScoped<IOnePagerRepository, OnePagerRepository>();
builder.Services.AddScoped<IOnePagerService, OnePagerService>();

builder.Services.AddScoped<IDataManagementRepository, DataManagementRepository>();
builder.Services.AddScoped<IDataManagementService, DataManagementService>();



var app = builder.Build();

//
// Seed Database
//
using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

    if (context.Database.IsRelational())
    {
        await context.Database.MigrateAsync();
    }
    else
    {
        await context.Database.EnsureCreatedAsync();
    }

    await DatabaseSeeder.SeedAsync(context);
}
// Configure HTTP request pipeline
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseGlobalExceptionMiddleware();

app.UseCors("FrontendPolicy");
app.UseAuthentication();

app.UseAuthorization();

app.MapControllers();


app.Run();

public partial class Program { }
