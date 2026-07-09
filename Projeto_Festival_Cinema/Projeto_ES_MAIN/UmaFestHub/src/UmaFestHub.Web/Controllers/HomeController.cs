using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Globalization;
using UmaFestHub.Application.Helpers;
using UmaFestHub.Application.Interfaces;
using UmaFestHub.Application.Recommendations;
using UmaFestHub.Web.Mappings;
using UmaFestHub.Web.ViewModels;
using UmaFestHub.Domain.Entities;

namespace UmaFestHub.Web.Controllers;

/// <summary>Home page: festivals plus signed-in user carousels (watchlist, favorites, Seen / <see cref="PersonalListType.Watched"/>).</summary>
public class HomeController : Controller
{
	private readonly IFestivalService _festivalService;
	private readonly IPersonalListService _personalListService;
	private readonly IFilmService _filmService;
	private readonly IRecommendationService _recommendationService;
	private readonly ILogger<HomeController> _logger;

	public HomeController(
		IFestivalService festivalService,
		IPersonalListService personalListService,
		IFilmService filmService,
		IRecommendationService recommendationService,
		ILogger<HomeController> logger)
	{
		_festivalService = festivalService;
		_personalListService = personalListService;
		_filmService = filmService;
		_recommendationService = recommendationService;
		_logger = logger;
	}

	/// <summary>Builds the home view; loads <see cref="PersonalListType.Watched"/> film ids for the Seen carousel when the user is authenticated.</summary>
	[HttpGet("/", Name = "HomeIndex")]
	public async Task<IActionResult> Index(CancellationToken cancellationToken)
	{
		var festivals = await _festivalService.GetAllAsync(cancellationToken);
		var ordered = festivals.OrderBy(f => f.StartDateUtc).ToList();
		var now = DateTime.UtcNow;

		var featured = ordered
			.Where(f => f.StartDateUtc > now)
			.Take(5)
			.Select(f => f.ToViewModel())
			.ToList();

		var upcoming = ordered
			.Where(f => f.StartDateUtc > now)
			.Take(12)
			.Select(f => f.ToViewModel())
			.ToList();

		var nowStreaming = ordered
			.Where(f => f.StartDateUtc <= now && f.EndDateUtc >= now)
			.Take(12)
			.Select(f => f.ToViewModel())
			.ToList();

		var vm = new HomePageViewModel
		{
			FeaturedFestivals = featured,
			UpcomingFestivals = upcoming,
			NowStreamingFestivals = nowStreaming,
			IsAuthenticated = User?.Identity?.IsAuthenticated ?? false
		};

		if (vm.IsAuthenticated && User is not null && User.TryGetCurrentUserId(out var userId))
		{
			var favoriteIds = await _personalListService.GetListAsync(userId, PersonalListType.Favorites, cancellationToken);
			var watchlistIds = await _personalListService.GetListAsync(userId, PersonalListType.Watchlist, cancellationToken);
			var watchedIds = await _personalListService.GetListAsync(userId, PersonalListType.Watched, cancellationToken);

			var favorites = await _filmService.GetByIdsAsync(favoriteIds, cancellationToken);
			var watchlist = await _filmService.GetByIdsAsync(watchlistIds, cancellationToken);
			var watched = await _filmService.GetByIdsAsync(watchedIds, cancellationToken);

			var tmdbLanguage = TmdbLanguageMapper.ToTmdbLanguage(CultureInfo.CurrentUICulture);
			favorites = await _filmService.LocalizeFilmsAsync(favorites, tmdbLanguage, cancellationToken);
			watchlist = await _filmService.LocalizeFilmsAsync(watchlist, tmdbLanguage, cancellationToken);
			watched = await _filmService.LocalizeFilmsAsync(watched, tmdbLanguage, cancellationToken);

			// Get personalized recommendations from ALL festivals
			var recommendations = new List<RecommendationViewModel>();
			try
			{
				_logger.LogInformation("Fetching recommendations for user {UserId} from all festivals", userId);
				var recs = await _recommendationService.GetFromAllFestivalsAsync(userId, 6, cancellationToken);
				recommendations = recs.Select(r => r.ToViewModel()).ToList();
				_logger.LogInformation("Got {Count} recommendations from all festivals", recommendations.Count);
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "Failed to get recommendations for user {UserId}", userId);
			}

			vm = new HomePageViewModel
			{
				FeaturedFestivals = featured,
				UpcomingFestivals = upcoming,
				NowStreamingFestivals = nowStreaming,
				IsAuthenticated = vm.IsAuthenticated,
				FavoriteFilms = favorites.Select(f => f.ToViewModel()).ToList(),
				WatchlistFilms = watchlist.Select(f => f.ToViewModel()).ToList(),
				WatchedFilms = watched.Select(f => f.ToViewModel()).ToList(),
				RecommendedFilms = recommendations
			};
		}

		return View(vm);
	}

	[Route("/error", Name = "Error")]
	public IActionResult Error() => View();
}