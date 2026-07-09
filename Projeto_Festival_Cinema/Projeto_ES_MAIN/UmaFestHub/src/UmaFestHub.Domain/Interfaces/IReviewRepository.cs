using UmaFestHub.Domain.Entities;

namespace UmaFestHub.Domain.Interfaces;

// Persistence contract for reviews.
// Application services depend on this interface so data access can be swapped (EF, tests, etc.).
public interface IReviewRepository
{
	Task<IReadOnlyList<Review>> GetAllAsync(CancellationToken cancellationToken = default);
	Task<IReadOnlyList<Review>> GetAllPageAsync(int skip, int take, CancellationToken cancellationToken = default);
	Task<IReadOnlyList<Review>> GetAllFilteredPageAsync(
		string? movieQuery,
		string? status,
		DateTime? dayUtc,
		IReadOnlyList<Guid>? userIds,
		int skip,
		int take,
		CancellationToken cancellationToken = default);
	Task<IReadOnlyList<Review>> GetForFestivalFilmAsync(Guid festivalFilmId, Guid? viewerUserId = null, CancellationToken cancellationToken = default);
	Task<IReadOnlyList<Review>> GetForFestivalFilmPageAsync(Guid festivalFilmId, Guid? viewerUserId, int skip, int take, CancellationToken cancellationToken = default);
	Task<IReadOnlyList<Review>> GetApprovedForFilmsAsync(IReadOnlyList<Guid> filmIds, CancellationToken cancellationToken = default);
	Task<Review?> GetByIdAsync(Guid reviewId, CancellationToken cancellationToken = default);

	/// <summary>Loads a review with film and festival navigations for author-facing notifications.</summary>
	Task<Review?> GetByIdWithFestivalAndFilmAsync(Guid reviewId, CancellationToken cancellationToken = default);

	Task AddAsync(Review review, CancellationToken cancellationToken = default);
	Task UpdateAsync(Review review, CancellationToken cancellationToken = default);
}
