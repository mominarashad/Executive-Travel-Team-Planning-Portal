using TravelManagement.API.Features.Meetings.DTOs;

namespace TravelManagement.API.Features.Meetings.Interfaces;

public interface IMeetingRepository
{
    Task<IEnumerable<MeetingDto>> GetAllAsync();

    Task<MeetingDto?> GetByIdAsync(Guid id);

    Task<MeetingDto> CreateAsync(CreateMeetingDto dto);

    Task<bool> UpdateAsync(Guid id, UpdateMeetingDto dto);

    Task<bool> DeleteAsync(Guid id);
}