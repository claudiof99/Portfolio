// -----------------------------------------------------------------------------
// Awards, nominations & votes — Awards index: paging, festival filter, festival dropdown.
// -----------------------------------------------------------------------------
namespace UmaFestHub.Web.ViewModels;

public sealed class AwardIndexPageViewModel
{
	public int Page { get; set; } = 1;
	public bool HasNext { get; set; }
	public Guid? FestivalId { get; set; }
	public IReadOnlyList<AwardViewModel> Awards { get; set; } = Array.Empty<AwardViewModel>();
	public IReadOnlyList<FestivalOptionViewModel> Festivals { get; set; } = Array.Empty<FestivalOptionViewModel>();
}

public sealed class FestivalOptionViewModel
{
	public Guid Id { get; set; }
	public string Name { get; set; } = string.Empty;
}

