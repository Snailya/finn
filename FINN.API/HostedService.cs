﻿using System.Text.Json;
using FINN.API.Contexts;
using FINN.SHAREDKERNEL;
using FINN.SHAREDKERNEL.Dtos;
using FINN.SHAREDKERNEL.Dtos.UpdateJobStatus;
using FINN.SHAREDKERNEL.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace FINN.API;

public class HostedService : BackgroundService
{
    private readonly IBroker _broker;
    private readonly IDbContextFactory<JobContext> _factory;
    private readonly ILogger<HostedService> _logger;

    public HostedService(ILogger<HostedService> logger, IBroker broker, IDbContextFactory<JobContext> factory)
    {
        _logger = logger;
        _broker = broker;
        _factory = factory;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _broker.RegisterHandler(RoutingKey.UpdateJobStatus, (routingKey, correlationId, message) =>
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

    private void HandleUpdateJobStatus(string message)
    {
        var status = JsonSerializer.Deserialize<UpdateJobStatusDto>(message);
        _logger.LogInformation("Try Updated {JobId} status: {JobStatus}", status.Id, status.Status);

        using var context = _factory.CreateDbContext();
        var job = context.Jobs.Find(status.Id);
        job.Status = status.Status;
        job.Output = status.Output;
        context.Update(job);
        context.SaveChanges();

        _logger.LogInformation("Updated {JobId} status: {JobStatus}", job.Id, job.Status);
    }

    #endregion
}