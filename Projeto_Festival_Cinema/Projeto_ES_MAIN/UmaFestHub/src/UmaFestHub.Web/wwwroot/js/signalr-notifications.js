/**
 * In-app notifications: after login, drain GET /notifications/pending (modal), then connect to /notificationHub
 * and show "ShowNotification" payloads; OK/modal close POSTs /notifications/ack (anti-forgery).
 */
(function () {
  var body = document.body;
  if (!body || body.dataset.signalrNotifications !== "true") {
    return;
  }
  if (!window.signalR) {
    return;
  }

  var modal = document.getElementById("realtimeNotificationModal");
  var titleEl = document.getElementById("realtimeNotificationTitle");
  var messageEl = document.getElementById("realtimeNotificationMessage");
  var okBtn = document.getElementById("realtimeNotificationOkBtn");
  var csrf = body.dataset.notificationCsrf || "";

  var COLLAPSE_REVIEW_PENDING = "review-pending";
  var COLLAPSE_REPLY_PENDING = "reply-pending";
  var AUTHOR_COMMENT_MAX = 600;

  function getI18n() {
    return window.__notificationI18n || {};
  }

  function t(key, args) {
    var i18n = getI18n();
    var template = i18n[key];
    if (template == null || template === key) {
      return template != null ? template : key;
    }
    if (!args || args.length === 0) {
      return template;
    }
    return template.replace(/\{(\d+)\}/g, function (_, index) {
      var i = parseInt(index, 10);
      return args[i] != null ? String(args[i]) : "";
    });
  }

  /** Resx key lookup with fallback to the raw label (matches server LocalizeDisplayText). */
  function localizeDisplayText(text) {
    if (!text) {
      return "";
    }
    var i18n = getI18n();
    if (Object.prototype.hasOwnProperty.call(i18n, text)) {
      return i18n[text];
    }
    return text;
  }

  function templateKind(template) {
    if (!template) {
      return "";
    }
    return template.kind || template.Kind || "";
  }

  function formatCurrency(amount) {
    var lang = document.documentElement.lang || "en";
    try {
      return new Intl.NumberFormat(lang, {
        style: "currency",
        currency: "USD",
      }).format(amount);
    } catch (e) {
      return "$" + Number(amount).toFixed(2);
    }
  }

  function trimForModal(text, maxChars) {
    var s = (text || "").trim();
    if (s.length <= maxChars) {
      return s;
    }
    return s.slice(0, maxChars).trimEnd() + "…";
  }

  function describeLessThanDaysRemaining(endDateUtc) {
    var end = new Date(endDateUtc);
    var now = new Date();
    var remainingMs = end.getTime() - now.getTime();
    if (remainingMs <= 0) {
      return t("Notification_EndingVerySoon");
    }
    var ceilDays = Math.min(3, Math.max(1, Math.ceil(remainingMs / 86400000)));
    return ceilDays === 1
      ? t("Notification_LessThan1Day")
      : t("Notification_LessThanDays", [ceilDays]);
  }

  function describeTimeRemaining(expiresAtUtc) {
    var end = new Date(expiresAtUtc);
    var now = new Date();
    var remainingMs = end.getTime() - now.getTime();
    if (remainingMs <= 0) {
      return t("Notification_EndingVerySoon");
    }
    if (remainingMs < 86400000) {
      var hours = Math.max(1, Math.ceil(remainingMs / 3600000));
      return hours === 1
        ? t("Notification_LessThan1Hour")
        : t("Notification_LessThanHours", [hours]);
    }
    var ceilDays = Math.min(3, Math.max(1, Math.ceil(remainingMs / 86400000)));
    return ceilDays === 1
      ? t("Notification_LessThan1Day")
      : t("Notification_LessThanDays", [ceilDays]);
  }

  function formatDateYmd(iso) {
    var d = new Date(iso);
    var y = d.getUTCFullYear();
    var m = String(d.getUTCMonth() + 1).padStart(2, "0");
    var day = String(d.getUTCDate()).padStart(2, "0");
    return y + "-" + m + "-" + day;
  }

  function formatDateYmdHm(iso) {
    var d = new Date(iso);
    var y = d.getUTCFullYear();
    var m = String(d.getUTCMonth() + 1).padStart(2, "0");
    var day = String(d.getUTCDate()).padStart(2, "0");
    var h = String(d.getUTCHours()).padStart(2, "0");
    var min = String(d.getUTCMinutes()).padStart(2, "0");
    return y + "-" + m + "-" + day + " " + h + ":" + min;
  }

  function renderTemplate(template) {
    var kind = templateKind(template);
    if (!kind) {
      return { title: "", message: "" };
    }
    switch (kind) {
      case "review-pending":
        return {
          title: t("Notification_ReviewPendingTitle"),
          message: t("Notification_ReviewPendingMessage"),
        };
      case "reply-pending":
        return {
          title: t("Notification_ReplyPendingTitle"),
          message: t("Notification_ReplyPendingMessage"),
        };
      case "review-outcome": {
        var approved =
          template.isApproved === true || template.IsApproved === true;
        var title = approved
          ? t("Notification_ReviewApprovedTitle")
          : t("Notification_ReviewRejectedTitle");
        var outcome = approved
          ? t("Review_StatusApproved")
          : t("Review_StatusRejected");
        var comment = trimForModal(
          template.comment || template.Comment || "",
          AUTHOR_COMMENT_MAX,
        );
        var festivalName = template.festivalName || template.FestivalName || "";
        var filmTitle = template.filmTitle || template.FilmTitle || "";
        var rating =
          template.rating != null
            ? template.rating
            : template.Rating != null
              ? template.Rating
              : 0;
        var lines = [
          t("Notification_ReviewOutcomeResponse", [outcome]),
          t("Notification_ReviewOutcomeFestival", [festivalName]),
          t("Notification_ReviewOutcomeFilm", [filmTitle]),
          t("Notification_ReviewOutcomeRating", [rating]),
          "",
          t("Notification_ReviewOutcomeYourReview"),
          comment,
        ];
        return { title: title, message: lines.join("\n") };
      }
      case "reply-outcome": {
        var replyApproved =
          template.isApproved === true || template.IsApproved === true;
        var replyTitle = replyApproved
          ? t("Notification_ReplyApprovedTitle")
          : t("Notification_ReplyRejectedTitle");
        var replyOutcome = replyApproved
          ? t("Review_StatusApproved")
          : t("Review_StatusRejected");
        var replyComment = trimForModal(
          template.comment || template.Comment || "",
          AUTHOR_COMMENT_MAX,
        );
        var replyFestivalName =
          template.festivalName || template.FestivalName || "";
        var replyFilmTitle = template.filmTitle || template.FilmTitle || "";
        var replyLines = [
          t("Notification_ReviewOutcomeResponse", [replyOutcome]),
          t("Notification_ReviewOutcomeFestival", [replyFestivalName]),
          t("Notification_ReviewOutcomeFilm", [replyFilmTitle]),
          "",
          t("Notification_ReplyOutcomeYourReply"),
          replyComment,
        ];
        return { title: replyTitle, message: replyLines.join("\n") };
      }
      case "award-results": {
        var awardName = template.awardName || template.AwardName || "";
        var awardTitle = t("Notification_AwardVotingClosedTitle", [awardName]);
        var bodyLines = [t("Notification_AwardFinalResults")];
        var results = template.results || template.Results || [];
        for (var i = 0; i < results.length; i++) {
          var line = results[i];
          bodyLines.push(
            t("Notification_AwardResultLine", [
              localizeDisplayText(line.label || line.Label || ""),
              line.percent != null
                ? line.percent
                : line.Percent != null
                  ? line.Percent
                  : 0,
            ]),
          );
        }
        return { title: awardTitle, message: bodyLines.join("\n").trim() };
      }
      case "festival-ending": {
        var useDefaultFest =
          template.useDefaultFestivalName === true ||
          template.UseDefaultFestivalName === true;
        var rawFestName = template.festivalName || template.FestivalName || "";
        var festName = useDefaultFest || !rawFestName
          ? t("Notification_FestivalEndingDefaultName")
          : String(rawFestName).trim();
        var endDate =
          template.endDateUtc || template.EndDateUtc || new Date().toISOString();
        return {
          title: t("Notification_FestivalEndingTitle"),
          message: t("Notification_FestivalEndingMessage", [
            festName,
            formatDateYmd(endDate),
            describeLessThanDaysRemaining(endDate),
          ]),
        };
      }
      case "rental-expiring": {
        var useDefaultFilm =
          template.useDefaultFilmTitle === true ||
          template.UseDefaultFilmTitle === true;
        var rawFilm = template.filmTitle || template.FilmTitle || "";
        var film = useDefaultFilm || !rawFilm
          ? t("Notification_RentalDefaultFilm")
          : String(rawFilm).trim();
        var expires =
          template.expiresAtUtc ||
          template.ExpiresAtUtc ||
          new Date().toISOString();
        return {
          title: t("Notification_RentalExpiringTitle"),
          message: t("Notification_RentalExpiringMessage", [
            film,
            formatDateYmdHm(expires),
            describeTimeRemaining(expires),
          ]),
        };
      }
      case "purchase-completed":
        return {
          title: t("Notification_PurchaseCompletedTitle"),
          message: t("Notification_PurchaseCompletedMessage", [
            formatCurrency(
              template.totalAmount != null
                ? template.totalAmount
                : template.TotalAmount != null
                  ? template.TotalAmount
                  : 0,
            ),
          ]),
        };
      default:
        return { title: "", message: "" };
    }
  }

  function parseTemplateJson(raw) {
    if (!raw) {
      return null;
    }
    if (typeof raw === "object") {
      return raw;
    }
    try {
      var parsed = JSON.parse(raw);
      if (typeof parsed === "string") {
        try {
          return JSON.parse(parsed);
        } catch (inner) {
          return null;
        }
      }
      return parsed;
    } catch (e) {
      return null;
    }
  }

  function resolvePayload(p) {
    if (!p) {
      return { title: "", message: "", id: null, collapseGroup: null };
    }
    var id = p.id != null ? p.id : p.Id != null ? p.Id : null;
    var collapseGroup =
      p.collapseGroup != null
        ? p.collapseGroup
        : p.CollapseGroup != null
          ? p.CollapseGroup
          : null;

    var templateJson =
      p.templateJson != null
        ? p.templateJson
        : p.TemplateJson != null
          ? p.TemplateJson
          : null;
    var template = parseTemplateJson(templateJson);
    if (template) {
      var rendered = renderTemplate(template);
      return {
        title: rendered.title,
        message: rendered.message,
        id: id,
        collapseGroup:
          collapseGroup != null
            ? collapseGroup
            : templateKind(template) === "review-pending"
              ? COLLAPSE_REVIEW_PENDING
              : templateKind(template) === "reply-pending"
                ? COLLAPSE_REPLY_PENDING
                : null,
      };
    }

    var title = p.title != null ? p.title : p.Title || "";
    var message = p.message != null ? p.message : p.Message || "";
    return { title: title, message: message, id: id, collapseGroup: collapseGroup };
  }

  function getRawItemId(raw) {
    if (!raw) {
      return null;
    }
    return raw.id != null ? raw.id : raw.Id;
  }

  /** Collapse identical staff “pending review/reply” alerts into one modal + batch ack. */
  function buildCollapsedBacklogQueue(items) {
    var reviewPending = [];
    var replyPending = [];
    var rest = [];
    for (var i = 0; i < items.length; i++) {
      var raw = items[i];
      var resolved = resolvePayload(raw);
      if (resolved.collapseGroup === COLLAPSE_REVIEW_PENDING) {
        reviewPending.push(raw);
      } else if (resolved.collapseGroup === COLLAPSE_REPLY_PENDING) {
        replyPending.push(raw);
      } else {
        rest.push(raw);
      }
    }
    var queue = [];
    if (reviewPending.length > 0) {
      var primaryReview = reviewPending[0];
      var extraReviewAckIds = [];
      for (var j = 1; j < reviewPending.length; j++) {
        var reviewEid = getRawItemId(reviewPending[j]);
        if (reviewEid && extraReviewAckIds.indexOf(reviewEid) === -1) {
          extraReviewAckIds.push(reviewEid);
        }
      }
      queue.push({ payload: primaryReview, extraAckIds: extraReviewAckIds });
    }
    if (replyPending.length > 0) {
      var primaryReply = replyPending[0];
      var extraReplyAckIds = [];
      for (var k = 1; k < replyPending.length; k++) {
        var replyEid = getRawItemId(replyPending[k]);
        if (replyEid && extraReplyAckIds.indexOf(replyEid) === -1) {
          extraReplyAckIds.push(replyEid);
        }
      }
      queue.push({ payload: primaryReply, extraAckIds: extraReplyAckIds });
    }
    for (var r = 0; r < rest.length; r++) {
      queue.push({ payload: rest[r], extraAckIds: [] });
    }
    return queue;
  }

  function showNotificationModal(payload, extraAckIds) {
    extraAckIds = extraAckIds || [];
    return new Promise(function (resolve) {
      if (!modal || !window.bootstrap) {
        resolve();
        return;
      }
      var p = resolvePayload(payload);
      var idsToAck = [];
      if (p.id) {
        idsToAck.push(p.id);
      }
      for (var i = 0; i < extraAckIds.length; i++) {
        var xid = extraAckIds[i];
        if (xid && idsToAck.indexOf(xid) === -1) {
          idsToAck.push(xid);
        }
      }
      var batchAcked = false;

      function postAckAll() {
        if (batchAcked || !csrf || idsToAck.length === 0) {
          return Promise.resolve();
        }
        batchAcked = true;
        return idsToAck.reduce(function (chain, id) {
          return chain.then(function () {
            var params = new URLSearchParams();
            params.append("id", id);
            params.append("__RequestVerificationToken", csrf);
            return fetch("/notifications/ack", {
              method: "POST",
              headers: { "Content-Type": "application/x-www-form-urlencoded" },
              body: params.toString(),
              credentials: "same-origin",
            }).catch(function () {});
          });
        }, Promise.resolve());
      }

      if (titleEl) {
        titleEl.textContent = p.title;
      }
      if (messageEl) {
        messageEl.textContent = p.message;
      }

      var instance = bootstrap.Modal.getOrCreateInstance(modal);

      function onOkClick() {
        postAckAll().finally(function () {
          instance.hide();
        });
      }

      function onHidden() {
        modal.removeEventListener("hidden.bs.modal", onHidden);
        if (okBtn) {
          okBtn.removeEventListener("click", onOkClick);
        }
        postAckAll().finally(function () {
          resolve();
        });
      }

      modal.addEventListener("hidden.bs.modal", onHidden);
      if (okBtn) {
        okBtn.addEventListener("click", onOkClick);
      }
      instance.show();
    });
  }

  function ackWithoutModal(id) {
    if (!id || !csrf) {
      return Promise.resolve();
    }
    var params = new URLSearchParams();
    params.append("id", id);
    params.append("__RequestVerificationToken", csrf);
    return fetch("/notifications/ack", {
      method: "POST",
      headers: { "Content-Type": "application/x-www-form-urlencoded" },
      body: params.toString(),
      credentials: "same-origin",
    }).catch(function () {});
  }

  var shownIds = new Set();

  function drainBacklogThenConnect() {
    return fetch("/notifications/pending", { credentials: "same-origin" })
      .then(function (r) {
        if (!r.ok) {
          return [];
        }
        return r.json();
      })
      .then(function (items) {
        if (!Array.isArray(items) || items.length === 0) {
          return Promise.resolve();
        }
        var queue = buildCollapsedBacklogQueue(items);
        return queue.reduce(function (chain, entry) {
          return chain.then(function () {
            var primaryId = getRawItemId(entry.payload);
            if (primaryId) {
              shownIds.add(String(primaryId).toLowerCase());
            }
            for (var i = 0; i < entry.extraAckIds.length; i++) {
              var eid = entry.extraAckIds[i];
              if (eid) {
                shownIds.add(String(eid).toLowerCase());
              }
            }
            return showNotificationModal(entry.payload, entry.extraAckIds);
          });
        }, Promise.resolve());
      })
      .then(function () {
        var connection = new signalR.HubConnectionBuilder()
          .withUrl("/notificationHub", { withCredentials: true })
          .withAutomaticReconnect([0, 2000, 5000, 10000])
          .build();

        connection.on("ShowNotification", function (payload) {
          var p = resolvePayload(payload);
          var sid = p.id ? String(p.id).toLowerCase() : "";
          if (sid && shownIds.has(sid)) {
            ackWithoutModal(p.id);
            return;
          }
          if (sid) {
            shownIds.add(sid);
          }
          showNotificationModal(payload);
        });

        return connection.start().catch(function (err) {
          console.warn("SignalR notifications:", err);
        });
      });
  }

  drainBacklogThenConnect();
})();
