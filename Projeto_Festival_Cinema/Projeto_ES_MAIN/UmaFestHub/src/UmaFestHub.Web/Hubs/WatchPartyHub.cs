// Live Watch Party: SignalR hub — synced YouTube playback + chat for entitled users.
// Explicit role selection: CreateParty (host) or JoinPartyWithCode (guest).
using System.Collections.Concurrent;
using System.Security.Claims;
using System.Security.Cryptography;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using UmaFestHub.Application.Interfaces;

namespace UmaFestHub.Web.Hubs;

/// <summary>Party lifecycle phase: Lobby (waiting room) → Live (video playing).</summary>
public enum PartyPhase { Lobby, Live }

/// <summary>
/// Real-time watch-party: host-controlled YouTube playback sync + group chat.
/// In-memory state (no DB entity) keyed by "{festivalFilmId}:{sessionId|none}".
/// </summary>
[Authorize]
public sealed class WatchPartyHub : Hub
{
	private static readonly ConcurrentDictionary<string, PartyState> _parties = new();

	/// <summary>Reverse lookup: connectionId → partyKey, so OnDisconnectedAsync can find the party.</summary>
	private static readonly ConcurrentDictionary<string, string> _connectionToParty = new();

	/// <summary>Unambiguous alphabet for join codes — no 0/O, 1/I, no lowercase.</summary>
	private const string JoinCodeAlphabet = "ABCDEFGHJKLMNPQRSTUVWXYZ23456789";
	private const int JoinCodeLength = 6;

	private readonly IEntitlementService _entitlementService;

	public WatchPartyHub(IEntitlementService entitlementService)
	{
		_entitlementService = entitlementService;
	}

	// ── Hub methods ──────────────────────────────────────────

	/// <summary>
	/// Called when a user explicitly chooses "Host this Party".
	/// Creates a new party or lets the same user resume hosting.
	/// </summary>
	public async Task CreateParty(Guid festivalId, Guid festivalFilmId, Guid? sessionId, string youtubeVideoId)
	{
		var userId = GetUserId();

		// Re-verify entitlement (defense-in-depth).
		var allowed = await _entitlementService.CanWatchMovieAsync(
			userId, festivalId, festivalFilmId, sessionId, Context.ConnectionAborted);

		if (!allowed)
			throw new HubException("access-denied");

		var partyKey = MakePartyKey(festivalFilmId, sessionId);

		// Build candidate — only used if no party exists yet.
		var candidate = new PartyState
		{
			HostConnectionId = Context.ConnectionId,
			HostUserId = userId,
			YoutubeVideoId = youtubeVideoId,
			Phase = PartyPhase.Lobby,
			JoinCode = GenerateJoinCode(),
			LastUpdateUtc = DateTime.UtcNow
		};

		var state = _parties.GetOrAdd(partyKey, candidate);
		bool isCreator = ReferenceEquals(state, candidate);

		// ── Branch: party already existed ──
		if (!isCreator)
		{
			bool isResume;
			lock (state.Lock)
			{
				isResume = state.HostUserId == userId;
			}

			if (!isResume)
			{
				// Different active host — reject.
				await Clients.Caller.SendAsync("PartyAlreadyExists");
				return;
			}

			// Same user resuming host (e.g. page refresh while guests are present).
			// Evict stale connections for this user, then re-seat as host.
			lock (state.Lock)
			{
				EvictStaleConnections(state, userId, Context.ConnectionId);

				state.HostConnectionId = Context.ConnectionId;
				// Do NOT overwrite YoutubeVideoId, JoinCode, Phase — keep originals.

				if (!state.JoinOrder.Contains(Context.ConnectionId))
					state.JoinOrder.Add(Context.ConnectionId);

				state.ConnectionToUserName[Context.ConnectionId] = GetDisplayName();
				state.ConnectionToUserId[Context.ConnectionId] = userId;
			}
		}
		else
		{
			// ── Branch: we just created the party ──
			lock (state.Lock)
			{
				if (!state.JoinOrder.Contains(Context.ConnectionId))
					state.JoinOrder.Add(Context.ConnectionId);

				state.ConnectionToUserName[Context.ConnectionId] = GetDisplayName();
				state.ConnectionToUserId[Context.ConnectionId] = userId;
			}
		}

		await Groups.AddToGroupAsync(Context.ConnectionId, partyKey);
		_connectionToParty[Context.ConnectionId] = partyKey;

		// Build response
		string joinCode;
		string videoId;
		PartyPhase phase;
		bool isPlaying;
		double computedPosition;
		List<string> participants;

		lock (state.Lock)
		{
			joinCode = state.JoinCode;
			videoId = state.YoutubeVideoId;
			phase = state.Phase;
			isPlaying = state.IsPlaying;
			participants = state.ConnectionToUserName.Values.ToList();

			computedPosition = (phase == PartyPhase.Live && isPlaying)
				? state.PositionSeconds + (DateTime.UtcNow - state.LastUpdateUtc).TotalSeconds
				: state.PositionSeconds;
		}

		await Clients.Caller.SendAsync("JoinedParty", new
		{
			isHost = true,
			isPlaying,
			position = computedPosition,
			youtubeVideoId = videoId,
			partyKey,
			joinCode,
			phase = phase.ToString(),
			participants
		});

		if (phase == PartyPhase.Lobby)
		{
			await Clients.Group(partyKey).SendAsync("LobbyUpdate", participants);
		}
	}

