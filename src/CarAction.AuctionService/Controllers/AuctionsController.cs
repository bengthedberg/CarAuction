using AutoMapper;
using AutoMapper.QueryableExtensions;

using CarAction.AuctionService.Data;
using CarAction.AuctionService.DTOs;
using CarAction.AuctionService.Entities;
using CarAction.Contracts.Auctions;

using MassTransit;

using Microsoft.AspNetCore.Authentication.JwtBearer;

using Microsoft.AspNetCore.Authorization;

using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CarAction.AuctionService.Controller;

[ApiController]
[Route("api/auctions")]
public class AuctionsController : ControllerBase
{
    private readonly IAuctionRepository _repo;
    private readonly IMapper _mapper;
    private readonly IPublishEndpoint _publishEndpoint;

    public AuctionsController(IAuctionRepository repo, IMapper mapper, IPublishEndpoint publishEndpoint)
    {
        _repo = repo;

        _mapper = mapper;
        _publishEndpoint = publishEndpoint;
    }


    /// <summary>
    /// Get all actions
    /// </summary>
    /// <param name="date">optional date to limit auctions created since this date</param>
    /// <returns>List of AuctionDTO</returns>
    [HttpGet]
    public async Task<ActionResult<List<AuctionDTO>>> GetAllAuctions(string date)
    {
        return await _repo.GetAuctionsAsync(date);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<AuctionDTO>> GetAuctionById(Guid id)
    {
        var auction = await _repo.GetAuctionByIdAsync(id);

        return (auction == null)
            ? NotFound()
            : _mapper.Map<AuctionDTO>(auction);
    }

    [Authorize]
    [HttpPost]
    public async Task<ActionResult<AuctionDTO>> CreateAuction(CreateAuctionDTO auctionDto)
    {
        var auction = _mapper.Map<Auction>(auctionDto);

        auction.Seller = User.Identity.Name; // As we specified the NameClaimType at startup

        _repo.AddAuction(auction);

        // Create a DTO with the new Auction Id
        var newAuction = _mapper.Map<AuctionDTO>(auction);

        // Send the event to the message broker
        await _publishEndpoint.Publish<AuctionCreated>(_mapper.Map<AuctionCreated>(newAuction));

        // return the number of created records
        var result = await _repo.SaveChangesAsync();

        if (result)
        {
            // Return the created record
            return CreatedAtAction(nameof(GetAuctionById), new { id = auction.Id }, newAuction);
        }
        else
        {
            return BadRequest("Failed to save the auction");
        }
    }

    [Authorize]
    [HttpPut("{id}")]
    public async Task<ActionResult> UpdateAuction(Guid id, UpdateAuctionDTO auctionDto)
    {
        // Get existing record
        var auction = await _repo.GetAuctionEntityByIdAsync(id);

        if (auction == null)
            return NotFound();

        // Check that seller matches the original seller name
        if (auction.Seller != User.Identity.Name)
            return Forbid();

        // NOTE: Usually not recommended to update all of the values.
        // Recommended to implement some logic in the entity to validate what can change.
        auction.Item.Year = auctionDto.Year ?? auction.Item.Year;
        auction.Item.Make = auctionDto.Make ?? auction.Item.Make;
        auction.Item.Model = auctionDto.Model ?? auction.Item.Model;
        auction.Item.Mileage = auctionDto.Mileage ?? auction.Item.Mileage;
        auction.Item.Color = auctionDto.Color ?? auction.Item.Color;

        // Send the event to the message broker
        await _publishEndpoint.Publish(_mapper.Map<AuctionUpdated>(auction));

        var result = await _repo.SaveChangesAsync();

        return result
            ? NoContent()
            : BadRequest("Failed to update the auction");
    }

    // NOTE: Normally a delete is not actually relevant in a production system, possible mark the record as removed.
    [Authorize]
    [HttpDelete("{id}")]
    public async Task<ActionResult> DeleteAuction(Guid id)
    {
        // Get existing record
        var auction = await _repo.GetAuctionEntityByIdAsync(id);

        if (auction == null)
            return NotFound();

        // Check that seller matches the original seller name
        if (auction.Seller != User.Identity.Name)
            return Forbid();

        _repo.RemoveAuction(auction);

        // Send the event to the message broker
        await _publishEndpoint.Publish<AuctionDeleted>(_mapper.Map<AuctionDeleted>(new AuctionDeleted { Id = id.ToString() }));

        var result = await _repo.SaveChangesAsync();

        return result
            ? NoContent()
            : BadRequest("Failed to delete the auction");
    }

}
