namespace UmaFestHub.Application.Observers.FestivalEnding;

/// <summary>Observer callback for “festival end is within the reminder window” for a specific user.</summary>
public interface IFestivalEndingNotificationObserver
{
	/// <param name="context">Festival + user targeted by the scheduled reminder pass.</param>
	Task OnFestivalEndingSoonAsync(FestivalEndingSoonContext context, CancellationToken cancellationToken = default);
}
