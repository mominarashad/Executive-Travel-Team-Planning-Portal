namespace TravelManagement.API.Features.Directory.DTOs;

public class CreateContactDto
{
    public string Name { get; set; } = string.Empty;

    public string Organization { get; set; } = string.Empty;

    public string Role { get; set; } = string.Empty;

    public string Email { get; set; } = string.Empty;

    public string Phone { get; set; } = string.Empty;

    public int SortOrder { get; set; }

    public Guid CityId { get; set; }
}