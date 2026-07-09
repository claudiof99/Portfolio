using System;
using System.Collections.Generic;

namespace UmaFestHub.Web.ViewModels;

public class WatchViewModel
{
    
    public string FilmName { get; set; } = string.Empty;
    public Guid FilmId { get; set; }
    public string? TrailerEmbedUrl { get; set; }
    public string SessionType { get; set; } = string.Empty;
    public DateTime SessionStartUtc { get; set; }
    public Guid FestivalId { get; set; }
    public Guid FestivalFilmId { get; set; }
    public Guid? SessionId { get; set; }

    // ── Cinematic mode extras ──
    public string? FilmDescription { get; set; }
    public string? PosterUrl { get; set; }
    public int DurationMinutes { get; set; }
    public IReadOnlyList<string> Genres { get; set; } = [];
    public string FestivalName { get; set; } = string.Empty;
    public double? Rating { get; set; }
}