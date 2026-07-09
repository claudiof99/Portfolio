// In-app notifications: SignalR hub — authenticated clients join user:{id} and role:{name} groups.
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;

namespace UmaFestHub.Web.Hubs;

/// <summary>Real-time notifications: clients join <c>role:{RoleName}</c> and <c>user:{Guid}</c> groups on connect.</summary>
[Authorize]
public sealed class NotificationHub : Hub
{
	public const string RoleGroupPrefix = "role:";
	public const string UserGroupPrefix = "user:";

	public static string RoleGroupName(string role) => RoleGroupPrefix + role;

	public static string UserGroupName(Guid userId) => UserGroupPrefix + userId.ToString("D");

	public override async Task OnConnectedAsync()
	{
		var user = Context.User;
		if (user?.Identity?.IsAuthenticated == true)
		{
			var userIdClaim = user.FindFirst("sub")?.Value ?? user.FindFirst(ClaimTypes.NameIdentifier)?.Value;
			if (Guid.TryParse(userIdClaim, out var userId))
			{
				await Groups.AddToGroupAsync(Context.ConnectionId, UserGroupName(userId));
			}

			foreach (var role in user.FindAll(ClaimTypes.Role).Select(c => c.Value).Distinct(StringComparer.Ordinal))
			{
				await Groups.AddToGroupAsync(Context.ConnectionId, RoleGroupName(role));
			}
		}

		await base.OnConnectedAsync();
	}
}
