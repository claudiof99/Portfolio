// -----------------------------------------------------------------------------
// Awards, nominations & votes — DbSets Awards / AwardNominations / Votes and EF
// configuration (festival link, composite vote key, nomination FKs) live in OnModelCreating.
// Review replies: DbSet ReviewReplies + ReviewReply fluent config (FK Review, User).
// In-app notifications: DbSet Notifications + fluent mapping to table "Notifications".
// -----------------------------------------------------------------------------
using Microsoft.EntityFrameworkCore;
using UmaFestHub.Domain.Entities;
using UmaFestHub.Domain.ValueObjects;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using UmaFestHub.Domain.Enums;
using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace UmaFestHub.Infrastructure.Data;

public class AppDbContext : DbContext
{
	public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
	{
	}

	public DbSet<User> Users => Set<User>();
	public DbSet<Film> Films => Set<Film>();
	public DbSet<Festival> Festivals => Set<Festival>();
	public DbSet<FestivalFilm> FestivalFilms => Set<FestivalFilm>();
	public DbSet<Session> Sessions => Set<Session>();
	public DbSet<PremierSession> PremierSessions => Set<PremierSession>();
	public DbSet<FixedSession> FixedSessions => Set<FixedSession>();
	public DbSet<AccessWindowSession> AccessWindowSessions => Set<AccessWindowSession>();
	public DbSet<Review> Reviews => Set<Review>();
	/// <summary>Text replies under <see cref="Review"/> rows (thread + moderation columns).</summary>
	public DbSet<ReviewReply> ReviewReplies => Set<ReviewReply>();

	/// <summary>Per-user, per-type film list rows (FK to user and film).</summary>
	public DbSet<PersonalList> PersonalLists => Set<PersonalList>();
	public DbSet<Award> Awards => Set<Award>();
	public DbSet<AwardNomination> AwardNominations => Set<AwardNomination>();
	public DbSet<Vote> Votes => Set<Vote>();
	public DbSet<Cart> Carts => Set<Cart>();
	public DbSet<CartItem> CartItems => Set<CartItem>();
	public DbSet<Purchase> Purchases => Set<Purchase>();
	public DbSet<PurchaseItem> PurchaseItems => Set<PurchaseItem>();
	public DbSet<Product> Products => Set<Product>();
	public DbSet<Pass> Passes => Set<Pass>();
	public DbSet<DailyPass> DailyPasses => Set<DailyPass>();
	public DbSet<CompletePass> CompletePasses => Set<CompletePass>();
	public DbSet<Rental> Rentals => Set<Rental>();
	public DbSet<Ticket> Tickets => Set<Ticket>();
	public DbSet<Genre> Genres => Set<Genre>();
	public DbSet<CreditFilm> Credits => Set<CreditFilm>();
	public DbSet<Person> Persons => Set<Person>();
	/// <summary>Queued in-app notifications (replay after login).</summary>
	public DbSet<Notification> Notifications => Set<Notification>();

