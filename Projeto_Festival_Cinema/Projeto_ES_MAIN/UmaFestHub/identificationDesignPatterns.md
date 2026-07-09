## Identification of Design Patterns (UmaFestHub)

This document lists the **GoF design patterns** and where they currently appear in the codebase.
For patterns not implemented yet, it also notes a **natural place** in this project where they could be applied (without forcing complexity).

> Note: some patterns can be present as "implicit" implementations (e.g., a factory implemented as a switch). When possible, this document points to concrete files/classes.

---

## Creational patterns

### Abstract Factory

- **Status**: Not explicitly implemented.
- **Natural fit**: Creating product "families" for checkout (Ticket/Pass/Rental + their validators/pricers/entitlements) in a consistent bundle.

### Builder

- **Status**: Present (Framework level).
- **Evidence**:
  - `src/UmaFestHub.Web/Program.cs` heavily uses the Builder Pattern via .NET's `WebApplication.CreateBuilder(args)` to construct the application incrementally before calling `.Build()`.
- **Natural fit**: Building complex DTOs/view models for admin dashboards or film import results.

### Factory Method

- **Status**:  Present.
- **Evidence**:
  - `src/UmaFestHub.Application/Factories/ProductFactory.cs` - `ProductStore` (abstract) + `TicketStore`, `DailyPassStore`, `CompletePassStore`, `RentalStore`
  - `src/UmaFestHub.Application/Factories/ProductFactoryImpl.cs` - `ProductFactory` class implementing `IProductFactory`
  - `src/UmaFestHub.Application/Services/SessionService.cs` - uses `SessionStore` factory pattern to create sessions
  - `src/UmaFestHub.Infrastructure/Data/DesignTimeDbContextFactory.cs` (EF Core design-time factory)
- **Justification**: Centralizes object creation for different concrete types (e.g., product types, session types) so callers depend on abstractions instead of scattering `new ...()` logic throughout the code.

### Prototype

- **Status**: Not explicitly implemented.
- **Natural fit**: Cloning a festival template (festival + default awards + default session templates) to create a new edition quickly.

### Singleton

- **Status**: Not implemented as a classic static Singleton (by design).
- **Evidence of "singleton-like" lifetime**:
  - ASP.NET Core DI can create singleton services, but current registrations are primarily `AddScoped(...)`.
- **Natural fit**: Configuration objects (options) managed by DI rather than static global state.

---

## Structural patterns

### Adapter

- **Status**:  Present.
- **Evidence**:
  - `src/UmaFestHub.Web/Services/ITmdbClient.cs` + `src/UmaFestHub.Web/Services/TmdbClient.cs` adapt TMDb HTTP responses into internal DTOs used by controllers.
  - `src/UmaFestHub.Infrastructure/ExternalServices/TmdbFilmService.cs` implements `IExternalFilmMetadataService` as an adapter over external film metadata retrieval (and hides caching/transport details).
- **Justification**: Isolates third-party API contracts (TMDb endpoints/JSON) from the domain and application layers, reducing coupling and making it easier to swap providers, add caching, or change transport details without affecting core business logic.

### Bridge

- **Status**: Not explicitly implemented.
- **Natural fit**: Decouple "Access type" (Ticket/Pass/Rental) from "Viewing delivery" (Trailer URL / embedded player / local file).

### Composite

- **Status**: Not explicitly implemented.
- **Natural fit**: Cart as a composite of heterogeneous purchasable items with a common interface for pricing/validation.

### Decorator

- **Status**: Present.
- **Evidence**:
  - `src/UmaFestHub.Application/Pricing/EarlyBirdPricingDecorator.cs` - wraps `BasePricingStrategy` to add early bird discount logic
  - `src/UmaFestHub.Application/Extensions/DependencyInjectionExtensions.cs` wires the decorator chain: `EarlyBirdPricingDecorator → BasePricingStrategy`