	/// <summary>
	/// Called when a user explicitly chooses "Join with a code".
	/// Validates the code and adds the caller as a guest.
	/// </summary>
	public async Task JoinPartyWithCode(Guid festivalId, Guid festivalFilmId, Guid? sessionId, string joinCode)
	{
		var userId = GetUserId();

		// Re-verify entitlement (defense-in-depth).
		var allowed = await _entitlementService.CanWatchMovieAsync(
			userId, festivalId, festivalFilmId, sessionId, Context.ConnectionAborted);

		if (!allowed)
			throw new HubException("access-denied");

		var partyKey = MakePartyKey(festivalFilmId, sessionId);

		if (!_parties.TryGetValue(partyKey, out var state))
		{
			// No party exists for this film — nowhere to join.
			await Clients.Caller.SendAsync("InvalidJoinCode");
			return;
		}

		// Validate join code
		string requiredCode;
		lock (state.Lock)
		{
			requiredCode = state.JoinCode;
		}

		if (string.IsNullOrWhiteSpace(joinCode)
			|| !string.Equals(joinCode.Trim(), requiredCode, StringComparison.OrdinalIgnoreCase))
		{
			await Clients.Caller.SendAsync("InvalidJoinCode");
			return;
		}

		// Add as guest (never as host)
		await Groups.AddToGroupAsync(Context.ConnectionId, partyKey);
		_connectionToParty[Context.ConnectionId] = partyKey;

		lock (state.Lock)
		{
			if (!state.JoinOrder.Contains(Context.ConnectionId))
				state.JoinOrder.Add(Context.ConnectionId);

			state.ConnectionToUserName[Context.ConnectionId] = GetDisplayName();
			state.ConnectionToUserId[Context.ConnectionId] = userId;
		}

		// Build response
		double computedPosition;
		bool isPlaying;
		string videoId;
		PartyPhase phase;
		List<string> participants;

		lock (state.Lock)
		{
			isPlaying = state.IsPlaying;
			videoId = state.YoutubeVideoId;
			phase = state.Phase;
			participants = state.ConnectionToUserName.Values.ToList();

			computedPosition = (phase == PartyPhase.Live && isPlaying)
				? state.PositionSeconds + (DateTime.UtcNow - state.LastUpdateUtc).TotalSeconds
				: state.PositionSeconds;
		}

		await Clients.Caller.SendAsync("JoinedParty", new
		{
			isHost = false,
			isPlaying,
			position = computedPosition,
			youtubeVideoId = videoId,
			partyKey,
			joinCode = (string?)null, // Guests don't receive the code in JoinedParty
			phase = phase.ToString(),
			participants
		});

		if (phase == PartyPhase.Lobby)
		{
			await Clients.Group(partyKey).SendAsync("LobbyUpdate", participants);
		}
	}

