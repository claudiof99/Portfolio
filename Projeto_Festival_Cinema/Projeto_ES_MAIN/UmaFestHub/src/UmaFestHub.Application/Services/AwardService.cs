// -----------------------------------------------------------------------------
// Awards, nominations & votes — Application orchestration
// Paging/list/create awards, pick four nominees, toggle active; submit/clear votes
// via IVoteRepository; uses INominationValidator list for category rules.
// In-app notifications: publishes voting-closed events via IAwardNotificationNotifier.
// -----------------------------------------------------------------------------
using UmaFestHub.Application.DTOs;
using UmaFestHub.Application.Exceptions;
using UmaFestHub.Application.Interfaces;
using UmaFestHub.Application.Messaging;
using UmaFestHub.Application.Observers.Awards;
using UmaFestHub.Domain.Entities;
using UmaFestHub.Domain.Enums;
using UmaFestHub.Domain.Interfaces;


namespace UmaFestHub.Application.Services
{
	public class AwardService : IAwardService
	{
		private readonly IAwardRepository _awardRepository;
		private readonly INominationRepository _nominationRepository;
		private readonly IVoteRepository _voteRepository;
		private readonly IReadOnlyList<INominationValidator> _validators;
		private readonly IAwardNotificationNotifier _awardNotifications;

		public AwardService(
			IAwardRepository awardRepository,
			INominationRepository nominationRepository,
			IVoteRepository voteRepository,
			IEnumerable<INominationValidator> validators,
			IAwardNotificationNotifier awardNotifications)
		{
			_awardRepository = awardRepository;
			_nominationRepository = nominationRepository;
			_voteRepository = voteRepository;
			_validators = validators.ToList();
			_awardNotifications = awardNotifications;
		}

		public async Task<(IReadOnlyList<AwardDto> Items, bool HasNext)> GetPageAsync(int page, int pageSize, CancellationToken cancellationToken = default)
		{
			if (page < 1)
			{
				page = 1;
			}

			await ExpireDueAwardsAsync(cancellationToken);

			var (awards, hasNext) = await _awardRepository.GetPageWithNominationsAsync(page, pageSize, cancellationToken);
			return (awards.Select(MapToDto).ToList(), hasNext);
		}

		public async Task<IReadOnlyList<AwardDto>> GetAllAsync(CancellationToken cancellationToken = default)
		{
			await ExpireDueAwardsAsync(cancellationToken);

			var awards = await _awardRepository.GetAllWithNominationsAsync(cancellationToken);
			return awards.Select(MapToDto).ToList();
		}

		public async Task<IReadOnlyList<AwardDto>> GetByFestivalIdAsync(Guid festivalId, CancellationToken cancellationToken = default)
		{
			await ExpireDueAwardsAsync(cancellationToken);

			var awards = await _awardRepository.GetByFestivalIdAsync(festivalId, cancellationToken);
			return awards.Select(MapToDto).ToList();
		}

		public async Task<IReadOnlyList<AwardDto>> GetByFestivalIdAvailableForVotingAsync(Guid festivalId, Guid userId, CancellationToken cancellationToken = default)
		{
			await ExpireDueAwardsAsync(cancellationToken);

			var awards = await _awardRepository.GetByFestivalIdAsync(festivalId, cancellationToken);
			var votedAwardIds = await _voteRepository.GetVotedAwardIdsForFestivalAsync(userId, festivalId, cancellationToken);

			return awards
				.Where(a => a.IsActive && !votedAwardIds.Contains(a.Id))
				.Select(MapToDto)
				.ToList();
		}

		public async Task<IReadOnlyList<UserAwardVoteDto>> GetVotedAwardsForFestivalAsync(
			Guid festivalId,
			Guid userId,
			CancellationToken cancellationToken = default)
		{
			await ExpireDueAwardsAsync(cancellationToken);

			var votesByAwardId = await _voteRepository.GetUserVotedNominationIdsByAwardForFestivalAsync(
				userId,
				festivalId,
				cancellationToken);
			if (votesByAwardId.Count == 0)
			{
				return Array.Empty<UserAwardVoteDto>();
			}

			var awards = await _awardRepository.GetByFestivalIdAsync(festivalId, cancellationToken);
			return awards
				.Where(a => votesByAwardId.ContainsKey(a.Id))
				.Select(a => new UserAwardVoteDto
				{
					Award = MapToDto(a),
					SelectedNominationId = votesByAwardId[a.Id]
				})
				.ToList();
		}