- **Justification**: Allows adding pricing behavior (early bird discount) without modifying the base pricing class. Open for extension, closed for modification.

### Facade

- **Status**:  Present (application-service façade style).
- **Evidence**:
  - Application services provide simplified entry points that orchestrate repositories and domain objects, e.g.:
    - `src/UmaFestHub.Application/Services/PurchaseService.cs`
    - `src/UmaFestHub.Application/Services/AdminService.cs`
    - `src/UmaFestHub.Application/Services/AwardService.cs`
    - `src/umaFestHub.Application/Services/ReviewService.cs` (moderation workflow: report/hide/approve)
    - `src/UmaFestHub.Application/Services/PersonalListService.cs` (watchlist/seen/favorites operations)
    - `src/UmaFestHub.Application/Services/FestivalService.cs`
    - `src/UmaFestHub.Application/Services/CartService.cs`
- **Justification**: Provides a single, cohesive entry point per use case (purchase flow, admin overview, awards) so controllers/UI do not coordinate multiple repositories and rules directly, keeping orchestration logic centralized and testable.

### Flyweight

- **Status**: Not explicitly implemented.
- **Natural fit**: Shared reference data (genres, roles) if memory pressure becomes relevant (usually unnecessary for this scale).

### Proxy

- **Status**:  Present (partial/implicit).
- **Evidence**:
  - `src/UmaFestHub.Infrastructure/ExternalServices/TmdbFilmService.cs` uses caching (`IMemoryCache`) which acts as a proxy to the real external service.
- **Justification**: Reduces repeated external calls and latency by serving frequent requests from cache, while keeping the rest of the system unaware of whether data came from the cache or the underlying external service.

---

## Behavioral patterns

### Chain of Responsibility

- **Status**:  Present.
- **Evidence**:
  - `src/UmaFestHub.Application/Validators/CartValidationPipeline.cs` - validates cart items with chain of validators
  - `src/UmaFestHub.Application/Validators/PurchaseValidationPipeline.cs` - validates purchases with chain of validators
- **Justification**: Each validator handles one concern (date validity, session existence, access window rules, duplicate constraints) and passes to next if validation passes.

### Command

- **Status**: Not explicitly implemented.
- **Natural fit**: Encapsulate actions such as "CompletePurchase", "CancelPurchase", "ReportReview" as command objects for auditability and testing.

### Interpreter

- **Status**: Not implemented.
- **Natural fit**: If you add a mini-language for recommendations or advanced filtering (usually overkill).

### Iterator

- **Status**: Not explicitly implemented (language already provides iterators).
- **Evidence**: C# `foreach`/LINQ is used across the codebase (implicit iteration).
- **Justification**: Iteration over collections is expressed via `foreach`/LINQ, which uses iterator abstractions under the hood, keeping traversal logic uniform and avoiding custom iteration APIs unless a specialized traversal is required.

### Mediator

- **Status**: Not explicitly implemented.
- **Natural fit**: Coordinating multiple subsystems in complex workflows; often replaced by application services + domain events.

### Memento

- **Status**: Not implemented.
- **Natural fit**: Undo/rollback for admin edits (festival/session management) if required.

### Observer

- **Status**: Present.
- **Evidence**:
  - `src/UmaFestHub.Application/Observers/PurchaseCompleted/` - observers that run after purchase completion:
    - `PurchaseHistoryObserver` - updates purchase history
    - `FilmsWatchedObserver` - adds films to "Watched" list
    - `NotificationObserver` - sends in-app notification
  - `src/UmaFestHub.Application/Observers/RentalExpiry/` - rental expiry notifications:
    - `RentalExpiryWarningObserver` - sends reminder when rental expiring
  - `src/UmaFestHub.Application/Observers/FestivalEnding/` - festival ending notifications:
    - `FestivalEndingReminderObserver` - reminds purchasers when festival ending
  - `src/UmaFestHub.Application/Observers/Reviews/` - review notifications:
    - `ReviewNotificationNotifier` - notifies for pending moderation, author outcomes
  - `src/UmaFestHub.Application/Observers/Awards/` - award notifications:
    - `AwardNotificationNotifier` - notifies when voting closes