	public async Task StartParty(string partyKey)
	{
		if (!_parties.TryGetValue(partyKey, out var state))
			return;

		lock (state.Lock)
		{
			// Only the host may start the party
			if (Context.ConnectionId != state.HostConnectionId)
				return;

			// Idempotency: only transition from Lobby → Live
			if (state.Phase != PartyPhase.Lobby)
				return;

			state.Phase = PartyPhase.Live;
			state.LiveStartUtc = DateTime.UtcNow;
			state.LastUpdateUtc = DateTime.UtcNow;
			state.PositionSeconds = 0;
			state.IsPlaying = false;
		}

		await Clients.Group(partyKey).SendAsync("PartyStarted");
	}

	public async Task SendPlaybackAction(string partyKey, string action, double positionSeconds)
	{
		if (!_parties.TryGetValue(partyKey, out var state))
			return;

		lock (state.Lock)
		{
			if (Context.ConnectionId != state.HostConnectionId)
				return; // Only the host may control playback.

			if (state.Phase != PartyPhase.Live)
				return; // No playback commands during Lobby.

			state.PositionSeconds = positionSeconds;
			state.LastUpdateUtc = DateTime.UtcNow;
			state.IsPlaying = action switch
			{
				"play" => true,
				"pause" => false,
				_ => state.IsPlaying // "seek" preserves current play/pause state
			};
		}

		await Clients.OthersInGroup(partyKey)
			.SendAsync("ReceivePlaybackAction", action, positionSeconds);
	}

	public async Task SendChatMessage(string partyKey, string message)
	{
		if (string.IsNullOrWhiteSpace(message))
			return;

		// Only members who passed CreateParty/JoinPartyWithCode (entitlement-checked) may chat.
		if (!_connectionToParty.TryGetValue(Context.ConnectionId, out var actualPartyKey)
			|| actualPartyKey != partyKey)
			return;

		var displayName = GetDisplayName();

		await Clients.Group(partyKey)
			.SendAsync("ReceiveChatMessage", displayName, message, DateTime.UtcNow);
	}

	public Task Heartbeat(string partyKey, double positionSeconds)
	{
		if (!_parties.TryGetValue(partyKey, out var state))
			return Task.CompletedTask;

		lock (state.Lock)
		{
			if (Context.ConnectionId != state.HostConnectionId)
				return Task.CompletedTask;

			if (state.Phase != PartyPhase.Live)
				return Task.CompletedTask;

			state.PositionSeconds = positionSeconds;
			state.LastUpdateUtc = DateTime.UtcNow;
		}

		return Task.CompletedTask;
	}

	// ── Lifecycle ────────────────────────────────────────────

