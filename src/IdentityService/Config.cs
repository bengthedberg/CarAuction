using Duende.IdentityServer.Models;

namespace IdentityService;

public static class Config
{
    public static IEnumerable<IdentityResource> IdentityResources =>
        new IdentityResource[]
        {
            new IdentityResources.OpenId(),
            new IdentityResources.Profile(),
        };

    public static IEnumerable<ApiScope> ApiScopes =>
        new ApiScope[]
        {
            new ApiScope("auctionApp", "Auction App full access")
        };

    public static IEnumerable<Client> Clients =>
        new Client[]
        {
            // m2m (machine 2 machine) client credentials flow client
            // new Client
            // {
            //     ClientId = "m2m.client",
            //     ClientName = "Client Credentials Client",

            //     AllowedGrantTypes = GrantTypes.ClientCredentials,
            //     ClientSecrets = { new Secret("511536EF-F270-4058-80CA-1C89C192F69A".Sha256()) },

            //     AllowedScopes = { "scope1" }
            // },

            // Access used in postman for development
            // See https://docs.duendesoftware.com/identityserver/v7/tokens/password_grant/
            new Client
            {
                ClientId = "postman",
                ClientName = "Postman",
                AllowedScopes = { "openid", "profile", "auctionApp" },

                RedirectUris = { "https://www.getpostman.com/oauth2/callback" }, // Not used
                ClientSecrets = { new Secret("NotASecret".Sha256()) },  // Just for development so 'NotASecret' secret
                AllowedGrantTypes = { GrantType.ResourceOwnerPassword }
            },
            new Client
            {
                ClientId = "nextApp",
                ClientName = "nextApp",
                ClientSecrets = {new Secret("secret".Sha256())},
                AllowedGrantTypes = GrantTypes.CodeAndClientCredentials, // No browser involvment, as we use NextJS server
                RequirePkce = false, // Dont store credentials in client (browser) if you create a client web app or a mobile app then this will be true.
                RedirectUris = {"http://localhost:3000/api/auth/callback/id-server"},
                AllowOfflineAccess = true, // Enable refresh token
                AllowedScopes = {"openid", "profile", "auctionApp"},
                AccessTokenLifetime = 3600*24*30,  // Dont use this long authentication in production
                AlwaysIncludeUserClaimsInIdToken = true
            }
        };
}
