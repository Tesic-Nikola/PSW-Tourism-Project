using AutoMapper;
using Explorer.Bookings.API.Dtos;
using Explorer.Bookings.Core.Domain;

namespace Explorer.Bookings.Core.Mappers;

public class BookingsProfile : Profile
{
    public BookingsProfile()
    {
        CreateMap<ShoppingCart, ShoppingCartDto>().ReverseMap();
        CreateMap<CartItem, CartItemDto>().ReverseMap();
        CreateMap<TourPurchase, TourPurchaseDto>()
            .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.Status.ToString()))
            .ReverseMap()
            .ForMember(dest => dest.Status, opt => opt.MapFrom(src => Enum.Parse<PurchaseStatus>(src.Status)));
        CreateMap<PurchaseItem, PurchaseItemDto>().ReverseMap();
        CreateMap<BonusPoints, BonusPointsDto>().ReverseMap();
    }
}