- **Justification**: Decouples notification logic from core business actions. When a purchase completes or rental expires, observers automatically run without the main service knowing about them.

### State

- **Status**:  Present (domain state modeled with enums + guarded transitions).
- **Evidence**:
  - `src/UmaFestHub.Domain/Entities/Purchase.cs` uses `PurchaseStatus` with guarded transitions (e.g., cannot complete twice).
  - `src/umaFestHub.Domain/Entities/Review.cs` uses `ReviewStatus`.
  - `src/UmaFestHub.Domain/Enums/PurchaseStatus.cs`, `src/UmaFestHub.Domain/Enums/ReviewStatus.cs`.
  - Review moderation triggers state transitions in:
    - `src/UmaFestHub.Application/Services/ReviewService.cs` (`ReportAsync`, `HideReportedAsync`, `ApproveAsync`)
- **Justification**: Encodes lifecycle rules (e.g., purchase can be completed/failed only from valid states) in the domain model, preventing invalid transitions and keeping business invariants consistent regardless of UI/controller paths.

### Strategy

- **Status**:  Present.
- **Evidence**:
  - `src/umaFestHub.Application/Strategies/IEntitlementStrategy.cs` with implementations:
    - `TicketEntitlementStrategy` - ticket access rules
    - `DailyPassEntitlementStrategy` - daily pass access rules
    - `CompletePassEntitlementStrategy` - complete pass access rules
    - `RentalEntitlementStrategy` - rental access rules (48h window)
  - `src/UmaFestHub.Application/Pricing/IPricingStrategy.cs` with implementations:
    - `BasePricingStrategy` - returns base product price
    - `EarlyBirdPricingDecorator` - applies configurable early bird discount
  - `src/UmaFestHub.Application/Recommendations/RecommendationService.cs` - recommendation scoring strategy with weighted factors
- **Justification**: Allows swapping access control logic or pricing rules without changing the code that uses them. Each strategy encapsulates its specific rules.

### Template Method

- **Status**: Present.
- **Evidence**:
  - Used in validation structures (e.g., `RoleBasedCreditValidator.cs`) where a base class dictates the skeleton of the `ValidateAsync` algorithm but defers specific logic details (like role/category) to concrete subclasses.
- **Natural fit**: A base "checkout workflow" with overridable steps for each access type.

### Visitor

- **Status**: Not implemented.
- **Natural fit**: Reporting/export over a stable object structure (e.g., generate invoices, admin analytics, export festival catalog).

---

## Patterns currently used (non-GoF, but useful to mention in the course report)

These are not in the GoF list, but they are commonly graded as architectural/design patterns:

- **Repository pattern**:
  - `src/UmaFestHub.Domain/Interfaces/*Repository.cs`
  - `src/UmaFestHub.Infrastructure/Repositories/*Repository.cs`
- **Dependency Injection / IoC**:
  - `src/UmaFestHub.Web/Program.cs`
  - `src/UmaFestHub.Application/Extensions/DependencyInjectionExtensions.cs`
  - `src/UmaFestHub.Infrastructure/Extensions/DependencyInjectionExtensions.cs`
- **DTO + Mapping**:
  - `src/UmaFestHub.Application/DTOs/*`
  - `src/UmaFestHub.Web/Mappings/ViewModelMappings.cs`

### Patterns added/used in extended feature work (Reviews, Awards, Personal Lists, Pricing, Early Bird)

