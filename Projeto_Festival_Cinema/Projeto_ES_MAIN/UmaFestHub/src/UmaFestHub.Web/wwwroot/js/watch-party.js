/**
 * Live Watch Party: SignalR-connected YouTube sync + chat + lobby/join-code gate.
 * Explicit role selection: user picks "Host" or "Join with a code" before any party action.
 * Structured like signalr-notifications.js (IIFE, data-attribute guards, HubConnectionBuilder pattern).
 */
(function () {
  var body = document.body;
  if (!body || body.dataset.watchParty !== "true") {
    return;
  }
  if (!window.signalR) {
    console.warn("Watch Party: signalR not loaded.");
    return;
  }

  // ── Read data attributes ──
  var festivalId = body.dataset.festivalId || "";
  var festivalFilmId = body.dataset.festivalFilmId || "";
  var rawSessionId = body.dataset.sessionId || "";
  var sessionId = rawSessionId && rawSessionId !== "" ? rawSessionId : null;
  var youtubeVideoId = body.dataset.youtubeVideoId || "";
  var displayName = body.dataset.displayName || "Guest";
  var joinCodeFromUrl = body.dataset.joinCode || "";

  // ── DOM refs ──
  var chatMessages = document.getElementById("wpChatMessages");
  var chatInput = document.getElementById("wpChatInput");
  var chatSend = document.getElementById("wpChatSend");
  var hostBadge = document.getElementById("wpHostBadge");
  var guestBadge = document.getElementById("wpGuestBadge");
  var statusEl = document.getElementById("wpStatus");

  // ── Lobby / role-select DOM refs ──
  var lobbyEl = document.getElementById("wpLobby");
  var roleSelect = document.getElementById("wpRoleSelect");
  var btnHost = document.getElementById("wpBtnHost");
  var btnJoin = document.getElementById("wpBtnJoin");
  var roleError = document.getElementById("wpRoleError");
  var joinCodeEntry = document.getElementById("wpJoinCodeEntry");
  var joinCodeInput = document.getElementById("wpJoinCodeInput");
  var joinCodeSubmit = document.getElementById("wpJoinCodeSubmit");
  var joinCodeError = document.getElementById("wpJoinCodeError");
  var hostLobby = document.getElementById("wpHostLobby");
  var guestLobby = document.getElementById("wpGuestLobby");
  var joinCodeDisplay = document.getElementById("wpJoinCodeDisplay");
  var copyCodeBtn = document.getElementById("wpCopyCode");
  var startPartyBtn = document.getElementById("wpStartParty");
  var participantList = document.getElementById("wpParticipantList");
  var guestParticipantList = document.getElementById("wpGuestParticipantList");

  // ── Reaction bar DOM ref ──
  var reactionBar = document.getElementById("wpReactionBar");

  // ── State ──
  var isHost = false;
  var partyKey = "";
  var partyPhase = "";       // "Lobby" or "Live"
  var hasJoinedParty = false;
  var myJoinCode = "";       // stored after successful join (for reconnect)
  var lastTriedCode = "";    // track last code attempted (for share-link fallback)
  var ytPlayer = null;
  var seekCheckInterval = null;
  var heartbeatInterval = null;
  var lastKnownTime = 0;
  var suppressEvents = false; // Suppress YT state-change events during programmatic seeks

  // ── Curated reaction bar (8 emojis, send instantly to chat) ──
  var REACTIONS = ["\uD83D\uDE02", "\u2764\uFE0F", "\uD83D\uDD25", "\uD83D\uDE22", "\uD83D\uDC4F", "\uD83C\uDF7F", "\uD83D\uDE31", "\uD83D\uDC4D"];

  // ── Status indicator ──
  function setStatus(text, cssClass) {
    if (!statusEl) return;
    statusEl.textContent = text;
    statusEl.className = "wp-status visible " + (cssClass || "");
    if (cssClass === "connected") {
      setTimeout(function () {
        statusEl.classList.remove("visible");
      }, 2500);
    }
  }

  // ── YouTube IFrame API ──
  function loadYouTubeAPI(callback) {
    if (window.YT && window.YT.Player) {
      callback();
      return;
    }
    var tag = document.createElement("script");
    tag.src = "https://www.youtube.com/iframe_api";
    document.head.appendChild(tag);
    window.onYouTubeIframeAPIReady = callback;
  }

  function createPlayer(videoId, onReady) {
    ytPlayer = new YT.Player("player", {
      videoId: videoId,
      playerVars: {
        controls: 0,
        disablekb: 1,
        modestbranding: 1,
        rel: 0,
        iv_load_policy: 3,
        fs: 0,
        playsinline: 1,
        enablejsapi: 1,
        origin: window.location.origin,
      },
      events: {
        onReady: function () {
          if (onReady) onReady();
        },
        onStateChange: function (event) {
          if (suppressEvents) return;
          if (!isHost) return;

          var state = event.data;
          if (state === YT.PlayerState.PLAYING) {
            lastKnownTime = ytPlayer.getCurrentTime();
            connection.invoke("SendPlaybackAction", partyKey, "play", lastKnownTime).catch(logError);
          } else if (state === YT.PlayerState.PAUSED) {
            lastKnownTime = ytPlayer.getCurrentTime();
            connection.invoke("SendPlaybackAction", partyKey, "pause", lastKnownTime).catch(logError);
          }
        },
      },
    });
  }

  function enableHostControls() {
    if (!ytPlayer || typeof ytPlayer.getIframe !== "function") return;

    // Re-create player with controls enabled for host
    var currentTime = 0;
    var wasPlaying = false;
    try {
      currentTime = ytPlayer.getCurrentTime() || 0;
      wasPlaying = ytPlayer.getPlayerState() === YT.PlayerState.PLAYING;
    } catch (e) {
      // Player may not be fully ready
    }

    ytPlayer.destroy();
    ytPlayer = new YT.Player("player", {
      videoId: youtubeVideoId,
      playerVars: {
        controls: 1,
        disablekb: 0,
        modestbranding: 1,
        rel: 0,
        iv_load_policy: 3,
        fs: 0,
        playsinline: 1,
        enablejsapi: 1,
        origin: window.location.origin,
        start: Math.floor(currentTime),
      },
      events: {
        onReady: function () {
          ytPlayer.seekTo(currentTime, true);
          if (wasPlaying) ytPlayer.playVideo();
          startSeekDetection();
          startHeartbeat();
        },
        onStateChange: function (event) {
          if (suppressEvents) return;
          var state = event.data;
          if (state === YT.PlayerState.PLAYING) {
            lastKnownTime = ytPlayer.getCurrentTime();
            connection.invoke("SendPlaybackAction", partyKey, "play", lastKnownTime).catch(logError);
          } else if (state === YT.PlayerState.PAUSED) {
            lastKnownTime = ytPlayer.getCurrentTime();
            connection.invoke("SendPlaybackAction", partyKey, "pause", lastKnownTime).catch(logError);
          }
        },
      },
    });

    if (hostBadge) hostBadge.classList.add("visible");
    if (guestBadge) guestBadge.classList.remove("visible");
  }

  function startSeekDetection() {
    if (seekCheckInterval) clearInterval(seekCheckInterval);
    seekCheckInterval = setInterval(function () {
      if (!ytPlayer || !isHost) return;
      try {
        var currentTime = ytPlayer.getCurrentTime();
        var playerState = ytPlayer.getPlayerState();
        // Only expect time drift when actually playing — a paused video stays
        // at the same position, so expected === lastKnownTime (no +1.0).
        var expected = playerState === YT.PlayerState.PLAYING
          ? lastKnownTime + 1.0
          : lastKnownTime;
        if (Math.abs(currentTime - expected) > 1.5) {
          // Host seeked
          connection.invoke("SendPlaybackAction", partyKey, "seek", currentTime).catch(logError);
        }
        lastKnownTime = currentTime;
      } catch (e) {
        // Player not ready
      }
    }, 1000);
  }

  function startHeartbeat() {
    if (heartbeatInterval) clearInterval(heartbeatInterval);
    heartbeatInterval = setInterval(function () {
      if (!ytPlayer || !isHost) return;
      try {
        var t = ytPlayer.getCurrentTime();
        connection.invoke("Heartbeat", partyKey, t).catch(logError);
      } catch (e) {
        // Player not ready
      }
    }, 5000);
  }

  // ── UI helpers ──

  function hideAllLobbyPanels() {
    if (roleSelect) roleSelect.style.display = "none";
    if (joinCodeEntry) joinCodeEntry.style.display = "none";
    if (hostLobby) hostLobby.style.display = "none";
    if (guestLobby) guestLobby.style.display = "none";
    if (roleError) roleError.style.display = "none";
    if (joinCodeError) joinCodeError.style.display = "none";
  }

  function showRoleSelect() {
    if (lobbyEl) lobbyEl.style.display = "flex";
    hideAllLobbyPanels();
    if (roleSelect) roleSelect.style.display = "block";
    setStatus("Connected", "connected");
  }

  function showJoinCodeEntry() {
    if (lobbyEl) lobbyEl.style.display = "flex";
    hideAllLobbyPanels();
    if (joinCodeEntry) joinCodeEntry.style.display = "block";
    if (joinCodeInput) joinCodeInput.focus();
  }

  function showLobby(data) {
    if (lobbyEl) lobbyEl.style.display = "flex";
    hideAllLobbyPanels();

    if (isHost) {
      // Host lobby: show code + participants + Start Party
      if (hostLobby) hostLobby.style.display = "block";
      if (joinCodeDisplay && data.joinCode) {
        joinCodeDisplay.textContent = data.joinCode;
      }
    } else {
      // Guest lobby: waiting message + participant list
      if (guestLobby) guestLobby.style.display = "block";
    }

    // Populate initial participant list
    if (data.participants) {
      updateParticipantList(data.participants);
    }

    setStatus("In lobby", "connected");
  }

  function hideLobby() {
    if (lobbyEl) lobbyEl.style.display = "none";
    hideAllLobbyPanels();
  }

  function updateParticipantList(names) {
    var lists = [participantList, guestParticipantList];
    for (var i = 0; i < lists.length; i++) {
      var ul = lists[i];
      if (!ul) continue;
      ul.innerHTML = "";
      for (var j = 0; j < names.length; j++) {
        var li = document.createElement("li");
        li.textContent = names[j];
        ul.appendChild(li);
      }
    }
    // Update viewer count chip
    var viewerCount = document.getElementById("wpViewerCount");
    if (viewerCount) viewerCount.textContent = names.length;
  }

  function initializePlayer(data) {
    setStatus("Connected to party!", "connected");

    loadYouTubeAPI(function () {
      createPlayer(data.youtubeVideoId || youtubeVideoId, function () {
        if (isHost) {
          enableHostControls();
        } else {
          if (guestBadge) guestBadge.classList.add("visible");
          // Seek to current position
          suppressEvents = true;
          ytPlayer.seekTo(data.position || 0, true);
          if (data.isPlaying) {
            ytPlayer.playVideo();
          } else {
            ytPlayer.pauseVideo();
          }
          setTimeout(function () { suppressEvents = false; }, 500);
        }
      });
    });
  }

  // ── Chat helpers ──
  function appendChat(name, message, time) {
    if (!chatMessages) return;
    var div = document.createElement("div");
    div.className = "wp-msg";
    var nameEl = document.createElement("div");
    nameEl.className = "wp-msg-name";
    nameEl.textContent = name;
    var textEl = document.createElement("div");
    textEl.className = "wp-msg-text";
    textEl.textContent = message;
    var timeEl = document.createElement("div");
    timeEl.className = "wp-msg-time";
    if (time) {
      var d = new Date(time);
      timeEl.textContent = d.toLocaleTimeString([], { hour: "2-digit", minute: "2-digit" });
    }
    div.appendChild(nameEl);
    div.appendChild(textEl);
    div.appendChild(timeEl);
    chatMessages.appendChild(div);
    chatMessages.scrollTop = chatMessages.scrollHeight;
  }

  function appendSystemMsg(text) {
    if (!chatMessages) return;
    var div = document.createElement("div");
    div.className = "wp-msg-system";
    div.textContent = text;
    chatMessages.appendChild(div);
    chatMessages.scrollTop = chatMessages.scrollHeight;
  }

  function logError(err) {
    if (err) console.warn("Watch Party error:", err);
  }

  function handleConnectionError(err) {
    logError(err);
    var errStr = err ? err.toString() : "";
    if (errStr.indexOf("access-denied") !== -1) {
      setStatus("Access denied — you need a valid pass or ticket.", "error");
      appendSystemMsg("\u26A0\uFE0F Access denied. Please purchase a pass or ticket to join this watch party.");
      setTimeout(function () {
        window.location.href = "/festivals/" + festivalId + "?toast=access-denied";
      }, 3000);
    } else {
      setStatus("Failed to connect", "error");
    }
  }

  // ── Join code submission ──
  function submitJoinCode() {
    if (!joinCodeInput) return;
    var code = joinCodeInput.value.trim();
    if (!code) return;
    lastTriedCode = code;
    // Hide previous error
    if (joinCodeError) joinCodeError.style.display = "none";
    connection
      .invoke("JoinPartyWithCode", festivalId, festivalFilmId, sessionId, code)
      .catch(handleConnectionError);
  }

  // ── Build reaction bar (8-emoji curated row, sends directly to chat) ──
  if (reactionBar) {
    REACTIONS.forEach(function (em) {
      var btn = document.createElement("button");
      btn.type = "button";
      btn.className = "wp-reaction-btn";
      btn.textContent = em;
      btn.title = em;
      btn.addEventListener("click", function () {
        // Send emoji directly as a chat message (instant, no extra step)
        if (partyKey) {
          connection.invoke("SendChatMessage", partyKey, em).catch(logError);
        }
      });
      reactionBar.appendChild(btn);
    });
  }

  // ── SignalR connection ──
  var connection = new signalR.HubConnectionBuilder()
    .withUrl("/watchPartyHub", { withCredentials: true })
    .withAutomaticReconnect([0, 2000, 5000, 10000])
    .build();

  // ── Handle server → client messages ──

  connection.on("JoinedParty", function (data) {
    hasJoinedParty = true;
    isHost = data.isHost;
    partyKey = data.partyKey || "";
    partyPhase = data.phase || "Live";
    youtubeVideoId = data.youtubeVideoId || youtubeVideoId;

    // Store the join code for reconnect
    if (data.joinCode) {
      myJoinCode = data.joinCode;
    } else if (!data.isHost && lastTriedCode) {
      // Guest stores the code they used to join (for reconnect)
      myJoinCode = lastTriedCode;
    }

    // Single consolidated system message (Issue #4: dedup)
    if (isHost) {
      appendSystemMsg("\uD83C\uDFAC Welcome! You're the host \u2014 share the join code to invite others, then hit Start Party. Your playback controls everyone's player.");
    } else {
      appendSystemMsg("Welcome! You've joined the party \u2014 sit back and enjoy. The host controls playback.");
    }

    if (partyPhase === "Lobby") {
      showLobby(data);
    } else {
      // Live \u2014 show player immediately (late joiner or post-StartParty)
      hideLobby();
      initializePlayer(data);
    }
  });

  connection.on("PartyAlreadyExists", function () {
    showRoleSelect();
    if (roleError) {
      roleError.innerHTML = 'A party already exists for this film. <a href="#" class="wp-role-error-link" id="wpSwitchToJoin">Join with a code instead.</a>';
      roleError.style.display = "block";
      var switchLink = document.getElementById("wpSwitchToJoin");
      if (switchLink) {
        switchLink.addEventListener("click", function (e) {
          e.preventDefault();
          if (roleError) roleError.style.display = "none";
          showJoinCodeEntry();
        });
      }
    }
    setStatus("Party already exists", "error");
  });

  connection.on("InvalidJoinCode", function () {
    // Show join code entry (not role-select) with error + pre-fill
    showJoinCodeEntry();
    if (joinCodeInput && lastTriedCode) {
      joinCodeInput.value = lastTriedCode;
    }
    if (joinCodeError) {
      joinCodeError.textContent = "Invalid or expired code \u2014 please check and try again.";
      joinCodeError.style.display = "block";
    }
    setStatus("Invalid join code", "error");
  });

  connection.on("LobbyUpdate", function (participants) {
    updateParticipantList(participants);
  });

  connection.on("PartyStarted", function () {
    partyPhase = "Live";
    hideLobby();
    appendSystemMsg("\u{1F389} The party has started!");
    // Initialize the YouTube player — playback starts paused at 0, host presses play
    initializePlayer({
      isHost: isHost,
      isPlaying: false,
      position: 0,
      youtubeVideoId: youtubeVideoId
    });
  });

  connection.on("ReceivePlaybackAction", function (action, positionSeconds) {
    if (isHost) return; // Host doesn't listen to its own broadcasts
    if (!ytPlayer) return;

    suppressEvents = true;
    try {
      if (action === "seek") {
        ytPlayer.seekTo(positionSeconds, true);
      } else if (action === "play") {
        ytPlayer.seekTo(positionSeconds, true);
        ytPlayer.playVideo();
      } else if (action === "pause") {
        ytPlayer.seekTo(positionSeconds, true);
        ytPlayer.pauseVideo();
      }
    } catch (e) {
      // Player not ready
    }
    setTimeout(function () { suppressEvents = false; }, 500);
  });

  connection.on("ReceiveChatMessage", function (senderName, message, timestamp) {
    appendChat(senderName, message, timestamp);
  });

  connection.on("YouAreNowHost", function (data) {
    isHost = true;
    // Receive join code from hub so promoted host can display it
    if (data && data.joinCode) {
      myJoinCode = data.joinCode;
      if (joinCodeDisplay) joinCodeDisplay.textContent = data.joinCode;
    }
    if (partyPhase === "Lobby") {
      // Show host lobby controls (Start Party button) — inherit the lobby
      if (hostLobby) hostLobby.style.display = "block";
      if (guestLobby) guestLobby.style.display = "none";
      appendSystemMsg("\u{1F3AC} You are now the host! You can start the party when ready.");
    } else {
      // Live — existing behavior
      appendSystemMsg("\u{1F3AC} You are now the host! Your playback controls everyone.");
      enableHostControls();
    }
  });

  connection.on("HostChanged", function () {
    if (!isHost) {
      appendSystemMsg("The host has changed.");
    }
  });

  // ── Reconnect handling ──
  connection.onreconnecting(function () {
    setStatus("Reconnecting\u2026", "connecting");
  });

  connection.onreconnected(function () {
    setStatus("Reconnected!", "connected");
    // Re-join with the appropriate method
    if (isHost && partyKey) {
      // Host reconnects via CreateParty (resume path)
      connection
        .invoke("CreateParty", festivalId, festivalFilmId, sessionId, youtubeVideoId)
        .catch(handleConnectionError);
    } else if (myJoinCode) {
      // Guest reconnects with stored code
      connection
        .invoke("JoinPartyWithCode", festivalId, festivalFilmId, sessionId, myJoinCode)
        .catch(handleConnectionError);
    } else {
      // No stored state — show role selection
      showRoleSelect();
    }
  });

  connection.onclose(function () {
    setStatus("Disconnected", "error");
  });

  // ── Chat send ──
  function sendChat() {
    if (!chatInput || !partyKey) return;
    var msg = chatInput.value.trim();
    if (!msg) return;
    connection.invoke("SendChatMessage", partyKey, msg).catch(logError);
    chatInput.value = "";
    chatInput.focus();
  }

  if (chatSend) {
    chatSend.addEventListener("click", sendChat);
  }
  if (chatInput) {
    chatInput.addEventListener("keydown", function (e) {
      if (e.key === "Enter") {
        e.preventDefault();
        sendChat();
      }
    });
  }

  // ── Role-select event listeners ──
  if (btnHost) {
    btnHost.addEventListener("click", function () {
      if (roleError) roleError.style.display = "none";
      connection
        .invoke("CreateParty", festivalId, festivalFilmId, sessionId, youtubeVideoId)
        .catch(handleConnectionError);
    });
  }

  if (btnJoin) {
    btnJoin.addEventListener("click", function () {
      showJoinCodeEntry();
    });
  }

  // ── Join code entry event listeners ──
  if (joinCodeSubmit) {
    joinCodeSubmit.addEventListener("click", submitJoinCode);
  }
  if (joinCodeInput) {
    joinCodeInput.addEventListener("keydown", function (e) {
      if (e.key === "Enter") {
        e.preventDefault();
        submitJoinCode();
      }
    });
  }

  // ── Host lobby event listeners ──
  if (copyCodeBtn) {
    copyCodeBtn.addEventListener("click", function () {
      var code = joinCodeDisplay ? joinCodeDisplay.textContent : "";
      if (code && navigator.clipboard) {
        navigator.clipboard.writeText(code).then(function () {
          copyCodeBtn.innerHTML = '<i class="bi bi-check-lg"></i>';
          setTimeout(function () {
            copyCodeBtn.innerHTML = '<i class="bi bi-clipboard"></i>';
          }, 2000);
        });
      }
    });
  }

  if (startPartyBtn) {
    startPartyBtn.addEventListener("click", function () {
      if (!partyKey) return;
      startPartyBtn.disabled = true;
      startPartyBtn.textContent = "Starting\u2026";
      connection.invoke("StartParty", partyKey).catch(function (err) {
        logError(err);
        startPartyBtn.disabled = false;
        startPartyBtn.textContent = "Start Party \u{1F680}";
      });
    });
  }

  // ── Start connection ──
  setStatus("Connecting\u2026", "connecting");

  connection
    .start()
    .then(function () {
      if (joinCodeFromUrl) {
        // Auto-join via share link — try directly with the URL code
        lastTriedCode = joinCodeFromUrl;
        connection
          .invoke("JoinPartyWithCode", festivalId, festivalFilmId, sessionId, joinCodeFromUrl)
          .catch(handleConnectionError);
      } else {
        // Show role selection screen — no auto-join
        showRoleSelect();
      }
    })
    .catch(function (err) {
      logError(err);
      setStatus("Failed to connect", "error");
      appendSystemMsg("\u26A0\uFE0F Could not connect to the watch party. Please try again.");
    });
})();
