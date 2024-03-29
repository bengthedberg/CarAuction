﻿using AutoMapper;

using CarAction.AuctionService.DTOs;
using CarAction.AuctionService.Entities;
using CarAction.Contracts.Auctions;

namespace CarAction.AuctionService.RequestHelpers;

public class MappingProfiles : Profile
{
    public MappingProfiles()
    {
        CreateMap<Auction, AuctionDTO>().IncludeMembers(x => x.Item);
        CreateMap<Item, AuctionDTO>();
        CreateMap<CreateAuctionDTO, Auction>()
            .ForMember(d => d.Item, opt => opt.MapFrom(s => s));
        CreateMap<CreateAuctionDTO, Item>();

        CreateMap<AuctionDTO, AuctionCreated>();
        CreateMap<Auction, AuctionUpdated>().IncludeMembers(a => a.Item);
        CreateMap<Item, AuctionUpdated>();
    }
}