		private static AwardDto MapToDto(Award x)
		{
			var daysRemaining = ComputeDaysRemaining(x);
			return new AwardDto
			{
				Id = x.Id,
				FestivalId = x.FestivalId,
				FestivalName = x.Festival?.Name ?? string.Empty,
				Category = x.Category.ToString(),
				Name = x.Name,
				NominationCount = x.Nominations.Count,
				IsActive = x.IsActive,
				EndDateUtc = x.EndDateUtc,
				DaysRemaining = daysRemaining,
				Nominees = MapNominees(x.Nominations)
			};
		}

		private static int ComputeDaysRemaining(Award award)
		{
			if (!award.IsActive)
			{
				return 0;
			}

			return Math.Max(0, (award.EndDateUtc.Date - DateTime.UtcNow.Date).Days);
		}

		private static IReadOnlyList<AwardNomineeDto> MapNominees(ICollection<AwardNomination> nominations)
		{
			var totalVotes = nominations.Sum(n => n.Votes.Count);
			return nominations.Select(n => NomineeLabel(n, totalVotes)).ToList();
		}

		private static AwardNomineeDto NomineeLabel(AwardNomination n, int totalVotes)
		{
			var voteCount = n.Votes.Count;
			var votePercentage = totalVotes <= 0
				? 0
				: (int)Math.Round(voteCount * 100d / totalVotes, MidpointRounding.AwayFromZero);

			if (n.FestivalFilmId is not null && n.FestivalFilm?.Film is { } film)
			{
				return new AwardNomineeDto(n.Id, film.Name, voteCount, votePercentage, film.ImageUrl);
			}

			if (n.CreditFilmId is not null && n.CreditFilm?.Person is { } person)
			{
				return new AwardNomineeDto(n.Id, person.Name, voteCount, votePercentage, person.ImageUrl);
			}

			return new AwardNomineeDto(n.Id, "Award_UnknownNominee", voteCount, votePercentage);
		}

		public async Task<Guid> CreateAsync(AwardDto award, CancellationToken cancellationToken = default)
		{
			var createdAtUtc = DateTime.UtcNow;
			var endDateUtc = award.EndDateUtc > createdAtUtc
				? award.EndDateUtc
				: createdAtUtc.AddDays(30);

			var entity = Award.Create(
				award.FestivalId,
				award.Name,
				AwardCategory.Film,
				createdAtUtc,
				endDateUtc);

			await _awardRepository.AddAsync(entity, cancellationToken);
			return entity.Id;
		}

		public async Task<Guid> CreateWithNomineesAsync(
			Guid festivalId,
			string awardName,
			AwardCategory category,
			IReadOnlyList<Guid> nomineeIds,
			DateTime endDateUtc,
			CancellationToken cancellationToken = default)
		{
			var validator = _validators.FirstOrDefault(v => v.Category == category);
			if (validator is null)
			{
				throw new InvalidOperationException("Nomination_NoValidator");
			}

			var createdAtUtc = DateTime.UtcNow;
			if (endDateUtc <= createdAtUtc)
			{
				throw new InvalidOperationException("Award_EndDateMustBeFuture");
			}

			var (award, nominations) = Award.CreateWithNominees(
				festivalId,
				awardName,
				category,
				nomineeIds,
				createdAtUtc,
				endDateUtc);

			await _awardRepository.AddAsync(award, cancellationToken);

			foreach (var nomination in nominations)
			{
				var (valid, error) = await validator.ValidateAsync(nomination, cancellationToken);
				if (!valid)
				{
					throw new InvalidOperationException(error ?? "Nomination_Invalid");
				}

				await _nominationRepository.AddAsync(nomination, cancellationToken);
			}

			return award.Id;
		}

		public async Task ExpireDueAwardsAsync(CancellationToken cancellationToken = default)
		{
			var expiredAwardIds = await _awardRepository.GetActiveAwardIdsPastEndDateAsync(DateTime.UtcNow, cancellationToken);
			foreach (var awardId in expiredAwardIds)
			{
				await DeactivateAsync(awardId, Guid.Empty, cancellationToken);
			}
		}

