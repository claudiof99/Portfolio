// -----------------------------------------------------------------------------
// Awards, nominations & votes — registers IAwardRepository, INominationRepository, IVoteRepository.
// In-app notifications: IPendingNotificationRepository (Notifications table via EF).
// -----------------------------------------------------------------------------
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using UmaFestHub.Application.Interfaces;
using UmaFestHub.Domain.Interfaces;
using UmaFestHub.Infrastructure.Data;
using UmaFestHub.Infrastructure.ExternalServices;
using UmaFestHub.Infrastructure.Payment;
using UmaFestHub.Infrastructure.Repositories;


namespace UmaFestHub.Infrastructure.Extensions;

public static class DependencyInjectionExtensions
{
	public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
	{
		var connectionString = BuildConnectionString(configuration);
		var serverVersion = new MySqlServerVersion(new Version(8, 0, 36));

		services.AddDbContext<AppDbContext>(options =>
			options.UseMySql(connectionString, serverVersion, mySqlOptions =>
				mySqlOptions.EnableRetryOnFailure(
					maxRetryCount: 5,
					maxRetryDelay: TimeSpan.FromSeconds(5),
					errorNumbersToAdd: null)));
		services.AddMemoryCache();

		services.AddScoped<IFilmRepository, FilmRepository>();
		services.AddScoped<IFestivalRepository, FestivalRepository>();
		services.AddScoped<ICartRepository, CartRepository>();
		services.AddScoped<IFestivalFilmRepository, FestivalFilmRepository>();
		services.AddScoped<IPurchaseRepository, PurchaseRepository>();
		services.AddScoped<IReviewRepository, ReviewRepository>();
		// Review thread persistence (ReviewReplies table)
		services.AddScoped<IReviewReplyRepository, ReviewReplyRepository>();
		services.AddScoped<IUserRepository, UserRepository>();
		services.AddScoped<ISessionRepository, SessionRepository>();
	    services.AddScoped<IAwardRepository, AwardRepository>();
	    services.AddScoped<INominationRepository, NominationRepository>();
     	services.AddScoped<IVoteRepository, VoteRepository>();
		services.AddScoped<ICreditRepository, CreditRepository>();
		// PersonalLists table persistence
		services.AddScoped<IPersonalListRepository, PersonalListRepository>();
		// In-app notifications: queued rows for replay + correlation dedupe (see PendingNotificationRepository).
		services.AddScoped<IPendingNotificationRepository, PendingNotificationRepository>();
		services.AddScoped<IExternalFilmMetadataService, TmdbFilmService>();
		services.AddScoped<IPaymentSimulationService, SimulatedPaymentService>();
		services.AddScoped<IProductRepository, ProductRepository>();
		services.AddHttpClient();

		return services;
	}

	private static string BuildConnectionString(IConfiguration configuration)
	{
		var host = Environment.GetEnvironmentVariable("DB_HOST") ?? configuration["DB_HOST"] ?? "localhost";
		var port = Environment.GetEnvironmentVariable("DB_PORT") ?? configuration["DB_PORT"] ?? "3307";
		var database = Environment.GetEnvironmentVariable("DB_NAME") ?? configuration["DB_NAME"] ?? "UmaFestHub_DB";
		var user = Environment.GetEnvironmentVariable("DB_USER") ?? configuration["DB_USER"] ?? "root";
		var password = Environment.GetEnvironmentVariable("DB_PASSWORD") ?? configuration["DB_PASSWORD"] ?? "umafesthub_root";

		return $"server={host};port={port};database={database};user={user};password={password};TreatTinyAsBoolean=true;";
	}
}
