// -----------------------------------------------------------------------------
// Awards, nominations & votes — registers IAwardService, NominationCandidatesService,
// and all INominationValidator implementations (film + role-based credits).
// Review replies: ReviewService registered as IReviewService + IReviewReplyService (same instance).
// In-app notifications: Awards/Reviews/FestivalEnding/RentalExpiry/PurchaseCompleted observer folders under Observers; festival + rental reminders use I*ReminderService + BackgroundService in Web.
// -----------------------------------------------------------------------------
using Microsoft.Extensions.DependencyInjection;
using UmaFestHub.Application.Factories;
using UmaFestHub.Application.Interfaces;
using UmaFestHub.Application.Services;
using UmaFestHub.Application.Validation;
using UmaFestHub.Application.Services.Validators;
using UmaFestHub.Application.Strategies;
using UmaFestHub.Application.Security;
using UmaFestHub.Application.Validators;
using UmaFestHub.Application.Validators.Purchase;
using UmaFestHub.Application.Handlers;
using UmaFestHub.Application.Observers.Awards;
using UmaFestHub.Application.Observers.FestivalEnding;
using UmaFestHub.Application.Observers.FilmsWatched;
using UmaFestHub.Application.Observers.PurchaseCompleted;
using UmaFestHub.Application.Observers.Reviews;
using UmaFestHub.Application.Observers.RentalExpiry;
using UmaFestHub.Application.Pricing;
using UmaFestHub.Application.Recommendations;

namespace UmaFestHub.Application.Extensions;

