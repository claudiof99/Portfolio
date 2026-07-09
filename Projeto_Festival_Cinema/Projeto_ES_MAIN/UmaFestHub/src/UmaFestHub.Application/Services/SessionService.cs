using UmaFestHub.Application.DTOs;
using UmaFestHub.Application.Interfaces;
using UmaFestHub.Application.Messaging;
using UmaFestHub.Domain.Entities;
using UmaFestHub.Application.Factories;
using UmaFestHub.Domain.Interfaces;

namespace UmaFestHub.Application.Services;

/// <summary>
/// We use this service to manage festival sessions. It handles retrieving, creating, and mapping our session entities.
/// </summary>
public class SessionService : ISessionService
{
	private readonly ISessionRepository _sessionRepository;
    private readonly Dictionary<string, SessionStore> _stores;

	public SessionService(
		ISessionRepository sessionRepository,
		IEnumerable<SessionStore> stores)
	{
		_sessionRepository = sessionRepository;
        _stores = stores.ToDictionary(s => s.SessionType.ToLowerInvariant());
	}

	/// <summary>
	/// We retrieve all available sessions asynchronously and map them to our Data Transfer Objects (DTOs).
	/// </summary>
	public async Task<IReadOnlyList<SessionDto>> GetAllAsync(CancellationToken cancellationToken = default)
	{
		var sessions = await _sessionRepository.GetAllAsync(cancellationToken);
		return sessions.Select(Map).ToList();
	}

	/// <summary>
	/// We fetch all sessions associated with a specific festival film and map them to our DTOs.
	/// </summary>
	public async Task<IReadOnlyList<SessionDto>> GetByFestivalFilmIdAsync(Guid festivalFilmId, CancellationToken cancellationToken = default)
	{
		if (festivalFilmId == Guid.Empty)
            return [];

		var sessions = await _sessionRepository.GetByFestivalFilmIdAsync(festivalFilmId, cancellationToken);
		return sessions.Select(Map).ToList();
	}

	/// <summary>
	/// We create a new session based on the provided DTO, save it using our repository, and return the newly generated session ID.
	/// </summary>
	public async Task<(bool Succeeded, Guid? Id, UserMessage? Error)> CreateAsync(SessionDto sessionDto, CancellationToken cancellationToken = default)
	{
		if (sessionDto.FestivalFilmId == Guid.Empty)
		{
			return (false, null, new UserMessage(UserMessageKeys.Session_FestivalFilmRequired));
		}

        if (!_stores.TryGetValue(sessionDto.SessionType.Trim().ToLowerInvariant(), out var store))
		{
			return (false, null, new UserMessage(UserMessageKeys.Session_UnknownType, sessionDto.SessionType));
		}

		var existing = await _sessionRepository.GetByFestivalFilmIdAsync(sessionDto.FestivalFilmId, cancellationToken);
		var isDuplicate = existing.Any(s =>
			s.SessionType == sessionDto.SessionType &&
			s.StartTimeUtc == sessionDto.StartTimeUtc);

		if (isDuplicate)
		{
			return (false, null, new UserMessage(UserMessageKeys.Session_DuplicateStartTime));
		}

		try
		{
			var session = store.Create(sessionDto.FestivalFilmId, sessionDto.StartTimeUtc, sessionDto.EndTimeUtc);
			await _sessionRepository.AddAsync(session, cancellationToken);
            return (true, session.Id, null);
		}
		catch (ArgumentException)
		{
			return (false, null, new UserMessage(UserMessageKeys.Session_InvalidSchedule));
		}
	}


	/// <summary>
	/// We map the core Session entity to a SessionDto to safely pass data back out of the application layer.
	/// </summary>
	private static SessionDto Map(Session session) =>
		new(
			session.Id,
			session.FestivalFilmId,
			session.SessionType,
			session.StartTimeUtc,
			session.EndTimeUtc);
}
