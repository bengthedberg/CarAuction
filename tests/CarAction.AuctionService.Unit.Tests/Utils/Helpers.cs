using System.Security.Claims;

namespace CarAction.AuctionService.Unit.Tests.Utils;

public class Helpers
{
    public static string userName = "test";
    public static ClaimsPrincipal GetClaimsPrincipal()
    {
        var claims = new List<Claim> { new Claim(ClaimTypes.Name, userName) };
        var identity = new ClaimsIdentity(claims, "testing");
        return new ClaimsPrincipal(identity);
    }

}