- **Repository pattern (reinforced/extended)**:
  - `src/UmaFestHub.Domain/Interfaces/IVoteRepository.cs` + `src/UmaFestHub.Infrastructure/Repositories/VoteRepository.cs`
    - Adds `HasVotedAsync(...)` to support "one vote per user per nomination".
  - `src/UmaFestHub.Domain/Interfaces/IPersonalListRepository.cs` + `src/UmaFestHub.Infrastructure/Repositories/PersonalListRepository.cs`
    - Implements persistence for lists stored as `ExternalFilmIds` (serialized int collection).
  - `src/UmaFestHub.Infrastructure/Repositories/ReviewRepository.cs`
    - Used by `ReviewService` to persist moderation state transitions.

- **Service Layer / Application Service (Façade-style orchestration)**:
  - `src/UmaFestHub.Application/Services/AwardService.cs`
    - Orchestrates voting rules and winner computation across nomination/vote repositories.
  - `src/UmaFestHub.Application/Services/ReviewService.cs`
    - Orchestrates moderation workflow (`Report`, `Hide`, `Approve`) while repositories handle persistence.
  - `src/UmaFestHub.Application/Services/PersonalListService.cs`
    - Orchestrates list operations using external (TMDb) film ids.
  - `src/UmaFestHub.Application/Services/FestivalService.cs`
    - CRUD operations for festivals with early bird configuration.
  - `src/umaFestHub.Application/Services/CartService.cs`
    - Cart operations with validation pipeline.
  - `src/UmaFestHub.Application/Services/PurchaseService.cs`
    - Checkout flow with observer notifications.

- **Strategy Pattern (pricing & entitlements)**:
  - `src/UmaFestHub.Application/Strategies/*EntitlementStrategy.cs` (4 implementations)
  - `src/UmaFestHub.Application/Pricing/EarlyBirdPricingDecorator.cs`
  - `src/UmaFestHub.Application/Recommendations/RecommendationService.cs`

- **Decorator Pattern (pricing)**:
  - `src/UmaFestHub.Application/Pricing/EarlyBirdPricingDecorator.cs` wraps `BasePricingStrategy`
  - Configurable via Festival settings (discount %, days before)

- **Chain of Responsibility (validation)**:
  - `src/UmaFestHub.Application/Validators/CartValidationPipeline.cs`
  - `src/umaFestHub.Application/Validators/PurchaseValidationPipeline.cs`

- **Observer Pattern (notifications)**:
  - `src/UmaFestHub.Application/Observers/PurchaseCompleted/` (multiple observers)
  - `src/UmaFestHub.Application/Observers/RentalExpiry/`
  - `src/UmaFestHub.Application/Observers/FestivalEnding/`
  - `src/UmaFestHub.Application/Observers/Reviews/`
  - `src/UmaFestHub.Application/Observers/Awards/`

- **Factory Pattern (product creation)**:
  - `src/UmaFestHub.Application/Factories/ProductFactory.cs` (ProductStore classes)
  - `src/UmaFestHub.Application/Factories/ProductFactoryImpl.cs` (ProductFactory facade)

- **MVC (Controller + View) for UI flows**:
  - Various controllers and views for festivals, reviews, personal lists, cart, checkout

- **Dependency Injection / IoC (wiring services/repositories)**:
  - `src/UmaFestHub.Application/Extensions/DependencyInjectionExtensions.cs` registers all application services
  - `src/UmaFestHub.Infrastructure/Extensions/DependencyInjectionExtensions.cs` registers repositories

- **Options Pattern (Configuration)**:
  - `src/UmaFestHub.Web/Program.cs` binds app settings to strongly typed classes (e.g., `FestivalEndingReminderOptions`, `RentalExpiryReminderOptions`) for safe DI injection.

- **Background Service / Worker Pattern**:
  - Utilizes `IHostedService` via `AddHostedService<T>()` in `Program.cs` (e.g., `FestivalEndingReminderWorker`, `RentalExpiryReminderWorker`) for reliable out-of-band processing.

- **Distributed Observer / Pub-Sub (SignalR)**:
  - `NotificationHub` and `AddSignalR()` in `Program.cs` implement real-time WebSockets to push events to subscribed browser clients.