using System.Security.Claims;

using Duende.IdentityServer.Models;
using Duende.IdentityServer.Services;

using IdentityModel;

using IdentityService.Models;

using Microsoft.AspNetCore.Identity;

namespace IdentityService;

public class CustomerProfileService : IProfileService
{
    private readonly UserManager<ApplicationUser> _userManager;

    public CustomerProfileService(UserManager<ApplicationUser> userManager)
    {
        _userManager = userManager;
    }

    public async Task GetProfileDataAsync(ProfileDataRequestContext context)
    {
        var user = await _userManager.GetUserAsync(context.Subject); // User Id
        var existingClaims = await _userManager.GetClaimsAsync(user);

        var claims = new List<Claim>
        {
            new Claim("username", user.UserName ?? "unknown"),
        };

        context.IssuedClaims.AddRange(claims);
        context.IssuedClaims.Add(existingClaims.FirstOrDefault(x => x.Type == JwtClaimTypes.Name) ?? default!);
    }

    public async Task IsActiveAsync(IsActiveContext context) => await Task.CompletedTask;
}