		public async Task DeactivateAsync(Guid awardId, Guid deactivatedByUserId, CancellationToken cancellationToken = default)
		{
			if (awardId == Guid.Empty)
			{
				throw new ArgumentException("AwardId is required.", nameof(awardId));
			}

			var award = await _awardRepository.GetByIdWithNominationsAsync(awardId, cancellationToken);
			if (award is null)
			{
				throw new InvalidOperationException("Award not found.");
			}

			if (!award.IsActive)
			{
				return;
			}

			var voterIds = await _voteRepository.GetDistinctVoterUserIdsForAwardAsync(awardId, cancellationToken);
			var results = BuildAwardResultLines(award);

			var updated = await _awardRepository.TrySetIsActiveAsync(awardId, false, cancellationToken);
			if (!updated)
			{
				throw new InvalidOperationException("Award not found.");
			}

			await _awardNotifications.NotifyAwardVotingClosedAsync(
				new AwardVotingClosedContext(awardId, award.Name, deactivatedByUserId, results, voterIds),
				cancellationToken);
		}

		private static IReadOnlyList<AwardResultLine> BuildAwardResultLines(Award award)
		{
			var nominations = award.Nominations;
			var totalVotes = nominations.Sum(n => n.Votes.Count);
			return nominations
				.Select(n => new AwardResultLine(ResultLabelForNomination(n), VotePercent(n.Votes.Count, totalVotes)))
				.ToList();
		}

		private static int VotePercent(int voteCount, int totalVotes)
		{
			return totalVotes <= 0
				? 0
				: (int)Math.Round(voteCount * 100d / totalVotes, MidpointRounding.AwayFromZero);
		}

		private static string ResultLabelForNomination(AwardNomination n)
		{
			if (n.FestivalFilmId is not null && n.FestivalFilm?.Film is { } film)
			{
				return film.Name;
			}

			if (n.CreditFilmId is not null && n.CreditFilm?.Person is { } person)
			{
				return person.Name;
			}

			return "Award_UnknownNominee";
		}

		public async Task<Guid> NominateAsync(Guid awardId, Guid festivalFilmId, CancellationToken cancellationToken = default)
		{
			var nomination = AwardNomination.CreateFilmNomination(awardId, festivalFilmId);

			await _nominationRepository.AddAsync(nomination, cancellationToken);
			return nomination.Id;
		}

		public async Task VoteAsync(Guid userId, Guid nominationId, CancellationToken cancellationToken = default)
		{
			var nomination = await _nominationRepository.GetByIdWithVotesAsync(nominationId, cancellationToken);
			if (nomination is null)
			{
				throw new UserFacingException(new UserMessage(UserMessageKeys.Vote_NominationNotFound));
			}

			var awardEntity = await _awardRepository.GetByIdAsync(nomination.AwardId, cancellationToken);
			if (awardEntity is null || !awardEntity.IsActive || awardEntity.EndDateUtc <= DateTime.UtcNow)
			{
				if (awardEntity is not null && awardEntity.IsActive && awardEntity.EndDateUtc <= DateTime.UtcNow)
				{
					await DeactivateAsync(awardEntity.Id, Guid.Empty, cancellationToken);
				}

				throw new UserFacingException(new UserMessage(UserMessageKeys.Vote_AwardClosed));
			}

			var hasVotedForAward = await _voteRepository.HasVotedForAwardAsync(userId, nomination.AwardId, cancellationToken);
			if (hasVotedForAward)
			{
				throw new UserFacingException(new UserMessage(UserMessageKeys.Vote_AlreadyVotedAward));
			}

			var hasVotedForNominee = await _voteRepository.HasVotedAsync(userId, nominationId, cancellationToken);
			if (hasVotedForNominee)
			{
				throw new UserFacingException(new UserMessage(UserMessageKeys.Vote_AlreadyVotedNominee));
			}

			var vote = new Vote
			{
				Id = Guid.NewGuid(),
				AwardNominationId = nominationId,
				UserId = userId,
				CreatedAtUtc = DateTime.UtcNow
			};
			await _voteRepository.AddAsync(vote, cancellationToken);
		}

		public async Task<AwardNomination?> GetWinnerAsync(Guid awardId, CancellationToken cancellationToken = default)
		{
			var nominations = await _nominationRepository.GetByAwardIdAsync(awardId, cancellationToken);
			AwardNomination? winner = null;
			var maxVotes = -1;
			foreach (var nomination in nominations)
			{
				var votes = await _voteRepository.GetByNominationIdAsync(nomination.Id, cancellationToken);
				if (votes.Count > maxVotes)
				{
					maxVotes = votes.Count;
					winner = nomination;
				}
			}
			return winner;
		}
	}
}