	public override async Task OnDisconnectedAsync(Exception? exception)
	{
		if (_connectionToParty.TryRemove(Context.ConnectionId, out var partyKey)
			&& _parties.TryGetValue(partyKey, out var state))
		{
			string? newHostConnectionId = null;
			string? joinCodeForNewHost = null;
			PartyPhase currentPhase;
			List<string>? updatedParticipants = null;

			lock (state.Lock)
			{
				state.JoinOrder.Remove(Context.ConnectionId);
				state.ConnectionToUserName.Remove(Context.ConnectionId);
				state.ConnectionToUserId.Remove(Context.ConnectionId);

				if (Context.ConnectionId == state.HostConnectionId)
				{
					if (state.JoinOrder.Count > 0)
					{
						state.HostConnectionId = state.JoinOrder[0];
						newHostConnectionId = state.HostConnectionId;

						// Update HostUserId to the promoted user's id
						if (state.ConnectionToUserId.TryGetValue(newHostConnectionId, out var promotedUserId))
							state.HostUserId = promotedUserId;

						joinCodeForNewHost = state.JoinCode;
						// Phase and JoinCode stay unchanged — new host inherits lobby control
					}
					else
					{
						// Party is empty — remove it.
						_parties.TryRemove(partyKey, out _);
					}
				}

				currentPhase = state.Phase;

				// Gather participant list for lobby update (only if party still exists)
				if (state.JoinOrder.Count > 0 && currentPhase == PartyPhase.Lobby)
				{
					updatedParticipants = state.ConnectionToUserName.Values.ToList();
				}
			}

			if (newHostConnectionId is not null)
			{
				// Send join code to new host so they can display it
				await Clients.Client(newHostConnectionId)
					.SendAsync("YouAreNowHost", new { joinCode = joinCodeForNewHost });
				await Clients.Group(partyKey).SendAsync("HostChanged", newHostConnectionId);
			}

			// Keep lobby participant list current
			if (updatedParticipants is not null)
			{
				await Clients.Group(partyKey).SendAsync("LobbyUpdate", updatedParticipants);
			}
		}

		await base.OnDisconnectedAsync(exception);
	}

	// ── Helpers ──────────────────────────────────────────────

	private static string MakePartyKey(Guid festivalFilmId, Guid? sessionId)
		=> $"{festivalFilmId}:{sessionId?.ToString() ?? "none"}";

	private static string GenerateJoinCode()
	{
		Span<byte> bytes = stackalloc byte[JoinCodeLength];
		RandomNumberGenerator.Fill(bytes);
		var chars = new char[JoinCodeLength];
		for (int i = 0; i < JoinCodeLength; i++)
		{
			chars[i] = JoinCodeAlphabet[bytes[i] % JoinCodeAlphabet.Length];
		}
		return new string(chars);
	}

	/// <summary>Remove stale connection entries for a given userId, except the specified current connectionId.</summary>
	private static void EvictStaleConnections(PartyState state, Guid userId, string currentConnectionId)
	{
		// Must be called inside lock(state.Lock)
		var staleConns = state.ConnectionToUserId
			.Where(kvp => kvp.Value == userId && kvp.Key != currentConnectionId)
			.Select(kvp => kvp.Key)
			.ToList();

		foreach (var stale in staleConns)
		{
			state.JoinOrder.Remove(stale);
			state.ConnectionToUserName.Remove(stale);
			state.ConnectionToUserId.Remove(stale);
		}
	}

	private Guid GetUserId()
	{
		var claim = Context.User?.FindFirst("sub")?.Value
			?? Context.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;

		if (Guid.TryParse(claim, out var id))
			return id;

		throw new HubException("Unable to determine user identity.");
	}

	private string GetDisplayName()
	{
		return Context.User?.FindFirst(ClaimTypes.Name)?.Value
			?? Context.User?.FindFirst("name")?.Value
			?? "Guest";
	}

	// ── In-memory party state ────────────────────────────────

	private sealed class PartyState
	{
		public readonly object Lock = new();
		public string HostConnectionId { get; set; } = string.Empty;
		public Guid HostUserId { get; set; }
		public bool IsPlaying { get; set; }
		public double PositionSeconds { get; set; }
		public DateTime LastUpdateUtc { get; set; } = DateTime.UtcNow;
		public string YoutubeVideoId { get; set; } = string.Empty;
		public List<string> JoinOrder { get; } = new();
		public Dictionary<string, string> ConnectionToUserName { get; } = new();
		public Dictionary<string, Guid> ConnectionToUserId { get; } = new();

		// ── Lobby / join-code fields ──
		public PartyPhase Phase { get; set; } = PartyPhase.Lobby;
		public string JoinCode { get; set; } = string.Empty;
		public DateTime? LiveStartUtc { get; set; }
	}
}
