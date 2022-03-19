using AutoMapper;
using Vehicles.API.Data.Entities;
using Vehicles.API.Models;

namespace Vehicles.API.Profiles
{
    public class DetailProfile : Profile
    {
        public DetailProfile()
        {
            CreateMap<Detail, DetailViewModel>()
                .ForMember(d => d.IdDetalle, o => o.MapFrom(src => src.Id))
                .ReverseMap();
        }
    }
}
