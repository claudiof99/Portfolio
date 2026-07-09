using UmaFestHub.Domain.ValueObjects;
namespace UmaFestHub.Application.DTOs;

public sealed record RentalDto(
    Guid Id,
    string ProductType,
    decimal Price,
    Guid FestivalFilmId,
    int DurationValue,
    string DurationUnit);
