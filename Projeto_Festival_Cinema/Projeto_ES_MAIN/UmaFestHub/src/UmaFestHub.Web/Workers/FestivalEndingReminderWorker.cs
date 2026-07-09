using Microsoft.Extensions.Options;
using UmaFestHub.Application.Interfaces;
using UmaFestHub.Web.Options;

namespace UmaFestHub.Web.Workers;

/// <summary>
/// Host-side <see cref="BackgroundService"/>: periodic trigger for “festival ends within 3 days” notifications.
/// Does not contain business rules—delegates to scoped <see cref="IFestivalEndingReminderService"/>.
/// </summary>
public sealed class FestivalEndingReminderWorker : BackgroundService
{
	private readonly IServiceScopeFactory _scopeFactory;
	private readonly IOptions<FestivalEndingReminderOptions> _options;
	private readonly ILogger<FestivalEndingReminderWorker> _logger;

	public FestivalEndingReminderWorker(
		IServiceScopeFactory scopeFactory,
		IOptions<FestivalEndingReminderOptions> options,
		ILogger<FestivalEndingReminderWorker> logger)
	{
		_scopeFactory = scopeFactory;
		_options = options;
		_logger = logger;
	}

	protected override async Task ExecuteAsync(CancellationToken stoppingToken)
	{
		var opts = _options.Value;
		// Defensive bounds so bad config cannot stall the host or fire sub-hour spam.
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
				// Scoped services (DbContext, reminder service) must not outlive the pass.
				using var scope = _scopeFactory.CreateScope();
				var reminder = scope.ServiceProvider.GetRequiredService<IFestivalEndingReminderService>();
				await reminder.EnqueueEndingSoonNotificationsAsync(stoppingToken);
			}
			catch (OperationCanceledException) when (stoppingToken.IsCancellationRequested)
			{
				break;
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "Festival ending reminder pass failed.");
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
