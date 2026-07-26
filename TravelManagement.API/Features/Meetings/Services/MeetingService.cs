using TravelManagement.API.Features.Meetings.DTOs;
using TravelManagement.API.Features.Meetings.Interfaces;

namespace TravelManagement.API.Features.Meetings.Services;

public class MeetingService : IMeetingService
{
    private readonly IMeetingRepository _repository;

    public MeetingService(IMeetingRepository repository)
    {
        _repository = repository;
    }

    public async Task<IEnumerable<MeetingDto>> GetAllAsync()
    {
        return await _repository.GetAllAsync();
    }

    public async Task<MeetingDto?> GetByIdAsync(Guid id)
    {
        return await _repository.GetByIdAsync(id);
    }

    public async Task<MeetingDto> CreateAsync(CreateMeetingDto dto)
    {
        return await _repository.CreateAsync(dto);
    }

    public async Task<bool> UpdateAsync(Guid id, UpdateMeetingDto dto)
    {
        return await _repository.UpdateAsync(id, dto);
    }

    public async Task<bool> DeleteAsync(Guid id)
    {
        return await _repository.DeleteAsync(id);
    }
}