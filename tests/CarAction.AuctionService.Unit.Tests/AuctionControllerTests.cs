using AutoFixture;

using AutoMapper;

using CarAction.AuctionService.Controller;
using CarAction.AuctionService.Data;
using CarAction.AuctionService.DTOs;
using CarAction.AuctionService.Entities;
using CarAction.AuctionService.RequestHelpers;
using CarAction.AuctionService.Unit.Tests.Utils;

using MassTransit;

using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

using Moq;

namespace CarAction.AuctionService.Unit.Tests;

public class AuctionControllerTests
{
    private readonly Mock<IAuctionRepository> _auctionRepo;
    private readonly Mock<IPublishEndpoint> _publishEndpoint;
    private readonly Fixture _fixture;
    private readonly AuctionsController _controller;
    private readonly IMapper _mapper;

    // XUnit will run constructor for every test.
    public AuctionControllerTests()
    {
        _fixture = new Fixture();
        _auctionRepo = new Mock<IAuctionRepository>();
        _publishEndpoint = new Mock<IPublishEndpoint>();

        // Don't mock the mapping, just create our own instance from the mappinp configured in the Auction Service.
        var mockMapper = new MapperConfiguration(mc =>
        {
            mc.AddMaps(typeof(MappingProfiles).Assembly);
        }).CreateMapper().ConfigurationProvider;
        _mapper = new Mapper(mockMapper);

        // Create a controller with required dependencies; either mocks or real.
        _controller = new AuctionsController(_auctionRepo.Object, _mapper, _publishEndpoint.Object)
        {
            ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext { User = Helpers.GetClaimsPrincipal() }
            }
        };
    }

    [Fact]
    public async Task GetAllAuctions_WithNoParams_ReturnsAuctions()
    {
        // Arrange
        var numItems = 10;
        var auctions = _fixture.CreateMany<AuctionDTO>(numItems).ToList();
        _auctionRepo.Setup(x => x.GetAuctionsAsync(null)).ReturnsAsync(auctions);

        // Act
        var result = await _controller.GetAllAuctions(null);

        // Assert
        Assert.Equal(numItems, result.Value!.Count);
        Assert.IsType<ActionResult<List<AuctionDTO>>>(result);
    }

    [Fact]
    public async Task GetAuctionById_WithExistingAuction_ReturnsAuction()
    {
        // Arrange
        var auction = _fixture.Create<AuctionDTO>();
        _auctionRepo.Setup(x => x.GetAuctionByIdAsync(It.IsAny<Guid>())).ReturnsAsync(auction);

        // Act
        var result = await _controller.GetAuctionById(auction.Id);

        // Assert
        Assert.Equal(auction.Model, result.Value!.Model);
        Assert.IsType<ActionResult<AuctionDTO>>(result);
    }
    [Fact]
    public async Task GetAuctionById_WithNoneExistingAuction_ReturnsNotFound()
    {
        // Arrange
        _auctionRepo.Setup(repo => repo.GetAuctionByIdAsync(It.IsAny<Guid>()))
            .ReturnsAsync(value: null);

        // Act
        var result = await _controller.GetAuctionById(Guid.NewGuid());

        // Assert
        Assert.IsType<NotFoundResult>(result.Result);
    }

    [Fact]
    public async Task CreateAuction_WithValidCreateAuctionDTO_ReturnsCreated()
    {
        // Arrange
        var auction = _fixture.Create<CreateAuctionDTO>();
        _auctionRepo.Setup(repo => repo.AddAuction(It.IsAny<Auction>()));
        _auctionRepo.Setup(repo => repo.SaveChangesAsync()).ReturnsAsync(true);

        // Act
        var result = await _controller.CreateAuction(auction);
        var createdResult = result.Result as CreatedAtActionResult;

        // Assert
        Assert.NotNull(createdResult);
        Assert.Equal("GetAuctionById", createdResult.ActionName);
        Assert.IsType<AuctionDTO>(createdResult.Value);
    }

    [Fact]
    public async Task CreateAuction_FailedSave_Returns400BadRequest()
    {
        // arrange
        var auctionDto = _fixture.Create<CreateAuctionDTO>();
        _auctionRepo.Setup(repo => repo.AddAuction(It.IsAny<Auction>()));
        _auctionRepo.Setup(repo => repo.SaveChangesAsync()).ReturnsAsync(false);

        // act
        var result = await _controller.CreateAuction(auctionDto);

        // assert
        Assert.IsType<BadRequestObjectResult>(result.Result);
    }
}
