using AutoMapper;
using Explorer.Tours.API.Dtos;
using Explorer.Tours.Core.Domain;

namespace Explorer.Tours.Core.Mappers;

public class ToursProfile : Profile
{
    public ToursProfile()
    {
        CreateMap<EquipmentDto, Equipment>().ReverseMap();
        CreateMap<TourDto, Tour>().ReverseMap()
            .ForMember(dest => dest.Difficulty, opt => opt.MapFrom(src => (int)src.Difficulty))
            .ForMember(dest => dest.Category, opt => opt.MapFrom(src => (int)src.Category))
            .ForMember(dest => dest.Status, opt => opt.MapFrom(src => (int)src.Status));
        CreateMap<KeyPointDto, KeyPoint>().ReverseMap();
    }
}