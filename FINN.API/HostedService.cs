using System.Text.Json;
using FINN.CORE.Interfaces;
using FINN.CORE.Models;
using FINN.SHAREDKERNEL.Constants;
using FINN.SHAREDKERNEL.Dtos.Management;

namespace FINN.API;

public class HostedService : BackgroundService
{
    private readonly IBroker _broker;
    private readonly ILogger<HostedService> _logger;
    private readonly IServiceProvider _serviceProvider;

    public HostedService(ILogger<HostedService> logger, IBroker broker, IServiceProvider serviceProvider)
    {
        _logger = logger;
        _broker = broker;
        _serviceProvider = serviceProvider;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _broker.RegisterHandler(RoutingKeys.UpdateJobStatus, (routingKey, correlationId, message) =>
            HandleUpdateJobStatus(message));

        while (!stoppingToken.IsCancellationRequested)
        {
            _logger.LogTrace(
                "{Service} is running at {Time}... Next check is in {Duration}s", nameof(HostedService),
                DateTime.Now.ToLocalTime(), 30);
            await Task.Delay(30000, stoppingToken);
        }

        _logger.LogInformation("Connection closed");
    }

    #region Handlers

    private async void HandleUpdateJobStatus(string message)
    {
        var status = JsonSerializer.Deserialize<UpdateJobStatusRequestDto>(message);
        _logger.LogInformation("Try Updated {JobId} status: {JobStatus}", status.Id, status.Status);

        using var scope = _serviceProvider.CreateScope();
        var repository = scope.ServiceProvider.GetService<IRepository<Job>>();
        var job = await repository.GetByIdAsync(status.Id);

        if (job != null)
        {
            job.Status = status.Status;
            job.Output = status.Output;
            await repository.UpdateAsync(job);
            await repository.SaveChangesAsync();
            _logger.LogInformation("Updated {JobId} status: {JobStatus}", job.Id, job.Status);
        }
    }

    #endregion
}