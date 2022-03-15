using AutoMapper;
using Vehicles.API.Data.Entities;
using Vehicles.API.Models;

namespace Vehicles.API.Profiles
{
    public class UserProfile : Profile
    {
        public UserProfile()
        {
            CreateMap<User, UserViewModel>()
                .ForMember(d=>d.Email,o=>o.MapFrom(src=>src.UserName))
                .ReverseMap();
        }
    }
}
