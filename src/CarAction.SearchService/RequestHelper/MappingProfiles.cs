using AutoMapper;

using CarAction.Contracts.Auctions;
using CarAction.SearchService.Models;

namespace CarAction.SearchService;

public class MappingProfiles : Profile
{
    public MappingProfiles()
    {
        CreateMap<AuctionCreated, Item>();
        CreateMap<AuctionUpdated, Item>();
    }
}
