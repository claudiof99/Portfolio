using UmaFestHub.Domain.Enums;

namespace UmaFestHub.Application.DTOs;

public sealed record UserDto(
	Guid Id,
	string Name,
	string Email,
	IReadOnlyList<string> Roles);
