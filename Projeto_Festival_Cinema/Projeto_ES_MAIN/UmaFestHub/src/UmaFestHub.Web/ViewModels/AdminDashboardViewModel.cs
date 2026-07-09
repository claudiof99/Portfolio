namespace UmaFestHub.Web.ViewModels;

public sealed class AdminDashboardViewModel
{
	public int TotalFestivals { get; set; }
	public int TotalFilms { get; set; }
	public int ActiveSessions { get; set; }
	public int TotalPurchases { get; set; }
	public string ServiceStatus { get; set; } = "Unknown";
}
