namespace UmaFestHub.Web.ViewModels;

public sealed class SessionViewModel
{
	public Guid Id { get; set; }
	public Guid FestivalFilmId { get; set; }
	public string SessionType { get; set; } = string.Empty;
	public DateTime StartTimeUtc { get; set; }
	public DateTime EndTimeUtc { get; set; }
	public decimal Price { get; set; }
	public Guid? TicketId { get; set; }
}
