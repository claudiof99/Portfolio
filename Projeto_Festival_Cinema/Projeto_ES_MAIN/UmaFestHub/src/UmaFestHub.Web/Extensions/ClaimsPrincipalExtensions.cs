
using System.Security.Claims;

namespace UmaFestHub.Web.Extensions;

public static class ClaimsPrincipalExtensions
{
    public static bool TryGetCurrentUserId(this ClaimsPrincipal user, out Guid userId)
    {
        var value = user.FindFirst("sub")?.Value
            ?? user.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        return Guid.TryParse(value, out userId);
    }
}