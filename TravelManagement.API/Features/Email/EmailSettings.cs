namespace TravelManagement.API.Features.Email;

public class EmailSettings
{
    public const string SectionName = "Email";

    public string SmtpHost { get; set; } = "mailpit";
    public int SmtpPort { get; set; } = 1025;
    public string FromAddress { get; set; } = "noreply@travelmanagement.local";
    public string FromName { get; set; } = "Travel Management";
}