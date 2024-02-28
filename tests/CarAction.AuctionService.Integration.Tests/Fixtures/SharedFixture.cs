namespace CarAction.AuctionService.Integration.Tests.Fixtures;


/// <summary>
/// Include the same CustomWebAppFactory across all collection, remove the IClassFixture<CustomWebAppFactory>
/// in any test classes.
/// NOTE: The name in the attribute must corresponds with the test classes.
/// </summary>
[CollectionDefinition("Shared collection")]
public class SharedFixture : ICollectionFixture<CustomWebAppFactory>
{

}