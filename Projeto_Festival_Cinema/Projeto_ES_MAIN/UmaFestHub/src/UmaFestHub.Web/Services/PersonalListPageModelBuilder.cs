using Microsoft.Extensions.Localization;
using System.Globalization;
using UmaFestHub.Application.DTOs;
using UmaFestHub.Application.Helpers;
using UmaFestHub.Application.Interfaces;
using UmaFestHub.Domain.Entities;
using UmaFestHub.Domain.Interfaces;
using UmaFestHub.Web.Mappings;
using UmaFestHub.Web.Resources;
using UmaFestHub.Web.ViewModels;

namespace UmaFestHub.Web.Services;

/// <summary>
/// Assembles a <see cref="PersonalListPageViewModel"/> for the personal-list hub and for the Seen-only watch-history page.
/// When <paramref name="type"/> is <see cref="PersonalListType.Watched"/>, the result powers the UI label “Seen”
/// (<c>/PersonalList?type=Watched</c> and <c>/watchHistory</c>).
/// </summary>
public static class PersonalListPageModelBuilder
{
	/// <summary>
	/// Loads list film ids, resolves films, applies festival lineup + title/genre filters, and sets page titles
	/// (including “Seen” for <see cref="PersonalListType.Watched"/>).
	/// </summary>
	/// <param name="userId">Current user.</param>
	/// <param name="type">Watchlist, Favorites, or Watched (Seen).</param>
	/// <param name="title">Optional title filter.</param>
	/// <param name="genre">Optional genre filter.</param>
	/// <param name="festivalId">Optional festival filter when valid for this user’s list.</param>
	/// <param name="personalListService">Source of list membership ids.</param>
	/// <param name="filmService">Resolves catalog films for ids.</param>
	/// <param name="purchaseRepository">Used to narrow festival filter options by purchased festivals.</param>
	/// <param name="festivalFilmRepository">Festival lineup overlap for filter dropdown and scoping.</param>
	/// <param name="localizer">Localized page titles and subtitles.</param>
	/// <param name="cancellationToken">Cancellation token.</param>
	public static async Task<PersonalListPageViewModel> BuildAsync(
		Guid userId,
		PersonalListType type,
		string? title,
		string? genre,
		Guid? festivalId,
		IPersonalListService personalListService,
		IFilmService filmService,
		IPurchaseRepository purchaseRepository,
		IFestivalFilmRepository festivalFilmRepository,
		IStringLocalizer<SharedResources> localizer,
		CancellationToken cancellationToken = default)
	{
		var filmIds = await personalListService.GetListAsync(userId, type, cancellationToken);
		var sourceCount = filmIds.Count;

		var purchasedFestIds = await purchaseRepository.GetDistinctFestivalIdsFromUserPurchasesAsync(userId, cancellationToken);
		IReadOnlyList<PersonalListFestivalFilterOption> festivalOptions = [];

		IReadOnlyList<FilmDto> filmDtosOrdered = [];

		if (filmIds.Count > 0)
		{
			var orderRank = filmIds.Select((id, index) => (id, index)).ToDictionary(x => x.id, x => x.index);
			var unordered = await filmService.GetByIdsAsync(filmIds, cancellationToken);
			filmDtosOrdered = unordered
				.OrderBy(f => orderRank.GetValueOrDefault(f.Id, int.MaxValue))
				.ToList();

			var lineupFestivals =
				await festivalFilmRepository.GetDistinctFestivalsContainingFilmIdsAsync(filmIds, cancellationToken);
			festivalOptions = (purchasedFestIds.Count > 0
					? lineupFestivals.Where(x => purchasedFestIds.Contains(x.FestivalId))
					: lineupFestivals)
				.Select(x => new PersonalListFestivalFilterOption(x.FestivalId, x.FestivalName))
				.ToList();

			var allowedFestivalIds = festivalOptions.Select(f => f.Id).ToHashSet();
			if (festivalId.HasValue && festivalId.Value != Guid.Empty &&
			    allowedFestivalIds.Contains(festivalId.Value))
			{
				var inProgram = await festivalFilmRepository.GetFilmIdsInFestivalProgramAsync(festivalId.Value, cancellationToken);
				filmDtosOrdered = filmDtosOrdered.Where(f => inProgram.Contains(f.Id)).ToList();
			}
			else
			{
				festivalId = null;
			}

			filmDtosOrdered = ApplyFilters(filmDtosOrdered, title, genre).ToList();
		}
		else
		{
			festivalId = null;
		}

		if (filmDtosOrdered.Count > 0)
		{
			var tmdbLanguage = TmdbLanguageMapper.ToTmdbLanguage(CultureInfo.CurrentUICulture);
			filmDtosOrdered = await filmService.LocalizeFilmsAsync(filmDtosOrdered, tmdbLanguage, cancellationToken);
		}

		var (pageTitle, subtitle) = type switch
		{
			PersonalListType.Favorites =>
				(localizer["PersonalList_TitleFavorites"].Value, localizer["PersonalList_SubtitleFavorites"].Value),
			PersonalListType.Watchlist =>
				(localizer["PersonalList_TitleWatchlist"].Value, localizer["PersonalList_SubtitleWatchlist"].Value),
			PersonalListType.Watched =>
				(localizer["PersonalList_TitleSeen"].Value, localizer["PersonalList_SubtitleSeen"].Value),
			_ => (localizer["PersonalList_TitleWatchlist"].Value, localizer["PersonalList_SubtitleWatchlist"].Value)
		};

		return new PersonalListPageViewModel
		{
			ListType = type,
			PageTitle = pageTitle,
			PageSubtitle = subtitle,
			SourceListFilmCount = sourceCount,
			FilterTitle = title,
			FilterGenre = genre,
			SelectedFestivalId = festivalId,
			FestivalFilterOptions = festivalOptions,
			Films = filmDtosOrdered.Select(f => f.ToViewModel()).ToList()
		};
	}

	/// <summary>In-memory filters aligned with the personal list film search behaviour (title + genre).</summary>
	private static IEnumerable<FilmDto> ApplyFilters(
		IEnumerable<FilmDto> films,
		string? title,
		string? genre)
	{
		var query = films.AsEnumerable();
		if (!string.IsNullOrWhiteSpace(title))
		{
			query = query.Where(x =>
				x.Name.Contains(title, StringComparison.OrdinalIgnoreCase) ||
				x.Genres.Any(g => g.Contains(title, StringComparison.OrdinalIgnoreCase)));
		}

		if (!string.IsNullOrWhiteSpace(genre))
		{
			query = query.Where(x =>
				x.Genres.Any(g => g.Contains(genre, StringComparison.OrdinalIgnoreCase)));
		}

		return query;
	}
}
