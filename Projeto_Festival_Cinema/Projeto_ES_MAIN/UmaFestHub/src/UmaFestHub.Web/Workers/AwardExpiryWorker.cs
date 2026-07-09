using Microsoft.Extensions.Options;
using UmaFestHub.Application.Interfaces;
using UmaFestHub.Web.Options;

namespace UmaFestHub.Web.Workers;

/// <summary>
/// Periodically deactivates awards whose <see cref="Domain.Entities.Award.EndDateUtc"/> has passed.
/// </summary>
public sealed class AwardExpiryWorker : BackgroundService
{
	private readonly IServiceScopeFactory _scopeFactory;
	private readonly IOptions<AwardExpiryOptions> _options;
	private readonly ILogger<AwardExpiryWorker> _logger;

	public AwardExpiryWorker(
		IServiceScopeFactory scopeFactory,
		IOptions<AwardExpiryOptions> options,
		ILogger<AwardExpiryWorker> logger)
	{
		_scopeFactory = scopeFactory;
		_options = options;
		_logger = logger;
	}

	protected override async Task ExecuteAsync(CancellationToken stoppingToken)
	{
		var opts = _options.Value;
		var initialDelay = TimeSpan.FromSeconds(Math.Clamp(opts.InitialDelaySeconds, 0, 86400));
		var interval = TimeSpan.FromMinutes(Math.Clamp(opts.IntervalMinutes, 1, 1440));

		if (initialDelay > TimeSpan.Zero)
		{
			await Task.Delay(initialDelay, stoppingToken);
		}

		while (!stoppingToken.IsCancellationRequested)
		{
			try
			{
				using var scope = _scopeFactory.CreateScope();
				var awardService = scope.ServiceProvider.GetRequiredService<IAwardService>();
				await awardService.ExpireDueAwardsAsync(stoppingToken);
			}
			catch (OperationCanceledException) when (stoppingToken.IsCancellationRequested)
			{
				break;
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "Award expiry pass failed.");
			}

			try
			{
				await Task.Delay(interval, stoppingToken);
			}
			catch (OperationCanceledException) when (stoppingToken.IsCancellationRequested)
			{
				break;
			}
		}
	}
}