	protected override void OnModelCreating(ModelBuilder modelBuilder)
	{
		base.OnModelCreating(modelBuilder);

		var rolesConverter = new ValueConverter<ICollection<UserRole>, string>(
			roles => string.Join(',', roles.Select(x => x.ToString())),
			value => value.Split(',', StringSplitOptions.RemoveEmptyEntries)
				.Select(x => Enum.Parse<UserRole>(x))
				.ToList());

		var rolesComparer = new ValueComparer<ICollection<UserRole>>(
			(c1, c2) => c1!.SequenceEqual(c2!),
			c => c.Aggregate(0, (a, v) => HashCode.Combine(a, v.GetHashCode())),
			c => c.ToList());

		var intCollectionConverter = new ValueConverter<ICollection<int>, string>(
			items => string.Join(',', items),
			value => string.IsNullOrWhiteSpace(value)
				? new List<int>()
				: value.Split(',', StringSplitOptions.RemoveEmptyEntries).Select(int.Parse).ToList());

		var intCollectionComparer = new ValueComparer<ICollection<int>>(
			(c1, c2) => c1!.SequenceEqual(c2!),
			c => c.Aggregate(0, (a, v) => HashCode.Combine(a, v.GetHashCode())),
			c => c.ToList());

		modelBuilder.Entity<User>(entity =>
		{
			entity.HasKey(x => x.Id);
			entity.Property(x => x.Name).IsRequired().HasMaxLength(200);
			entity.Property(x => x.Email).IsRequired().HasMaxLength(255);
			entity.Property(x => x.PasswordResetToken)
				.HasMaxLength(200)
				.IsRequired(false);
			entity.Property(x => x.PasswordResetTokenExpiry)
				.IsRequired(false);
			entity.Property(x => x.Roles)
				.HasConversion(rolesConverter, rolesComparer)
				.HasColumnType("longtext");
			entity.HasIndex(x => x.Email).IsUnique();
		});

		modelBuilder.Entity<Film>(entity =>
		{
			entity.HasKey(x => x.Id);
			entity.Property(x => x.Name).IsRequired().HasMaxLength(200);
			entity.Property(x => x.Url).HasMaxLength(500);
			entity.Property(x => x.Description).HasMaxLength(2000);
			entity.HasIndex(x => x.ExternalId);
			entity.Property(x => x.TmdbPopularity).HasPrecision(10, 4);
			entity.OwnsOne(x => x.Duration, owned =>
			{
				owned.Property(d => d.Value).HasColumnName("FilmDurationValue");
				owned.Property(d => d.Unit).HasConversion<string>().HasColumnName("FilmDurationUnit");
			});
		});

		modelBuilder.Entity<Genre>(entity =>
		{
			entity.HasKey(x => x.Id);
			entity.Property(x => x.Name).IsRequired().HasMaxLength(80);
			entity.HasOne(x => x.Film)
				.WithMany(x => x.Genres)
				.HasForeignKey(x => x.FilmId)
				.OnDelete(DeleteBehavior.Cascade);
		});

		modelBuilder.Entity<Person>(entity =>
		{
			entity.HasKey(x => x.Id);
			entity.Property(x => x.Name).IsRequired().HasMaxLength(200);
		});

		// In-app notifications: maps Notification entity to SQL table "Notifications".
		modelBuilder.Entity<Notification>(entity =>
		{
			entity.ToTable("Notifications");
			entity.HasKey(x => x.Id);
			entity.Property(x => x.Title).IsRequired().HasMaxLength(512);
			entity.Property(x => x.Message).IsRequired().HasMaxLength(4000);
			entity.Property(x => x.TemplateJson).HasMaxLength(8000);
			entity.Property(x => x.CorrelationId).HasMaxLength(256);
			entity.Property(x => x.TargetUserRole).IsRequired().HasMaxLength(64);
			entity.Property(x => x.AcknowledgedUtc).IsRequired(false);
			entity.HasIndex(x => x.CreatedUtc);
			entity.HasIndex(x => new { x.TargetUserId, x.AcknowledgedUtc });
			entity.HasOne<User>()
				.WithMany()
				.HasForeignKey(x => x.TargetUserId)
				.OnDelete(DeleteBehavior.Cascade)
				.IsRequired();
		});

		modelBuilder.Entity<CreditFilm>(entity =>
		{
			entity.HasKey(x => x.Id);
			entity.Property(x => x.Role).IsRequired().HasMaxLength(100);
			entity.HasOne(x => x.Film)
				.WithMany(x => x.Credits)
				.HasForeignKey(x => x.FilmId)
				.OnDelete(DeleteBehavior.Cascade);
			entity.HasOne(x => x.Person)
				.WithMany()
				.HasForeignKey(x => x.PersonId)
				.OnDelete(DeleteBehavior.Restrict);
		});

		modelBuilder.Entity<Festival>(entity =>
		{
			entity.HasKey(x => x.Id);
			entity.Property(x => x.Name).IsRequired().HasMaxLength(200);
			entity.Property(x => x.Description).HasMaxLength(2000);
			entity.Property(x => x.StartDateUtc).IsRequired();
			entity.Property(x => x.EndDateUtc).IsRequired();
			entity.Property(x => x.IsHidden).IsRequired().HasDefaultValue(false);
		});

		modelBuilder.Entity<FestivalFilm>(entity =>
		{
			entity.HasKey(x => x.Id);
			entity.Property(x => x.ProgrammingNotes).HasMaxLength(2000);
			entity.Property(x => x.AddedAtUtc).IsRequired();
			entity.HasIndex(x => new { x.FestivalId, x.FilmId }).IsUnique();
			entity.HasOne(x => x.Festival)
				.WithMany(x => x.FestivalFilms)
				.HasForeignKey(x => x.FestivalId)
				.OnDelete(DeleteBehavior.Cascade);
			entity.HasOne(x => x.Film)
				.WithMany(x => x.FestivalFilms)
				.HasForeignKey(x => x.FilmId)
				.OnDelete(DeleteBehavior.Cascade);
		});

		modelBuilder.Entity<Session>(entity =>
		{
			entity.HasKey(x => x.Id);
			entity.Property(x => x.SessionType).IsRequired().HasMaxLength(60);

			entity.HasDiscriminator(x => x.SessionType)
				.HasValue<PremierSession>(nameof(PremierSession))
				.HasValue<FixedSession>(nameof(FixedSession))
				.HasValue<AccessWindowSession>(nameof(AccessWindowSession));

			entity.HasOne(x => x.FestivalFilm)
				.WithMany(x => x.Sessions)
				.HasForeignKey(x => x.FestivalFilmId)
				.OnDelete(DeleteBehavior.Cascade);
		});

		modelBuilder.Entity<Review>(entity =>
		{
			entity.HasKey(x => x.Id);
			entity.Property(x => x.Comment).HasMaxLength(1200);
			entity.Property(x => x.Status).HasConversion<string>().HasMaxLength(30);
			entity.HasOne(x => x.User)
				.WithMany(x => x.Reviews)
				.HasForeignKey(x => x.UserId)
				.OnDelete(DeleteBehavior.Cascade);
			entity.HasOne(x => x.FestivalFilm)
				.WithMany(x => x.Reviews)
				.HasForeignKey(x => x.FestivalFilmId)
				.OnDelete(DeleteBehavior.SetNull);
			entity.HasOne(x => x.Film)
				.WithMany()
				.HasForeignKey(x => x.FilmId)
				.OnDelete(DeleteBehavior.Restrict);
		});

		// ReviewReply: one thread per review; Status stored as string (same pattern as Review).
		modelBuilder.Entity<ReviewReply>(entity =>
		{
			entity.HasKey(x => x.Id);
			entity.Property(x => x.Comment).IsRequired().HasMaxLength(ReviewReply.MaxCommentLength);
			entity.Property(x => x.Status).HasConversion<string>().HasMaxLength(30);
			entity.HasIndex(x => x.ReviewId);
			entity.HasOne(x => x.Review)
				.WithMany(x => x.Replies)
				.HasForeignKey(x => x.ReviewId)
				.OnDelete(DeleteBehavior.Cascade);
			entity.HasOne(x => x.User)
				.WithMany(x => x.ReviewReplies)
				.HasForeignKey(x => x.UserId)
				.OnDelete(DeleteBehavior.Cascade);
		});

		// PersonalList: string-stored enum discriminator, unique (UserId, Type, FilmId)
		modelBuilder.Entity<PersonalList>(entity =>
		{
			entity.HasKey(x => x.Id);
			entity.Property(x => x.Type)
				.HasConversion<string>()
				.IsRequired()
				.HasMaxLength(80);
			entity.HasOne(x => x.User)
				.WithMany(x => x.PersonalLists)
				.HasForeignKey(x => x.UserId)
				.OnDelete(DeleteBehavior.Cascade);
			entity.HasOne(x => x.Film)
				.WithMany()
				.HasForeignKey(x => x.FilmId)
				.OnDelete(DeleteBehavior.Cascade);
			entity.HasIndex(x => new { x.UserId, x.Type, x.FilmId }).IsUnique();
		});

		modelBuilder.Entity<Award>(entity =>
		{
			entity.HasKey(x => x.Id);
			entity.Property(x => x.Name).IsRequired().HasMaxLength(200);
			entity.Property(x => x.Category).HasConversion<string>().HasMaxLength(40);
			entity.Property(x => x.CreatedAtUtc).IsRequired();
			entity.Property(x => x.EndDateUtc).IsRequired();
			entity.Property(x => x.IsActive).IsRequired();
			entity.HasOne(x => x.Festival)
				.WithMany(x => x.Awards)
				.HasForeignKey(x => x.FestivalId)
				.OnDelete(DeleteBehavior.Cascade);
		});

		modelBuilder.Entity<AwardNomination>(entity =>
		{
			entity.HasKey(x => x.Id);
			entity.HasOne(x => x.Award)
				.WithMany(x => x.Nominations)
				.HasForeignKey(x => x.AwardId)
				.OnDelete(DeleteBehavior.Cascade);
			entity.HasOne(x => x.FestivalFilm)
				.WithMany(x => x.AwardNomination)
				.HasForeignKey(x => x.FestivalFilmId)
				.OnDelete(DeleteBehavior.SetNull);
			entity.HasOne(x => x.CreditFilm)
				.WithMany()
				.HasForeignKey(x => x.CreditFilmId)
				.OnDelete(DeleteBehavior.SetNull);
		});

	    modelBuilder.Entity<Vote>(entity =>
       {
        entity.HasKey(x => new { x.UserId, x.AwardNominationId }); 
        entity.HasOne(x => x.AwardNomination)
        .WithMany(x => x.Votes)
        .HasForeignKey(x => x.AwardNominationId)
        .OnDelete(DeleteBehavior.Cascade);
        entity.HasOne(x => x.User)
        .WithMany(x => x.Votes)
        .HasForeignKey(x => x.UserId)
        .OnDelete(DeleteBehavior.Cascade);
        });

		modelBuilder.Entity<Product>(entity =>
		{
			entity.HasKey(x => x.Id);
			entity.Property(x => x.Price).HasPrecision(18, 2);
			entity.Property(x => x.ProductType).IsRequired().HasMaxLength(60);
			entity.HasDiscriminator(x => x.ProductType)
				.HasValue<Ticket>(nameof(Ticket))
				.HasValue<DailyPass>(nameof(DailyPass))
				.HasValue<CompletePass>(nameof(CompletePass))
				.HasValue<Rental>(nameof(Rental));
		});

		modelBuilder.Entity<Pass>(entity =>
		{
			entity.HasOne(x => x.Festival)
				.WithMany(x => x.Passes)
				.HasForeignKey(x => x.FestivalId)
				.OnDelete(DeleteBehavior.Cascade);
		});

		modelBuilder.Entity<Rental>(entity =>
		{
			entity.OwnsOne(x => x.Duration, owned =>
			{
				owned.Property(d => d.Value).HasColumnName("DurationValue");
				owned.Property(d => d.Unit).HasConversion<string>().HasColumnName("DurationUnit");
			});
			entity.HasOne(x => x.FestivalFilm)
				.WithMany(x => x.Rentals)
				.HasForeignKey(x => x.FestivalFilmId)
				.OnDelete(DeleteBehavior.Cascade);
		});

		modelBuilder.Entity<Cart>(entity =>
		{
			entity.HasKey(x => x.Id);
			entity.HasIndex(x => x.UserId).IsUnique();
			entity.HasOne(x => x.User)
				.WithMany(x => x.Carts)
				.HasForeignKey(x => x.UserId)
				.OnDelete(DeleteBehavior.Cascade);
		});

		modelBuilder.Entity<CartItem>(entity =>
		{
			entity.HasKey(x => x.Id);

			entity.HasOne(x => x.Cart)
				.WithMany(x => x.CartItems)
				.HasForeignKey(x => x.CartId)
				.OnDelete(DeleteBehavior.Cascade);

			entity.HasOne(x => x.Product)
				.WithMany(x => x.CartItems)
				.HasForeignKey(x => x.ProductId)
				.OnDelete(DeleteBehavior.Restrict);
		});

		modelBuilder.Entity<Purchase>(entity =>
		{
			entity.HasKey(x => x.Id);
			entity.Property(x => x.TotalAmount).HasPrecision(18, 2);
			entity.Property(x => x.Status).HasConversion<string>().HasMaxLength(30);
			entity.HasOne(x => x.User)
				.WithMany(x => x.Purchases)
				.HasForeignKey(x => x.UserId)
				.OnDelete(DeleteBehavior.Cascade);
		});

		modelBuilder.Entity<PurchaseItem>(entity =>
		{
			entity.HasKey(x => x.Id);
			entity.Property(x => x.PriceAtPurchase).HasPrecision(18, 2);

			entity.HasOne(x => x.Purchase)
				.WithMany(x => x.PurchaseItems)
				.HasForeignKey(x => x.PurchaseId)
				.OnDelete(DeleteBehavior.Cascade);

			entity.HasOne(x => x.Product)
				.WithMany(x => x.PurchaseItems)
				.HasForeignKey(x => x.ProductId)
				.OnDelete(DeleteBehavior.Restrict);
		});

		modelBuilder.Entity<Ticket>(entity =>
		{
			entity.Property(x => x.TicketNumber).IsRequired().HasMaxLength(100);
			entity.HasIndex(x => x.TicketNumber).IsUnique();
			entity.HasOne(x => x.Session)
				.WithMany(x => x.Tickets)
				.HasForeignKey(x => x.SessionId)
				.OnDelete(DeleteBehavior.Restrict);
		});

		var seededFilmId = Guid.Parse("6E75788B-0A9C-4602-A5DE-E53A1F6D3A01");
		var seededFestivalId = Guid.Parse("D96B6B25-2B87-4E10-8F50-6D194AB49022");
		var seededFestivalFilmId = Guid.Parse("8D690E52-E89C-4B9D-B994-7F4E67D9E323");

		modelBuilder.Entity<Film>().HasData(new
		{
			Id = seededFilmId,
			ExternalId = 1001,
			Name = "Midnight Frames",
			Url = "https://example.com/midnight-frames",
			Description = "Seed film for initial local development.",
			CreatedAtUtc = new DateTime(2026, 01, 01, 0, 0, 0, DateTimeKind.Utc),
			TmdbPopularity = 0m
		});

		modelBuilder.Entity<Film>().OwnsOne(f => f.Duration).HasData(new
		{
			FilmId = seededFilmId,
			Value = 112,
			Unit = DurationUnit.Minutes
		});

		modelBuilder.Entity<Festival>().HasData(new Festival
		{
			Id = seededFestivalId,
			Name = "Uma Spring Fest",
			Description = "Join us for a week of independent and international films celebrating diverse storytelling.",
			StartDateUtc = new DateTime(2026, 04, 20, 0, 0, 0, DateTimeKind.Utc),
			EndDateUtc = new DateTime(2026, 04, 27, 0, 0, 0, DateTimeKind.Utc),
			IsHidden = false
		});

		modelBuilder.Entity<FestivalFilm>().HasData(new FestivalFilm
		{
			Id = seededFestivalFilmId,
			FestivalId = seededFestivalId,
			FilmId = seededFilmId,
			IsWorldPremier = false,
			AddedAtUtc = new DateTime(2026, 3, 1, 0, 0, 0, DateTimeKind.Utc)
		});

	}
}
