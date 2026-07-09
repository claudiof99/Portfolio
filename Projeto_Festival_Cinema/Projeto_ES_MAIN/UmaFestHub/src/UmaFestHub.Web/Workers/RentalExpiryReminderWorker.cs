using Microsoft.Extensions.Options;
using UmaFestHub.Application.Interfaces;
using UmaFestHub.Web.Options;

namespace UmaFestHub.Web.Workers;

/// <summary>Periodic host pass for rental access expiring within the reminder window (delegates to <see cref="IRentalExpiryReminderService"/>).</summary>
public sealed class RentalExpiryReminderWorker : BackgroundService
{
	private readonly IServiceScopeFactory _scopeFactory;
	private readonly IOptions<RentalExpiryReminderOptions> _options;
	private readonly ILogger<RentalExpiryReminderWorker> _logger;

	public RentalExpiryReminderWorker(
		IServiceScopeFactory scopeFactory,
		IOptions<RentalExpiryReminderOptions> options,
		ILogger<RentalExpiryReminderWorker> logger)
	{
		_scopeFactory = scopeFactory;
		_options = options;
		_logger = logger;
	}

	protected override async Task ExecuteAsync(CancellationToken stoppingToken)
	{
		var opts = _options.Value;
		// Defensive bounds (same pattern as FestivalEndingReminderWorker).
		var initialDelay = TimeSpan.FromSeconds(Math.Clamp(opts.InitialDelaySeconds, 0, 86400));
		var interval = TimeSpan.FromHours(Math.Clamp(opts.IntervalHours, 1, 168));

		if (initialDelay > TimeSpan.Zero)
		{
			await Task.Delay(initialDelay, stoppingToken);
		}

		while (!stoppingToken.IsCancellationRequested)
		{
			try
			{
				// Scoped services (DbContext, reminder) must not outlive the pass.
				using var scope = _scopeFactory.CreateScope();
				var reminder = scope.ServiceProvider.GetRequiredService<IRentalExpiryReminderService>();
				await reminder.EnqueueRentalExpiryRemindersAsync(stoppingToken);
			}
			catch (OperationCanceledException) when (stoppingToken.IsCancellationRequested)
			{
				break;
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "Rental expiry reminder pass failed.");
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