public static class DependencyInjectionExtensions
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddScoped<IAdminService, AdminService>();
        services.AddScoped<ICartService, CartService>();
        services.AddScoped<IFestivalService, FestivalService>();
        services.AddScoped<IFilmService, FilmService>();
        services.AddScoped<IFestivalFilmService, FestivalFilmService>();
        services.AddScoped<IPurchaseService, PurchaseService>();
		// Single scoped instance implements both review and reply ports (ISP split, same lifetime).
		services.AddScoped<ReviewService>();
		services.AddScoped<IReviewService>(sp => sp.GetRequiredService<ReviewService>());
		services.AddScoped<IReviewReplyService>(sp => sp.GetRequiredService<ReviewService>());
        services.AddScoped<IUserService, UserService>();
        services.AddScoped<ISessionService, SessionService>();
        services.AddScoped<ICustomerService, CustomerService>();
        services.AddScoped<IManagerService, ManagerService>();
        services.AddScoped<IUserRoleService, UserRoleService>();
        services.AddScoped<IVoteService, VoteService>();
        services.AddScoped<IAwardService, AwardService>();
		services.AddScoped<INominationCandidatesService, NominationCandidatesService>();
		// User watchlist / favorites / watched (Seen) list operations
		services.AddScoped<IPersonalListService, PersonalListService>();
		// Seen list + future “on film watched” hooks (Observer): IFilmWatchedNotifier fans out to all IFilmWatchedObserver implementations.
		services.AddScoped<IFilmWatchedObserver, FilmsWatchedListObserver>();
		services.AddScoped<IFilmWatchedNotifier, FilmWatchedNotifier>();
		// In-app notifications (Observer): enqueue + SignalR wired via INotificationService / IPendingNotificationRepository in Web+Infrastructure.
		services.AddScoped<IReviewNotificationObserver, ReviewNotificationObserver>();
		services.AddScoped<IReviewNotificationNotifier, ReviewNotificationNotifier>();
		services.AddScoped<IAwardNotificationObserver, AwardNotificationObserver>();
		services.AddScoped<IAwardNotificationNotifier, AwardNotificationNotifier>();
		// Festival end ≤3d: notifier fans out observers; reminder service is invoked only from FestivalEndingReminderWorker (Web).
		services.AddScoped<IFestivalEndingNotificationObserver, FestivalEndingNotificationObserver>();
		services.AddScoped<IFestivalEndingNotificationNotifier, FestivalEndingNotificationNotifier>();
		services.AddScoped<IFestivalEndingReminderService, FestivalEndingReminderService>();
		services.AddScoped<IRentalExpiryReminderService, RentalExpiryReminderService>();
        services.AddScoped<IProductService, ProductService>();
        services.AddScoped<IUserRoleStrategy, AdminStrategy>();
        services.AddScoped<IUserRoleStrategy, OrganizerStrategy>();
        services.AddScoped<IUserRoleStrategy, CustomerStrategy>();
        services.AddScoped<SessionExistsHandler>();
        services.AddScoped<SessionTimeGateHandler>();
        services.AddScoped<AccessWindowAccessHandler>();
        services.AddScoped<CompletePassAccessHandler>();
        services.AddScoped<DailyPassAccessHandler>();
        services.AddScoped<UserHasAccessHandler>();
        services.AddScoped<ISessionAccessService, SessionAccessService>();
     
        services.AddScoped<ProductStore, CompletePassStore>();
        services.AddScoped<ProductStore, DailyPassStore>();  
        services.AddScoped<ProductStore, TicketStore>();       
        services.AddScoped<ProductStore, RentalStore>();
        services.AddScoped<SessionStore, FixedSessionStore>();
        services.AddScoped<SessionStore, PremierSessionStore>();
        services.AddScoped<SessionStore, AccessWindowSessionStore>();
		services.AddScoped<INominationValidator, FilmValidator>();
		services.AddScoped<INominationValidator, ActorValidator>();
		services.AddScoped<INominationValidator, DirectorValidator>();
		services.AddScoped<INominationValidator, WritingValidator>();

        services.AddScoped<IEntitlementService, EntitlementService>();
        services.AddScoped<IEntitlementStrategy, CompletePassEntitlementStrategy>();
        services.AddScoped<IEntitlementStrategy, DailyPassEntitlementStrategy>();
        services.AddScoped<IEntitlementStrategy, RentalEntitlementStrategy>();
        services.AddScoped<IEntitlementStrategy, TicketEntitlementStrategy>();

		// Product Factory for OCP-compliant entity creation
		services.AddSingleton<IProductFactory, ProductFactory>();

		// Cart validation pipeline (Chain of Responsibility)
		services.AddScoped<ICartValidationPipeline, CartValidationPipeline>();
		services.AddScoped<ICartValidator, DuplicateItemValidator>();
		services.AddScoped<ICartValidator, ProductExistenceValidator>();

		// Pass delete strategies (Strategy pattern)
		services.AddScoped<IPassDeleteStrategy, DailyPassDeleteStrategy>();
		services.AddScoped<IPassDeleteStrategy, CompletePassDeleteStrategy>();
		services.AddScoped<IPassDeleteHandler, PassDeleteHandler>();

		// Purchase validation pipeline (OCP-compliant validators)
		services.AddScoped<PurchaseValidationPipeline>();
		services.AddScoped<IPurchaseValidator, FestivalActiveRule>();
		services.AddScoped<IPurchaseValidator, DailyPassValidator>();
		services.AddScoped<IPurchaseValidator, CompletePassValidator>();
		services.AddScoped<IPurchaseValidator, RentalValidator>();
		services.AddScoped<IPurchaseValidator, TicketValidator>();

		// Pricing strategy chain (Decorator pattern — layers discount decorators over base price)
		services.AddScoped<IPricingStrategy>(_ =>
			new EarlyBirdPricingDecorator(
				new BasePricingStrategy()));
		services.AddScoped<IPricingService, PricingService>();

		// Purchase completed observer (Observer pattern — service-layer trigger, not controller)
		services.AddScoped<IPurchaseCompletedObserver, PurchaseHistoryObserver>();
		services.AddScoped<IPurchaseCompletedNotifier, PurchaseCompletedNotifier>();

		// Rental expiry ≤3d: notifier fans out observers; reminder service is invoked from RentalExpiryReminderWorker (Web).
		services.AddScoped<IRentalExpiryObserver, RentalExpiryWarningObserver>();
		services.AddScoped<IRentalExpiryNotifier, RentalExpiryNotifier>();

		// Recommendation engine — composite scoring across review quality, social proof, nominations, genre affinity, and TMDb popularity
		services.AddScoped<IRecommendationService, RecommendationService>();

        return services;
    }
}
