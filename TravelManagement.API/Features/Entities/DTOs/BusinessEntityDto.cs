namespace TravelManagement.API.Features.Entities.DTOs;

public class BusinessEntityDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public bool IsSystem { get; set; }
}