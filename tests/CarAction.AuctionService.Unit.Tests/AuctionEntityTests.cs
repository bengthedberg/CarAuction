using CarAction.AuctionService.Entities;

namespace CarAction.AuctionService.Unit.Tests;

public class AuctionEntityTests
{
    [Fact]
    public void HasReservePrice_ReservePriceGreaterThanZero_True()
    {
        // Arrange
        var auction = new Auction
        {
            Id = Guid.NewGuid(),
            ReservePrice = 1
        };

        // Act
        var result = auction.HasReservePrice();

        // Assert
        Assert.True(result);
    }

    [Fact]
    public void HasReservePrice_ReservePriceIsZero_False()
    {
        // Arrange
        var auction = new Auction
        {
            Id = Guid.NewGuid(),
            ReservePrice = 0
        };

        // Act
        var result = auction.HasReservePrice();

        // Assert
        Assert.False(result);
    }
}