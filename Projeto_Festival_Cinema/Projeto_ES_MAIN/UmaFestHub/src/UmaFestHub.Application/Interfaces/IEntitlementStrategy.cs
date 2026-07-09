using System;
using System.Threading;
using System.Threading.Tasks;

namespace UmaFestHub.Application.Interfaces;

public interface IEntitlementStrategy
{
	Type ProductDomainType { get; }
	Task<bool> GrantsAccessAsync(
		Guid productId,
		DateTime purchaseDateUtc,
		Guid festivalId,
		Guid festivalFilmId,
		Guid? sessionId,
		CancellationToken cancellationToken = default);
}