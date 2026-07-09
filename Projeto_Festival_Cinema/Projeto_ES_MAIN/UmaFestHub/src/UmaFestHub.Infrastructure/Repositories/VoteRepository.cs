// -----------------------------------------------------------------------------
// Awards, nominations & votes — EF implementation of IVoteRepository (submit/query votes).
// -----------------------------------------------------------------------------
using UmaFestHub.Domain.Entities;
using UmaFestHub.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using UmaFestHub.Domain.Interfaces;

namespace UmaFestHub.Infrastructure.Repositories
{
    public class VoteRepository : IVoteRepository
    {
        private readonly AppDbContext _dbContext;

        public VoteRepository(AppDbContext dbContext)
        {
            _dbContext = dbContext;
        }

		public async Task<bool> HasVotedAsync(Guid userId, Guid nominationId, CancellationToken cancellationToken = default)
		{
			// Composite PK is (UserId, AwardNominationId). Any matching row means the user already voted.
			return await _dbContext.Votes.AnyAsync(
				v => v.UserId == userId && v.AwardNominationId == nominationId,
				cancellationToken);
		}

		public async Task<bool> HasVotedForAwardAsync(Guid userId, Guid awardId, CancellationToken cancellationToken = default)
		{
			return await (
					from v in _dbContext.Votes
					join n in _dbContext.AwardNominations on v.AwardNominationId equals n.Id
					where v.UserId == userId && n.AwardId == awardId
					select v.UserId
				)
				.AnyAsync(cancellationToken);
		}

		public async Task<IReadOnlySet<Guid>> GetVotedAwardIdsForFestivalAsync(Guid userId, Guid festivalId, CancellationToken cancellationToken = default)
		{
			var ids = await (
					from v in _dbContext.Votes
					join n in _dbContext.AwardNominations on v.AwardNominationId equals n.Id
					join a in _dbContext.Awards on n.AwardId equals a.Id
					where v.UserId == userId && a.FestivalId == festivalId
					select a.Id
				)
				.Distinct()
				.ToListAsync(cancellationToken);

			return ids.ToHashSet();
		}

		public async Task<IReadOnlyDictionary<Guid, Guid>> GetUserVotedNominationIdsByAwardForFestivalAsync(
			Guid userId,
			Guid festivalId,
			CancellationToken cancellationToken = default)
		{
			var rows = await (
					from v in _dbContext.Votes
					join n in _dbContext.AwardNominations on v.AwardNominationId equals n.Id
					join a in _dbContext.Awards on n.AwardId equals a.Id
					where v.UserId == userId && a.FestivalId == festivalId
					select new { AwardId = a.Id, NominationId = v.AwardNominationId }
				)
				.ToListAsync(cancellationToken);

			return rows.ToDictionary(x => x.AwardId, x => x.NominationId);
		}

        public async Task AddAsync(Vote vote, CancellationToken cancellationToken = default)
        {
            await _dbContext.Votes.AddAsync(vote, cancellationToken);
            await _dbContext.SaveChangesAsync(cancellationToken);
        }

        public async Task<IReadOnlyList<Vote>> GetByNominationIdAsync(Guid nominationId, CancellationToken cancellationToken = default)
        {
            return await _dbContext.Votes
                .Where(v => v.AwardNominationId == nominationId)
                .ToListAsync(cancellationToken);
        }

        public async Task<IReadOnlyDictionary<Guid, int>> GetVoteCountsByFilmIdsAsync(
            Guid festivalId,
            IReadOnlyList<Guid> filmIds,
            CancellationToken cancellationToken = default)
        {
            var counts = await (
                    from v in _dbContext.Votes
                    join n in _dbContext.AwardNominations on v.AwardNominationId equals n.Id
                    where n.Award != null && n.Award.FestivalId == festivalId
                        && n.FestivalFilmId != null
                        && filmIds.Contains(n.FestivalFilmId.Value)
                    group v by n.FestivalFilmId!.Value
                    into g
                    select new { FilmId = g.Key, Count = g.Count() }
                )
                .ToDictionaryAsync(x => x.FilmId, x => x.Count, cancellationToken);

            return counts;
        }

        public async Task<IReadOnlyList<Guid>> GetDistinctVoterUserIdsForAwardAsync(Guid awardId, CancellationToken cancellationToken = default)
        {
            return await (
                    from v in _dbContext.Votes
                    join n in _dbContext.AwardNominations on v.AwardNominationId equals n.Id
                    where n.AwardId == awardId
                    select v.UserId
                )
                .Distinct()
                .ToListAsync(cancellationToken);
        }
    }
}
