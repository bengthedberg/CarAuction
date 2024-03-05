using AutoMapper;

using CarAction.Contracts.Auctions;

using CardAction.BidService.DTOs;

namespace CardAction.BidService.RequestHelpers;

public class MappingProfiles : Profile
{
    public MappingProfiles()
    {
        CreateMap<Bid, BidDto>();
        CreateMap<Bid, BidPlaced>();
    }
}
