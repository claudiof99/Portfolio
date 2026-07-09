namespace UmaFestHub.Web.Security;

// Central place for role names / role groups used by the Web layer.

public static class RoleConstants
{
	public const string Customer = "Customer";
	public const string Admin = "Admin";
	public const string Organizer = "Organizer";
	public const string Manager = "Manager";

	// For [Authorize(Roles = "...")] attributes (must be compile-time constants).
	public const string CustomerRolesCsv = "Customer";
	public const string ModeratorRolesCsv = "Manager,Organizer,Admin";
	public const string OrganizerOrAdminRolesCsv = "Organizer,Admin";

	// For runtime checks (e.g., IsInAnyRole).
	public static readonly string[] ModeratorRoles = [Manager, Organizer, Admin];
	public static readonly string[] AutoApproveRoles = [Organizer, Admin];
	public static readonly string[] OrganizerOrAdminRoles = [Organizer, Admin];
}

