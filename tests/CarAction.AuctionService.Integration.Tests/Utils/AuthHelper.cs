using System.Security.Claims;

namespace CarAction.AuctionService.Integration.Tests.Utils;

/// <summary>
/// Helper class to manipulate the fake jwt bearer token.
/// </summary>
public class AuthHelper
{
    public static Dictionary<string, object> GetBearerForUser(string username)
    {
        return new Dictionary<string, object> { { ClaimTypes.Name, username } };
    }
}
