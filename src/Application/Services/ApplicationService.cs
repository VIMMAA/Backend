using Domain.Abstractions;
using Domain.Repositories;
using Microsoft.Extensions.Logging;

namespace Application.Services;

public class ApplicationService : IService
{
    private readonly IApplicationRepository _repository;
    private readonly ILogger<ApplicationService> _logger;

    public async Task ApproveApplicationAsync(Guid applicationId)
    {

    }
}