using Application.Dtos;

namespace Application.Abstractions;

public interface IApplicationService
{
    public Task<StudentApplicationDto> CreateApplicationAsync(CreateApplicationDto request);
    public Task<StudentApplicationDto> UpdateApplicationAsync(UpdateApplicationDto request);
    public Task<StudentApplicationDto> DeleteApplicationAsync(Guid applicationId);
    public Task<IEnumerable<StudentApplicationDto>> GetAllApplicationsAsync();
    public Task<StudentApplicationDto> GetByIdAsync(Guid applicationId);
    public Task<IEnumerable<StudentApplicationDto>> GetStudentApplicationsAsync(Guid studentId);
    public Task ApproveApplicationAsync(Guid applicationId);
    public Task DeclineApplicationAsync(Guid applicationId);
}