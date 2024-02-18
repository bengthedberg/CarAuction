using AutoMapper;
using AutoMapper.QueryableExtensions;

using CarAction.AuctionService.Data;
using CarAction.AuctionService.DTOs;
using CarAction.AuctionService.Entities;

using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CarAction.AuctionService.Controller;

[ApiController]
[Route("api/auctions")]
public class AuctionsController : ControllerBase
{
    private readonly AuctionDbContext _context;
    private readonly IMapper _mapper;

    public AuctionsController(AuctionDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }


    /// <summary>
    /// Get all actions
    /// </summary>
    /// <param name="date">optional date to limit auctions created since this date</param>
    /// <returns>List of AuctionDTO</returns>
    [HttpGet]
    public async Task<ActionResult<List<AuctionDTO>>> GetAllAuctions(string date)
    {
        var query = _context.Auctions.OrderBy(x => x.Item.Make).AsQueryable();

        if (!string.IsNullOrEmpty(date))
        {
            query = query.Where(x => x.UpdatedAt.CompareTo(DateTime.Parse(date).ToUniversalTime()) > 0);
        }

        return await query.ProjectTo<AuctionDTO>(_mapper.ConfigurationProvider).ToListAsync();

    }

    [HttpGet("{id}")]
    public async Task<ActionResult<AuctionDTO>> GetAuctionById(Guid id)
    {
        var auction = await _context.Auctions
        .Include(x => x.Item)
        .FirstOrDefaultAsync(x => x.Id == id);

        return (auction == null)
            ? NotFound()
            : _mapper.Map<AuctionDTO>(auction);
    }

    [HttpPost]
    public async Task<ActionResult<AuctionDTO>> CreateAuction(CreateAuctionDTO auctionDto)
    {
        var auction = _mapper.Map<Auction>(auctionDto);

        // TODO: Add current user as seller
        auction.Seller = "Test";

        _context.Auctions.Add(auction);

        // return the number of created records
        var result = await _context.SaveChangesAsync() > 0;

        return result
        ? CreatedAtAction(nameof(GetAuctionById), new { id = auction.Id }, _mapper.Map<AuctionDTO>(auction))
        : BadRequest("Failed to save the auction");
    }

    [HttpPut("{id}")]
    public async Task<ActionResult> UpdateAuction(Guid id, UpdateAuctionDTO auctionDto)
    {
        // Get existing record
        var auction = await _context.Auctions.Include(x => x.Item)
            .FirstOrDefaultAsync(x => x.Id == id);

        if (auction == null)
            return NotFound();

        // TODO: check that seller matches the original seller name

        // NOTE: Usually not recommended to update all of the values.
        // Recommended to implement some logic in the entity to validate what can change.
        auction.Item.Year = auctionDto.Year ?? auction.Item.Year;
        auction.Item.Make = auctionDto.Make ?? auction.Item.Make;
        auction.Item.Model = auctionDto.Model ?? auction.Item.Model;
        auction.Item.Mileage = auctionDto.Mileage ?? auction.Item.Mileage;
        auction.Item.Color = auctionDto.Color ?? auction.Item.Color;

        var result = await _context.SaveChangesAsync() > 0;

        return result
            ? NoContent()
            : BadRequest("Failed to update the auction");
    }

    // NOTE: Normally a delete is not actually relevant in a production system, possible mark the record as removed.
    [HttpDelete("{id}")]
    public async Task<ActionResult> DeleteAuction(Guid id)
    {
        // Get existing record
        var auction = await _context.Auctions.Include(x => x.Item)
            .FirstOrDefaultAsync(x => x.Id == id);

        if (auction == null)
            return NotFound();

        // TODO: check that seller matches the original seller name

        _context.Auctions.Remove(auction);

        var result = await _context.SaveChangesAsync() > 0;

        return result
            ? NoContent()
            : BadRequest("Failed to delete the auction");
    }

}
