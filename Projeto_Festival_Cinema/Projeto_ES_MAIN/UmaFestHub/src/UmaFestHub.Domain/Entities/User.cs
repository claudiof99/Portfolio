namespace UmaFestHub.Domain.Entities;
using UmaFestHub.Domain.Enums;
public class User
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string PasswordHash { get; set; } = string.Empty;
    public string PasswordSalt { get; set; } = string.Empty;
    public string? PasswordResetToken { get; set; }
    public DateTime? PasswordResetTokenExpiry { get; set; }
    public ICollection<UserRole> Roles { get; set; } = new List<UserRole> { UserRole.Customer };
    public DateTime CreatedAtUtc { get; set; } = DateTime.UtcNow;
    public ICollection<Review> Reviews { get; set; } = new List<Review>();
    /// <summary>Replies authored by this user on review threads (see <see cref="ReviewReply"/>).</summary>
    public ICollection<ReviewReply> ReviewReplies { get; set; } = new List<ReviewReply>();

    /// <summary>Watchlist / favorites / watched memberships (see <see cref="PersonalList"/>).</summary>
    public ICollection<PersonalList> PersonalLists { get; set; } = new List<PersonalList>();
    public ICollection<Vote> Votes { get; set; } = new List<Vote>();
    public ICollection<Cart> Carts { get; set; } = new List<Cart>();
    public ICollection<Purchase> Purchases { get; set; } = new List<Purchase>();
}

