// -----------------------------------------------------------------------------
// Awards, nominations & votes — AwardDto.ToViewModel maps nominees to AwardNomineeRowViewModel
// (this file also maps unrelated DTOs to view models).
// Review replies: ReviewReplyDto.ToViewModel -> ReviewReplyViewModel.
// -----------------------------------------------------------------------------
using System.Data.Common;
using System.Net.Http.Headers;
using UmaFestHub.Application.DTOs;
using UmaFestHub.Application.Recommendations;
using UmaFestHub.Web.ViewModels;

namespace UmaFestHub.Web.Mappings;

public static class ViewModelMappings
{
	public static FilmViewModel ToViewModel(this FilmDto dto) => new()
	{
		Id = dto.Id,
		ExternalId = dto.ExternalId,
		Name = dto.Name,
		Url = dto.Url,
		ImageUrl = dto.ImageUrl,
		Description = dto.Description,
		DurationMinutes = dto.DurationMinutes,
		Genres = dto.Genres
	};

	public static FestivalViewModel ToViewModel(this FestivalDto dto) => new()
	{
		Id = dto.Id,
		Name = dto.Name,
		Description = dto.Description,
		StartDateUtc = dto.StartDateUtc,
		EndDateUtc = dto.EndDateUtc,
		IsHidden = dto.IsHidden,
		EarlyBirdDiscountPercent = dto.EarlyBirdDiscountPercent,
		EarlyBirdDaysBeforeStart = dto.EarlyBirdDaysBeforeStart
	};

	public static CartViewModel ToViewModel(this CartDto dto) => new()
	{
		Id = dto.Id,
		UserId = dto.UserId,
		Items = dto.Items.Select(i => new CartItemViewModel
		{
			Id = i.Id,
			ProductId = i.ProductId,
			ProductType = i.ProductType,
			Quantity = i.Quantity,
			Price = i.Price
		}).ToList()
	};

	public static CartItemViewModel ToViewModel(this CartItemDto dto) => new()
	{
		Id = dto.Id,
		ProductId = dto.ProductId,
		ProductType = dto.ProductType,
		Quantity = dto.Quantity,
		Price = dto.Price
	};

	public static PurchaseViewModel ToViewModel(this PurchaseDto dto) => new()
	{
		Id = dto.Id,
		UserId = dto.UserId,
		DateUtc = dto.DateUtc,
		TotalAmount = dto.TotalAmount,
		Status = dto.Status,
		ActiveRentalRemaining = dto.ActiveRentalRemaining,
		Items = dto.Items.Select(i => new PurchaseItemViewModel
		{
			ProductId = i.ProductId,
			Quantity = i.Quantity,
			PriceAtPurchase = i.PriceAtPurchase
		}).ToList()
	};

	public static SessionViewModel ToViewModel(this SessionDto dto) => new()
	{
		Id = dto.Id,
		FestivalFilmId = dto.FestivalFilmId,
		SessionType = dto.SessionType,
		StartTimeUtc = dto.StartTimeUtc,
		EndTimeUtc = dto.EndTimeUtc
	};

	// Single review mapper for both public and management DTOs.
	// Management-only fields are filled when the DTO supports IManagedReviewLikeDto.
	public static ReviewViewModel ToViewModel(this IReviewLikeDto dto)
	{
		var vm = new ReviewViewModel
		{
			Id = dto.Id,
			UserId = dto.UserId,
			FestivalFilmId = dto.FestivalFilmId,
			FilmId = dto.FilmId,
			ExternalFilmId = dto.ExternalFilmId,
			Rating = dto.Rating,
			Comment = dto.Comment,
			Status = dto.Status,
			DateUtc = dto.DateUtc,
			IsReported = dto.IsReported,
			HasBeenReported = dto.HasBeenReported
		};

		if (dto is IManagedReviewLikeDto managed)
		{
			vm.AuthorName = managed.AuthorName;
			vm.IsStaffAuthor = managed.IsStaffAuthor;
			vm.FilmTitle = managed.FilmTitle;
		}

		return vm;
	}

	// --- Review replies (thread row under a review card) ---

	public static ReviewReplyViewModel ToViewModel(this ReviewReplyDto dto) => new()
	{
		Id = dto.Id,
		UserId = dto.UserId,
		Comment = dto.Comment,
		DateUtc = dto.DateUtc,
		Status = dto.Status,
		IsReported = dto.IsReported,
		HasBeenReported = dto.HasBeenReported,
		IsHiddenByAdmin = dto.IsHiddenByAdmin
	};

	public static AwardViewModel ToViewModel(this AwardDto dto) => new()
	{
		Id = dto.Id,
		FestivalId = dto.FestivalId,
		FestivalName = dto.FestivalName,
		Category = dto.Category,
		Name = dto.Name,
		NominationCount = dto.NominationCount,
		IsActive = dto.IsActive,
		EndDateUtc = dto.EndDateUtc,
		DaysRemaining = dto.DaysRemaining,
		Nominees = dto.Nominees
			.Select(n => new AwardNomineeRowViewModel
			{
				Label = n.Label,
				VoteCount = n.VoteCount,
				VotePercentage = n.VotePercentage,
				ImageUrl = n.ImageUrl
			})
			.ToList()
	};

	public static AdminDashboardViewModel ToViewModel(this AdminDashboardStatsDto dto) => new()
	{
		TotalFestivals = dto.TotalFestivals,
		TotalFilms = dto.TotalFilms,
		ActiveSessions = dto.ActiveSessions,
		TotalPurchases = dto.TotalPurchases,
		ServiceStatus = dto.IsServiceHealthy ? "Healthy" : "Unhealthy"
	};

	public static UserViewModel ToViewModel(this UserDto dto) => new()
	{
		Id = dto.Id,
		Email = dto.Email,
		FullName = dto.Name,
		Roles = dto.Roles.ToList()
	};

	public static RecommendationViewModel ToViewModel(this FilmRecommendationDto dto) => new()
	{
		FilmId = dto.FilmId,
		FestivalFilmId = dto.FestivalFilmId,
		Title = dto.Title,
		PosterUrl = dto.PosterUrl,
		Score = dto.Score
	};
}
